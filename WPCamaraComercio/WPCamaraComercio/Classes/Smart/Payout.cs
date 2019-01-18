using ITLlib;
using System;
using System.Collections.Generic;
using System.Threading;

namespace WPCamaraComercio.Classes.Smart
{
    class Payout
    {
        #region Referencias
        // ssp library variables
        private SSPComms eSSP;
        //
        private SSP_COMMAND cmd, storedCmd;
        //
        private SSP_KEYS keys;
        //
        private SSP_FULL_KEY sspKey;
        //
        private SSP_COMMAND_INFO info;
        // Variables to hold the number of notes accepted and dispensed
        private int m_TotalNotesAccepted, m_TotalNotesDispensed;
        // The type of unit this class represents, set in the setup request
        private char m_UnitType;
        //
        int m_NumberOfChannels;
        // The multiplier by which the channel values are multiplied to get their
        // true penny value.
        private int m_ValueMultiplier;
        // A list of dataset data, sorted by value. Holds the info on channel number, value, currency,
        // level and whether it is being recycled.
        private List<ChannelData> m_UnitDataList;
        //variables que muestran la suma del dinero
        private decimal intoValue;
        //variable dinero ingresado
        int noteVal;
        //
        public Action<decimal> callback;
        //
        public Action<decimal> callbackValue;
        //
        public Action<string> callbackError;

        private string statusPayout;

        #endregion

        public Payout()
        {
            eSSP = new SSPComms();
            cmd = new SSP_COMMAND();
            storedCmd = new SSP_COMMAND();
            keys = new SSP_KEYS();
            sspKey = new SSP_FULL_KEY();
            info = new SSP_COMMAND_INFO();

            m_NumberOfChannels = 0;
            m_ValueMultiplier = 1;
            m_UnitDataList = new List<ChannelData>();
            m_TotalNotesAccepted = 0;
            intoValue = 0;
            noteVal = 0;
            Init();
        }

        private void Init()
        {
            cmd.ComPort = Global.ComPort;
            cmd.SSPAddress = Global.SSPAddress;
            cmd.Timeout = 3000;
        }

        public bool IsStart()
        {
            Close();

            if (ConnectToValidator())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool ConnectToValidator()
        {

            if (Open() && NegotiateKeys())
            {
                cmd.EncryptionStatus = true;

                if (SetProtocolVersion())
                {

                    SetupRequest();

                    if (m_UnitType == (char)0x06)
                    {
                        SetInhibits();
                        EnableValidator();
                        EnablePayout();
                        return true;
                    }
                }
            }
            return false;
        }

        public void Close()
        {
            eSSP.CloseComPort();
            cmd.EncryptionStatus = false;
        }

        private bool Open()
        {
            if (eSSP.OpenSSPComPort(cmd))
            {
                return true;
            }

            return false;
        }

        public bool NegotiateKeys()
        {
            try
            {
                byte i;

                // send sync
                cmd.CommandData[0] = CCommands.SSP_CMD_SYNC;
                cmd.CommandDataLength = 1;

                if (!ValidateCommand()) return false;

                eSSP.InitiateSSPHostKeys(keys, cmd);

                // send generator
                cmd.CommandData[0] = CCommands.SSP_CMD_SET_GENERATOR;
                cmd.CommandDataLength = 9;
                for (i = 0; i < 8; i++)
                {
                    cmd.CommandData[i + 1] = (byte)(keys.Generator >> (8 * i));
                }

                if (!ValidateCommand()) return false;

                // send modulus
                cmd.CommandData[0] = CCommands.SSP_CMD_SET_MODULUS;
                cmd.CommandDataLength = 9;
                for (i = 0; i < 8; i++)
                {
                    cmd.CommandData[i + 1] = (byte)(keys.Modulus >> (8 * i));
                }

                if (!ValidateCommand()) return false;

                // send key exchange
                cmd.CommandData[0] = CCommands.SSP_CMD_KEY_EXCHANGE;
                cmd.CommandDataLength = 9;
                for (i = 0; i < 8; i++)
                {
                    cmd.CommandData[i + 1] = (byte)(keys.HostInter >> (8 * i));
                }

                if (!ValidateCommand()) return false;

                keys.SlaveInterKey = 0;
                for (i = 0; i < 8; i++)
                {
                    keys.SlaveInterKey += (UInt64)cmd.ResponseData[1 + i] << (8 * i);
                }

                eSSP.CreateSSPHostEncryptionKey(keys);

                // get full encryption key
                cmd.Key.FixedKey = 0x0123456701234567;
                cmd.Key.VariableKey = keys.KeyHost;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ValidateCommand()
        {
            // Backup data and length in case we need to retry
            byte[] backup = new byte[255];
            cmd.CommandData.CopyTo(backup, 0);
            byte length = cmd.CommandDataLength;

            // attempt to send the command
            if (eSSP.SSPSendCommand(cmd, info) == false)
            {
                eSSP.CloseComPort();
                return false;
            }
            // update the log after every command
            return true;
        }

        private bool SetProtocolVersion()
        {
            // not dealing with protocol under level 6
            // attempt to set in validator
            byte pVersion = 0x06;
            while (true)
            {
                cmd.CommandData[0] = CCommands.SSP_CMD_HOST_PROTOCOL_VERSION;
                cmd.CommandData[1] = pVersion;
                cmd.CommandDataLength = 2;

                ValidateCommand();

                if (cmd.ResponseData[0] == CCommands.SSP_RESPONSE_CMD_FAIL)
                {
                    --pVersion;
                    break;
                }
                pVersion++;

                // catch runaway
                if (pVersion > 12)
                {
                    pVersion = 0x06;
                    break;
                }
                // return default
            }
            if (pVersion >= 6)
            {
                cmd.CommandData[0] = CCommands.SSP_CMD_HOST_PROTOCOL_VERSION;
                cmd.CommandData[1] = pVersion;
                cmd.CommandDataLength = 2;

                ValidateCommand();
                return true;
            }
            return false;
        }

        public void SetupRequest()
        {
            // send setup request
            cmd.CommandData[0] = CCommands.SSP_CMD_SETUP_REQUEST;
            cmd.CommandDataLength = 1;

            if (!ValidateCommand()) return;

            // display setup request
            string displayString = "Unit Type: ";
            int index = 1;

            // unit type (table 0-1)
            m_UnitType = (char)cmd.ResponseData[index++];
            switch (m_UnitType)
            {
                case (char)0x00: displayString += "Validator"; break;
                case (char)0x03: displayString += "SMART Hopper"; break;
                case (char)0x06: displayString += "SMART Payout"; break;
                case (char)0x07: displayString += "NV11"; break;
                default: displayString += "Unknown Type"; break;
            }

            displayString += "\r\nFirmware: ";

            // firmware (table 2-5)
            while (index <= 5)
            {
                displayString += (char)cmd.ResponseData[index++];
                if (index == 4)
                    displayString += ".";
            }

            // country code (table 6-8)
            // this is legacy code, in protocol version 6+ each channel has a seperate currency
            index = 9; // to skip country code

            // value multiplier (table 9-11) 
            // also legacy code, a real value multiplier appears later in the response
            index = 12; // to skip value multiplier

            displayString += "\r\nNumber of Channels: ";
            int numChannels = cmd.ResponseData[index++];
            m_NumberOfChannels = numChannels;

            displayString += numChannels + "\r\n";
            // channel values (table 13 to 13+n)
            // the channel values located here in the table are legacy, protocol 6+ provides a set of expanded
            // channel values.
            index = 13 + m_NumberOfChannels; // Skip channel values

            // channel security (table 13+n to 13+(n*2))
            // channel security values are also legacy code
            index = 13 + (m_NumberOfChannels * 2); // Skip channel security

            displayString += "Real Value Multiplier: ";

            // real value multiplier (table 13+(n*2) to 15+(n*2))
            // (big endian)
            m_ValueMultiplier = cmd.ResponseData[index + 2];
            m_ValueMultiplier += cmd.ResponseData[index + 1] << 8;
            m_ValueMultiplier += cmd.ResponseData[index] << 16;
            displayString += m_ValueMultiplier + "\r\nProtocol Version: ";
            index += 3;

            // protocol version (table 16+(n*2))
            index = 16 + (m_NumberOfChannels * 2);
            int protocol = cmd.ResponseData[index++];
            displayString += protocol + "\r\n";

            // protocol 6+ only

            // channel currency country code (table 17+(n*2) to 17+(n*5))
            index = 17 + (m_NumberOfChannels * 2);
            int sectionEnd = 17 + (m_NumberOfChannels * 5);
            int count = 0;
            byte[] channelCurrencyTemp = new byte[3 * m_NumberOfChannels];
            while (index < sectionEnd)
            {
                displayString += "Channel " + ((count / 3) + 1) + ", currency: ";
                channelCurrencyTemp[count] = cmd.ResponseData[index++];
                displayString += (char)channelCurrencyTemp[count++];
                channelCurrencyTemp[count] = cmd.ResponseData[index++];
                displayString += (char)channelCurrencyTemp[count++];
                channelCurrencyTemp[count] = cmd.ResponseData[index++];
                displayString += (char)channelCurrencyTemp[count++];
                displayString += "\r\n";
            }

            // expanded channel values (table 17+(n*5) to 17+(n*9))
            index = sectionEnd;
            displayString += "Expanded channel values:\r\n";
            sectionEnd = 17 + (m_NumberOfChannels * 9);
            int n = 0;
            count = 0;
            int[] channelValuesTemp = new int[m_NumberOfChannels];
            while (index < sectionEnd)
            {
                n = CHelpers.ConvertBytesToInt32(cmd.ResponseData, index);
                channelValuesTemp[count] = n;
                index += 4;
                displayString += "Channel " + ++count + ", value = " + n + "\r\n";
            }

            // Create list entry for each channel
            m_UnitDataList.Clear(); // clear old table
            for (byte i = 0; i < m_NumberOfChannels; i++)
            {
                ChannelData d = new ChannelData();
                d.Channel = i;
                d.Channel++;
                d.Value = channelValuesTemp[i] * m_ValueMultiplier;
                d.Currency[0] = (char)channelCurrencyTemp[0 + (i * 3)];
                d.Currency[1] = (char)channelCurrencyTemp[1 + (i * 3)];
                d.Currency[2] = (char)channelCurrencyTemp[2 + (i * 3)];
                d.Level = CheckNoteLevel(d.Value, d.Currency);
                bool b = false;
                IsNoteRecycling(d.Value, d.Currency, ref b);
                d.Recycling = b;

                m_UnitDataList.Add(d);
            }

            // Sort the list of data by the value.
            m_UnitDataList.Sort(delegate (ChannelData d1, ChannelData d2) { return d1.Value.CompareTo(d2.Value); });

        }

        public int CheckNoteLevel(int note, char[] currency)
        {
            cmd.CommandData[0] = CCommands.SSP_CMD_GET_NOTE_AMOUNT;
            byte[] b = CHelpers.ConvertIntToBytes(note);
            cmd.CommandData[1] = b[0];
            cmd.CommandData[2] = b[1];
            cmd.CommandData[3] = b[2];
            cmd.CommandData[4] = b[3];

            cmd.CommandData[5] = (byte)currency[0];
            cmd.CommandData[6] = (byte)currency[1];
            cmd.CommandData[7] = (byte)currency[2];
            cmd.CommandDataLength = 8;

            if (!ValidateCommand()) return 0;
            if (cmd.ResponseData[0] == CCommands.SSP_RESPONSE_CMD_OK)
            {
                int i = (int)cmd.ResponseData[1];
                return i;
            }
            return 0;
        }

        public void IsNoteRecycling(int noteValue, char[] currency, ref bool response)
        {
            // Determine if the note is currently being recycled
            cmd.CommandData[0] = CCommands.SSP_CMD_GET_ROUTING;
            byte[] b = CHelpers.ConvertIntToBytes(noteValue);
            cmd.CommandData[1] = b[0];
            cmd.CommandData[2] = b[1];
            cmd.CommandData[3] = b[2];
            cmd.CommandData[4] = b[3];

            // Add currency
            cmd.CommandData[5] = (byte)currency[0];
            cmd.CommandData[6] = (byte)currency[1];
            cmd.CommandData[7] = (byte)currency[2];
            cmd.CommandDataLength = 8;

            if (!ValidateCommand()) return;
            if (CheckGenericResponses())
            {
                // True if it is currently being recycled
                if (cmd.ResponseData[1] == 0x00)
                    response = true;
                // False if not
                else if (cmd.ResponseData[1] == 0x01)
                    response = false;
            }
        }

        private bool CheckGenericResponses()
        {
            if (cmd.ResponseData[0] == CCommands.SSP_RESPONSE_CMD_OK)
                return true;
            else
            {
                switch (cmd.ResponseData[0])
                {
                    case CCommands.SSP_RESPONSE_CMD_CANNOT_PROCESS:
                        if (cmd.ResponseData[1] == 0x03)
                        {
                            Console.WriteLine("Unit responded with a \"Busy\" response, command cannot be " +
                                "processed at this time\r\n");
                        }
                        else
                        {
                            Console.WriteLine("Command response is CANNOT PROCESS COMMAND, error code - 0x"
                            + BitConverter.ToString(cmd.ResponseData, 1, 1) + "\r\n");
                        }
                        return false;
                    case CCommands.SSP_RESPONSE_CMD_FAIL:
                        Console.WriteLine("Command response is FAIL\r\n");
                        return false;
                    case CCommands.SSP_RESPONSE_CMD_KEY_NOT_SET:
                        Console.WriteLine("Command response is KEY NOT SET, renegotiate keys\r\n");
                        return false;
                    case CCommands.SSP_RESPONSE_CMD_PARAM_OUT_OF_RANGE:
                        Console.WriteLine("Command response is PARAM OUT OF RANGE\r\n");
                        return false;
                    case CCommands.SSP_RESPONSE_CMD_SOFTWARE_ERROR:
                        Console.WriteLine("Command response is SOFTWARE ERROR\r\n");
                        return false;
                    case CCommands.SSP_RESPONSE_CMD_UNKNOWN:
                        Console.WriteLine("Command response is UNKNOWN\r\n");
                        return false;
                    case CCommands.SSP_RESPONSE_CMD_WRONG_PARAMS:
                        Console.WriteLine("Command response is WRONG PARAMETERS\r\n");
                        return false;
                    default:
                        return false;
                }
            }
        }

        public void SetInhibits()
        {
            // set inhibits
            cmd.CommandData[0] = CCommands.SSP_CMD_SET_INHIBITS;
            cmd.CommandData[1] = 0xFF;
            cmd.CommandData[2] = 0xFF;
            cmd.CommandDataLength = 3;

            if (!ValidateCommand()) return;
            if (CheckGenericResponses())
                Console.WriteLine("Payout enabled\r\n");
        }

        public void EnableValidator()
        {
            cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE;
            cmd.CommandDataLength = 1;

            if (!ValidateCommand()) return;
            if (CheckGenericResponses())
                Console.WriteLine("Payout enabled\r\n");
            // check response
        }

        public void EnablePayout()
        {
            cmd.CommandData[0] = CCommands.SSP_CMD_ENABLE_PAYOUT;
            cmd.CommandDataLength = 1;

            if (!ValidateCommand()) return;
            if (CheckGenericResponses())
                Console.WriteLine("Payout enabled\r\n");
        }

        // The poll function is called repeatedly to poll to validator for information, it returns as
        // a response in the command structure what events are currently happening.
        public bool DoPoll()
        {
            byte i;

            //send poll
            cmd.CommandData[0] = CCommands.SSP_CMD_POLL;
            cmd.CommandDataLength = 1;
            if (!ValidateCommand())
            {
                statusPayout = "Error";
                return false;
            }

            // store response locally so data can't get corrupted by other use of the cmd variable
            byte[] response = new byte[255];
            cmd.ResponseData.CopyTo(response, 0);
            byte responseLength = cmd.ResponseDataLength;

            //parse poll response
            ChannelData data = new ChannelData();
            for (i = 1; i < responseLength; ++i)
            {
                switch (response[i])
                {
                    // This response indicates that the unit was reset and this is the first time a poll
                    // has been called since the reset.
                    case CCommands.SSP_POLL_RESET:
                        Console.WriteLine("Unit reset\r\n");
                        Console.WriteLine("SSP_POLL_RESET");
                        UpdateData();
                        break;
                    // This response indicates the unit is disabled.
                    case CCommands.SSP_POLL_DISABLED:
                        Console.WriteLine("Unit disabled...\r\n");
                        Close();
                        IsStart();
                        break;
                    // A note is currently being read by the validator sensors. The second byte of this response
                    // is zero until the note's type has been determined, it then changes to the channel of the 
                    // scanned note.
                    case CCommands.SSP_POLL_NOTE_READ:
                        statusPayout = "LEYENDO";
                        if (cmd.ResponseData[i + 1] > 0)
                        {
                            GetDataByChannel(response[i + 1], ref data);
                            Console.WriteLine("Note in escrow, amount: " + CHelpers.FormatToCurrency(data.Value) + "\r\n");
                        }
                        else
                        {
                            Console.WriteLine("Reading note\r\n");
                        }
                        i++;
                        break;
                    // A credit event has been detected, this is when the validator has accepted a note as legal currency.
                    case CCommands.SSP_POLL_CREDIT:
                        GetDataByChannel(response[i + 1], ref data);
                        //var value = decimal.Parse(CHelpers.FormatToCurrency(data.Value).Replace(".00", ",00"));
                        Console.WriteLine("credit jujuju");
                        statusPayout = "LEYENDO";
                        // NotesAccepted 
                        m_TotalNotesAccepted++;
                        UpdateData();
                        Console.WriteLine("SSP_POLL_CREDIT");
                        i++;
                        ProccesPayment(data.Value.ToString(), 1);
                        break;
                    // A note is being rejected from the validator. This will carry on polling while the note is in transit.
                    case CCommands.SSP_POLL_REJECTING:
                        Console.WriteLine("Rejecting note\r\n");
                        break;
                    // A note has been rejected from the validator, the note will be resting in the bezel. This response only
                    // appears once.
                    case CCommands.SSP_POLL_REJECTED:
                        Console.WriteLine("Note rejected\r\n");
                        QueryRejection();
                        break;
                    // A note is in transit to the cashbox.
                    case CCommands.SSP_POLL_STACKING:
                        Console.WriteLine("Stacking note\r\n");
                        statusPayout = "LEYENDO";
                        break;
                    // The payout device is 'floating' a specified amount of notes. It will transfer some to the cashbox and
                    // leave the specified amount in the payout device.
                    case CCommands.SSP_POLL_FLOATING:
                        Console.WriteLine("Floating notes\r\n");
                        // Now the index needs to be moved on to skip over the data provided by this response so it
                        // is not parsed as a normal poll response.
                        // In this response, the data includes the number of countries being floated (1 byte), then a 4 byte value
                        // and 3 byte currency code for each country. 
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // A note has reached the cashbox.
                    case CCommands.SSP_POLL_STACKED:
                        Console.WriteLine("Note stacked\r\n");
                        statusPayout = "ALMACENADO";
                        break;
                    // The float operation has been completed.
                    case CCommands.SSP_POLL_FLOATED:
                        Console.WriteLine("Completed floating\r\n");
                        Console.WriteLine("SSP_POLL_FLOATED");
                        UpdateData();
                        EnableValidator();
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // A note has been stored in the payout device to be paid out instead of going into the cashbox.
                    case CCommands.SSP_POLL_NOTE_STORED:
                        Console.WriteLine("Note stored\r\n");
                        Console.WriteLine("SSP_POLL_NOTE_STORED");
                        statusPayout = "BODEGA";
                        UpdateData();
                        break;
                    // A safe jam has been detected. This is where the user has inserted a note and the note
                    // is jammed somewhere that the user cannot reach.
                    case CCommands.SSP_POLL_SAFE_JAM:
                        Console.WriteLine("Safe jam\r\n");
                        break;
                    // An unsafe jam has been detected. This is where a user has inserted a note and the note
                    // is jammed somewhere that the user can potentially recover the note from.
                    case CCommands.SSP_POLL_UNSAFE_JAM:
                        Console.WriteLine("Unsafe jam\r\n");
                        break;
                    // An error has been detected by the payout unit.
                    case CCommands.SSP_POLL_PAYOUT_ERROR: // Note: Will be reported only when Protocol version >= 7
                        Console.WriteLine("Detected error with payout device\r\n");
                        i += (byte)((response[i + 1] * 7) + 2);
                        break;
                    // A fraud attempt has been detected. 
                    case CCommands.SSP_POLL_FRAUD_ATTEMPT:
                        Console.WriteLine("Fraud attempt\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The stacker (cashbox) is full.
                    case CCommands.SSP_POLL_STACKER_FULL:
                        Console.WriteLine("Stacker full\r\n");
                        break;
                    // A note was detected somewhere inside the validator on startup and was rejected from the front of the
                    // unit.
                    case CCommands.SSP_POLL_NOTE_CLEARED_FROM_FRONT:
                        Console.WriteLine("Note cleared from front of validator\r\n");
                        i++;
                        break;
                    // A note was detected somewhere inside the validator on startup and was cleared into the cashbox.
                    case CCommands.SSP_POLL_NOTE_CLEARED_TO_CASHBOX:
                        Console.WriteLine("Note cleared to cashbox\r\n");
                        i++;
                        break;
                    // A note has been detected in the validator on startup and moved to the payout device 
                    case CCommands.SSP_POLL_NOTE_PAID_INTO_STORE_ON_POWERUP:
                        Console.WriteLine("Note paid into payout device on startup\r\n");
                        i += 7;
                        break;
                    // A note has been detected in the validator on startup and moved to the cashbox
                    case CCommands.SSP_POLL_NOTE_PAID_INTO_STACKER_ON_POWERUP:
                        Console.WriteLine("Note paid into cashbox on startup\r\n");
                        i += 7;
                        break;
                    // The cashbox has been removed from the unit. This will continue to poll until the cashbox is replaced.
                    case CCommands.SSP_POLL_CASHBOX_REMOVED:
                        Console.WriteLine("Cashbox removed\r\n");
                        break;
                    // The cashbox has been replaced, this will only display on a poll once.
                    case CCommands.SSP_POLL_CASHBOX_REPLACED:
                        Console.WriteLine("Cashbox replaced\r\n");
                        break;
                    // The validator is in the process of paying out a note, this will continue to poll until the note has 
                    // been fully dispensed and removed from the front of the validator by the user.
                    case CCommands.SSP_POLL_NOTE_DISPENSING:
                        Console.WriteLine("Dispensing note(s)\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The note has been dispensed and removed from the bezel by the user.
                    case CCommands.SSP_POLL_NOTE_DISPENSED:
                        Console.WriteLine("Dispensed note(s)\r\n");
                        Console.WriteLine("SSP_POLL_NOTE_DISPENSED");
                        UpdateData();
                        EnableValidator();
                        i += (byte)((response[i + 1] * 7) + 1);
                        ProccesPayment("", 2);
                        break;
                    // The payout device is in the process of emptying all its stored notes to the cashbox. This
                    // will continue to poll until the device is empty.
                    case CCommands.SSP_POLL_EMPTYING:
                        Console.WriteLine("Emptying\r\n");
                        break;
                    // This single poll response indicates that the payout device has finished emptying.
                    case CCommands.SSP_POLL_EMPTIED:
                        Console.WriteLine("Emptied\r\n");
                        Console.WriteLine("SSP_POLL_EMPTIED");
                        UpdateData();
                        EnableValidator();
                        break;
                    // The payout device is in the process of SMART emptying all its stored notes to the cashbox, keeping
                    // a count of the notes emptied. This will continue to poll until the device is empty.
                    case CCommands.SSP_POLL_SMART_EMPTYING:
                        Console.WriteLine("SMART Emptying...\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The payout device has finished SMART emptying, the information of what was emptied can now be displayed
                    // using the CASHBOX PAYOUT OPERATION DATA command.
                    case CCommands.SSP_POLL_SMART_EMPTIED:
                        Console.WriteLine("SMART Emptied, getting info...\r\n");
                        Console.WriteLine("SSP_POLL_SMART_EMPTIED");
                        UpdateData();
                        GetCashboxPayoutOpData();
                        EnableValidator();
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // The payout device has encountered a jam. This will not clear until the jam has been removed and the unit
                    // reset.
                    case CCommands.SSP_POLL_JAMMED:
                        Console.WriteLine("Unit jammed...\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // This is reported when the payout has been halted by a host command. This will report the value of
                    // currency dispensed upto the point it was halted. 
                    case CCommands.SSP_POLL_HALTED:
                        Console.WriteLine("Halted...\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    // This is reported when the payout was powered down during a payout operation. It reports the original amount
                    // requested and the amount paid out up to this point for each currency.
                    case CCommands.SSP_POLL_INCOMPLETE_PAYOUT:
                        Console.WriteLine("Incomplete payout\r\n");
                        i += (byte)((response[i + 1] * 11) + 1);
                        break;
                    // This is reported when the payout was powered down during a float operation. It reports the original amount
                    // requested and the amount paid out up to this point for each currency.
                    case CCommands.SSP_POLL_INCOMPLETE_FLOAT:
                        Console.WriteLine("Incomplete float\r\n");
                        i += (byte)((response[i + 1] * 11) + 1);
                        break;
                    // A note has been transferred from the payout unit to the stacker.
                    case CCommands.SSP_POLL_NOTE_TRANSFERRED_TO_STACKER:
                        Console.WriteLine("Note transferred to stacker\r\n");
                        i += 7;
                        break;
                    // A note is resting in the bezel waiting to be removed by the user.
                    case CCommands.SSP_POLL_NOTE_HELD_IN_BEZEL:
                        Console.WriteLine("Note in bezel...\r\n");
                        i += 7;
                        break;
                    // The payout has gone out of service, the host can attempt to re-enable the payout by sending the enable payout
                    // command.
                    case CCommands.SSP_POLL_PAYOUT_OUT_OF_SERVICE:
                        Console.WriteLine("Payout out of service...\r\n");
                        break;
                    // The unit has timed out while searching for a note to payout. It reports the value dispensed before the timeout
                    // event.
                    case CCommands.SSP_POLL_TIMEOUT:
                        Console.WriteLine("Timed out searching for a note\r\n");
                        i += (byte)((response[i + 1] * 7) + 1);
                        break;
                    default:
                        Console.WriteLine("Unsupported poll response received: " + (int)cmd.ResponseData[i] + "\r\n");
                        break;
                }
            }
            return true;
        }

        public void UpdateData()
        {
            foreach (ChannelData d in m_UnitDataList)
            {
                d.Level = CheckNoteLevel(d.Value, d.Currency);
                IsNoteRecycling(d.Value, d.Currency, ref d.Recycling);
            }
        }

        // This allows the caller to access all the data stored with a channel. An empty ChannelData struct is passed
        // over which gets filled with info.
        public void GetDataByChannel(int channel, ref ChannelData d)
        {
            // Iterate through each 
            foreach (ChannelData dList in m_UnitDataList)
            {
                if (dList.Channel == channel) // Compare channel
                {
                    d = dList; // Copy data from list to param
                    break;
                }
            }
        }

        // This function sends the command LAST REJECT CODE which gives info about why a note has been rejected. It then
        // outputs the info to a passed across textbox.
        public void QueryRejection()
        {
            cmd.CommandData[0] = CCommands.SSP_CMD_LAST_REJECT_CODE;
            cmd.CommandDataLength = 1;
            if (!ValidateCommand()) return;

            if (CheckGenericResponses())
            {
                switch (cmd.ResponseData[1])
                {
                    case 0x00: Console.WriteLine("Note accepted\r\n"); break;
                    case 0x01: Console.WriteLine("Note length incorrect\r\n"); break;
                    case 0x02: Console.WriteLine("Invalid note\r\n"); break;
                    case 0x03: Console.WriteLine("Invalid note\r\n"); break;
                    case 0x04: Console.WriteLine("Invalid note\r\n"); break;
                    case 0x05: Console.WriteLine("Invalid note\r\n"); break;
                    case 0x06: Console.WriteLine("Channel inhibited\r\n"); break;
                    case 0x07: Console.WriteLine("Second note inserted during read\r\n"); break;
                    case 0x08: Console.WriteLine("Host rejected note\r\n"); break;
                    case 0x09: Console.WriteLine("Invalid note\r\n"); break;
                    case 0x0A: Console.WriteLine("Invalid note read\r\n"); break;
                    case 0x0B: Console.WriteLine("Note too long\r\n"); break;
                    case 0x0C: Console.WriteLine("Validator disabled\r\n"); break;
                    case 0x0D: Console.WriteLine("Mechanism slow/stalled\r\n"); break;
                    case 0x0E: Console.WriteLine("Strim attempt\r\n"); break;
                    case 0x0F: Console.WriteLine("Fraud channel reject\r\n"); break;
                    case 0x10: Console.WriteLine("No notes inserted\r\n"); break;
                    case 0x11: Console.WriteLine("Invalid note read\r\n"); break;
                    case 0x12: Console.WriteLine("Twisted note detected\r\n"); break;
                    case 0x13: Console.WriteLine("Escrow time-out\r\n"); break;
                    case 0x14: Console.WriteLine("Bar code scan fail\r\n"); break;
                    case 0x15: Console.WriteLine("Invalid note read\r\n"); break;
                    case 0x16: Console.WriteLine("Invalid note read\r\n"); break;
                    case 0x17: Console.WriteLine("Invalid note read\r\n"); break;
                    case 0x18: Console.WriteLine("Invalid note read\r\n"); break;
                    case 0x19: Console.WriteLine("Incorrect note width\r\n"); break;
                    case 0x1A: Console.WriteLine("Note too short\r\n"); break;
                }
            }
        }

        // This function can be called after SMART events such as SMART empty. It will return a report
        // of what was moved to the cashbox. It returns a formatted string. The command can be used for
        // more useful things such as detecting what has been paid into the cashbox in the case of a payout
        // error.
        public string GetCashboxPayoutOpData()
        {
            // first send the command
            cmd.CommandData[0] = CCommands.SSP_CMD_CASHBOX_PAYOUT_OP_DATA;
            cmd.CommandDataLength = 1;

            if (!ValidateCommand()) return "";

            // now deal with the response data
            if (CheckGenericResponses())
            {
                // number of different notes
                int n = cmd.ResponseData[1];
                string displayString = "Total Number of Notes: " + n.ToString() + "\r\n\r\n";
                int i = 0;
                for (i = 2; i < (9 * n); i += 9)
                {
                    displayString += "Moved " + CHelpers.ConvertBytesToInt16(cmd.ResponseData, i) +
                        " x " + CHelpers.FormatToCurrency(CHelpers.ConvertBytesToInt32(cmd.ResponseData, i + 2)) +
                        " " + (char)cmd.ResponseData[i + 6] + (char)cmd.ResponseData[i + 7] + (char)cmd.ResponseData[i + 8] + " to cashbox\r\n";
                }
                displayString += CHelpers.ConvertBytesToInt32(cmd.ResponseData, i) + " notes not recognised\r\n";

                return displayString;
            }
            return "";
        }

        public decimal PayOut(decimal amountReturn)
        {
            bool payoutRequired = false;
            decimal substracValue = amountReturn;
            Thread.Sleep(1000);
            if (amountReturn > 0)
            {
                byte[] data = new byte[9 * m_NumberOfChannels]; // create to size of maximum possible
                byte length = 0;
                int dataIndex = 0;
                byte denomsToPayout = 0;


                // For each denomination
                for (int i = m_NumberOfChannels - 1; i >= 0; i--)
                {
                    try
                    {
                        ChannelData d = m_UnitDataList[i];
                        decimal realValue = RealValue(d.Value);
                        if (substracValue >= realValue && d.Level > 0)
                        {

                            denomsToPayout++;
                            length += 9; // 9 bytes per denom to payout (2 amount, 4 value, 3 currency)
                            payoutRequired = true; // need to do a payout as there is now > 0 denoms
                            decimal amountToPayout = Math.Floor(Convert.ToDecimal(substracValue / realValue));

                            if (amountToPayout > d.Level)
                            {
                                amountToPayout = d.Level;
                            }

                            substracValue -= (realValue * amountToPayout);


                            // Number of this denomination to payout
                            UInt16 numToPayout = UInt16.Parse(amountToPayout.ToString());
                            byte[] b = CHelpers.ConvertIntToBytes(numToPayout);
                            data[dataIndex++] = b[0];
                            data[dataIndex++] = b[1];

                            // Value of this denomination
                            b = CHelpers.ConvertIntToBytes(d.Value);
                            data[dataIndex++] = b[0];
                            data[dataIndex++] = b[1];
                            data[dataIndex++] = b[2];
                            data[dataIndex++] = b[3];

                            // Currency of this denomination
                            data[dataIndex++] = (Byte)d.Currency[0];
                            data[dataIndex++] = (Byte)d.Currency[1];
                            data[dataIndex++] = (Byte)d.Currency[2];
                        }
                        // If textbox isn't blank then this denom is being paid out
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        payoutRequired = false; // don't payout on exception
                    }
                }

                if (payoutRequired)
                {
                    // Send payout command and shut this form
                    PayoutByDenomination(denomsToPayout, data, length);
                }


            }
            return substracValue;
        }

        // Payout by denomination. This function allows a developer to payout specified amounts of certain
        // notes. Due to the variable length of the data that could be passed to the function, the user 
        // passes an array containing the data to payout and the length of that array along with the number
        // of denominations they are paying out.
        public void PayoutByDenomination(byte numDenoms, byte[] data, byte dataLength)
        {
            // First is the command byte
            cmd.CommandData[0] = CCommands.SSP_CMD_PAYOUT_BY_DENOMINATION;

            // Next is the number of denominations to be paid out
            cmd.CommandData[1] = numDenoms;

            // Copy over data byte array parameter into command structure
            int currentIndex = 2;
            for (int i = 0; i < dataLength; i++)
                cmd.CommandData[currentIndex++] = data[i];

            // Perform a real payout (0x19 for test)
            cmd.CommandData[currentIndex++] = 0x58;

            // Length of command data (add 3 to cover the command byte, num of denoms and real/test byte)
            dataLength += 3;
            cmd.CommandDataLength = dataLength;

            if (!ValidateCommand()) return;
            if (CheckGenericResponses())
            {
                Console.WriteLine("PayoutByDenomination");
            }
        }

        private void ProccesPayment(string value, int type)
        {
            try
            {
                Thread.Sleep(1000);
                if (type == 2)
                {
                    callback?.Invoke(type);
                }
                else
                {
                    decimal realValue = RealValue(Convert.ToDecimal(value));
                    //this.intoValue = this.intoValue + realValue;
                    callbackValue?.Invoke(realValue);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private decimal RealValue(decimal value)
        {
            try
            {
                return decimal.Parse(value.ToString().Substring(0, value.ToString().Length - 2));
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public string GetStatus()
        {
            try
            {
                return statusPayout;
            }
            catch (Exception ex)
            {
                return "Error";
            }
        }

        public List<Bills> GetBills()
        {
            try
            {
                if (m_UnitDataList.Count > 0)
                {
                    List<Bills> listBills = new List<Bills>();
                    foreach (var item in m_UnitDataList)
                    {
                        listBills.Add(new Bills
                        {
                            Denomination = RealValue(item.Value).ToString(),
                            Quantity = item.Level.ToString(),
                            Total = (RealValue(item.Value) * item.Level).ToString()
                        });
                    }
                    return listBills;
                }

            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }

    public class Global
    {
        public static string ComPort = Utilities.GetConfiguration("Port");
        public static byte SSPAddress = 0;
    }

    public class Bills
    {
        public string Denomination { get; set; }
        public string Quantity { get; set; }
        public string Total { get; set; }
    }
}
