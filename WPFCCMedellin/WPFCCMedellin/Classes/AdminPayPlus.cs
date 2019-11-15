using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using WPFCCMedellin.Classes.Printer;
using WPFCCMedellin.DataModel;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services;
using WPFCCMedellin.Services.Object;

namespace WPFCCMedellin.Classes
{
    public class AdminPayPlus
    {
        #region "Referencias"

        private static Api api;

        public Action<bool> callbackResult;//Calback de mensaje

        private static CONFIGURATION_PAYDAD _dataConfiguration;

        public static CONFIGURATION_PAYDAD DataConfiguration
        {
            get { return _dataConfiguration; }
        }

        private static DataPayPlus _dataPayPlus;

        public static DataPayPlus DataPayPlus
        {
            get { return _dataPayPlus; }
        }

        private static PrintService _printService;

        public static PrintService PrintService
        {
            get { return _printService; }
        }

        private static PrinterFile _printerFile;

        public static PrinterFile PrinterFile
        {
            get { return _printerFile; }
        }

        private static ControlPeripherals _controlPeripherals;

        public static ControlPeripherals ControlPeripherals
        {
            get { return _controlPeripherals; }
        }

        private static ApiIntegration _apiIntegration;

        public static ApiIntegration ApiIntegration
        {
            get { return _apiIntegration; }
        }

        #endregion

        #region "Constructor"

        public AdminPayPlus()
        {
            if (api == null)
            {
                api = new Api();
            }

            if (_apiIntegration == null)
            {
                _apiIntegration = new ApiIntegration();
            }

            if (_printService == null)
            {
                _printService = new PrintService();
            }

            if (_dataPayPlus == null)
            {
                _dataPayPlus = new DataPayPlus();
            }

            if (_printerFile == null)
            {
                _printerFile = new PrinterFile(Utilities.GetConfiguration("PrinterName").Trim(), true);
            }
        }
        #endregion

        public async void Start()
        {
            if (await LoginPaypad())
            {
                if (await ValidatePaypad())
                {
                    ValidatePeripherals();
                }
                else
                {
                    callbackResult?.Invoke(false);
                }
            }
            else
            {
                callbackResult?.Invoke(false);
            }
        }

        private async Task<bool> LoginPaypad()
        {
            try
            {
                var config = LoadInformation();

                if (config != null)
                {
                    var result = await api.GetSecurityToken(config);

                    if (result != null)
                    {
                        config.ID_PAYPAD = Convert.ToInt32(result.User);
                        config.ID_SESSION = Convert.ToInt32(result.Session);
                        config.TOKEN_API = result.Token;

                        if (DBManagment.UpdateConfiguration(config))
                        {
                            _dataConfiguration = config;
                            return true;
                        }
                    }
                    else
                    {
                        SaveErrorControl(MessageResource.ErrorServiceLogin, MessageResource.NoGoInitial, EError.Api, ELevelError.Strong);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
            return false;
        }

        public static async Task<bool> ValidatePaypad()
        {
            try
            {
                var response = await api.CallApi("InitPaypad");
                if (response != null)
                {
                    _dataPayPlus = JsonConvert.DeserializeObject<DataPayPlus>(response.ToString());

                    //Utilities.ImagesSlider = JsonConvert.DeserializeObject<List<string>>(data.ListImages.ToString());
                    if (_dataPayPlus.StateBalanece || _dataPayPlus.StateUpload)
                    {
                        SaveLog(new RequestLog
                        {
                            Reference = response.ToString(),
                            Description = MessageResource.PaypadGoAdmin
                        }, ELogType.General);
                        return true;
                    }
                    if (_dataPayPlus.State && _dataPayPlus.StateAceptance && _dataPayPlus.StateDispenser)
                    {
                        SaveLog(new RequestLog
                        {
                            Reference = response.ToString(),
                            Description = MessageResource.PaypadStarSusses
                        }, ELogType.General);
                        return true;
                    }
                    else
                    {
                        SaveLog(new RequestLog
                        {
                            Reference = response.ToString(),
                            Description = MessageResource.NoGoInitial + _dataPayPlus.Message
                        }, ELogType.General);

                        SaveErrorControl(MessageResource.NoGoInitial, _dataPayPlus.Message, EError.Aplication, ELevelError.Strong);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
            return false;
        }

        private void ValidatePeripherals()
        {
            try
            {
                if (_controlPeripherals == null)
                {
                    _controlPeripherals = new ControlPeripherals(Utilities.GetConfiguration("PortBills"),
                        Utilities.GetConfiguration("PortCoins"), Utilities.GetConfiguration("ValuesDispenser"));
                }

                _controlPeripherals.callbackError = error =>
                {
                    SaveLog(new RequestLogDevice
                    {
                        Code = "",
                        Date = DateTime.Now,
                        Description = error.Item2,
                        Level = ELevelError.Strong
                    }, ELogType.Device);
                    Finish(false);
                };

                _controlPeripherals.callbackToken = isSucces =>
                {
                    _controlPeripherals.callbackError = null;
                    Finish(isSucces);
                };
                _controlPeripherals.Start();

            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
                callbackResult?.Invoke(false);
            }
        }

        private void Finish(bool isSucces)
        {
            _controlPeripherals.callbackToken = null;
            callbackResult?.Invoke(isSucces);
        }

        private CONFIGURATION_PAYDAD LoadInformation()
        {
            try
            {
                string[] keys = ReadKeys();

                return new CONFIGURATION_PAYDAD
                {
                    USER_API = Encryptor.Decrypt(keys[0]),
                    PASSWORD_API = Encryptor.Decrypt(keys[1]),
                    USER = Encryptor.Decrypt(keys[2]),
                    PASSWORD = Encryptor.Decrypt(keys[3]),
                    TYPE = Convert.ToInt32(keys[4])
                };
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
            return null;
        }

        private string[] ReadKeys()
        {
            try
            {
                string[] keys = new string[5];
                string[] text = File.ReadAllLines(@".\keys.txt");

                if (text.Length > 0)
                {
                    string[] line1 = text[0].Split(';');
                    string[] line2 = text[1].Split(';');

                    keys[0] = (line1[0].Split(':'))[1];
                    keys[1] = line1[1].Split(':')[1];
                    keys[2] = line2[0].Split(':')[1];
                    keys[3] = line2[1].Split(':')[1];
                    keys[4] = line2[2].Split(':')[1];
                }
                return keys;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
            return null;
        }

        public async static void SaveLog(object log, ELogType type)
        {
            try
            {
                Task.Run(async () =>
                {
                    var saveResult = DBManagment.SaveLog(log, type);
                    object result = "false";

                    if (log != null && saveResult)
                    {
                        if (type == ELogType.General)
                        {
                            result = await api.CallApi("SaveLog", (RequestLog)log);
                        }
                        else if (type == ELogType.Error)
                        {
                            result = await api.CallApi("SaveLogError", (ERROR_LOG)log);
                        }
                        else
                        {
                            var error = (RequestLogDevice)log;
                            result = await api.CallApi("SaveLogDevice", error);
                            SaveErrorControl(error.Description, "", EError.Device, error.Level);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
        }

        public static void SaveErrorControl(string desciption, string observation, EError error, ELevelError level, int device = 0)
        {
            try
            {
                Task.Run(() =>
                {
                    if (_dataConfiguration != null)
                    {
                        var idPaypad = _dataConfiguration.ID_PAYPAD;
                        if (idPaypad == null)
                        {
                            idPaypad = int.Parse(Utilities.GetConfiguration("idPaypad"));
                        }

                        if (desciption.Contains("FATAL"))
                        {
                            level = ELevelError.Strong;
                        }

                        PAYPAD_CONSOLE_ERROR consoleError = new PAYPAD_CONSOLE_ERROR
                        {
                            PAYPAD_ID = (int)idPaypad,
                            DATE = DateTime.Now,
                            STATE = 0,
                            DESCRIPTION = desciption,
                            OBSERVATION = observation,
                            ERROR_ID = (int)error,
                            ERROR_LEVEL_ID = (int)level
                        };

                        DBManagment.InsetConsoleError(consoleError);
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
        }

        public static async Task SaveTransactions(Transaction transaction, bool getConsecutive)
        {
            try
            {
                if (transaction != null)
                {
                 //   transaction.IsReturn = await ValidateMoney(transaction);

                    if (getConsecutive)
                    {
                        transaction.consecutive = (await GetConsecutive()).ToString();
                    }

                    if ((getConsecutive && int.Parse(transaction.consecutive) > 0) || !getConsecutive)
                    {
                        if (transaction.payer == null)
                        {
                            transaction.payer.IDENTIFICATION = _dataConfiguration.ID_PAYPAD.ToString();
                            transaction.payer.NAME = Utilities.GetConfiguration("NAME_PAYPAD");
                            transaction.payer.LAST_NAME = Utilities.GetConfiguration("LAST_NAME_PAYPAD");
                        }

                        transaction.payer.STATE = true;

                        var resultPayer = await api.CallApi("SavePayer", transaction.payer);

                        if (resultPayer != null)
                        {
                            transaction.payer.PAYER_ID = JsonConvert.DeserializeObject<int>(resultPayer.ToString());
                        }

                        if (transaction.payer.PAYER_ID > 0)
                        {
                            var data = new TRANSACTION
                            {
                                TYPE_TRANSACTION_ID = Convert.ToInt32(transaction.Type),
                                PAYER_ID = transaction.payer.PAYER_ID,
                                STATE_TRANSACTION_ID = Convert.ToInt32(transaction.State),
                                TOTAL_AMOUNT = transaction.Amount,
                                DATE_END = DateTime.Now,
                                TRANSACTION_ID = 0,
                                RETURN_AMOUNT = 0,
                                INCOME_AMOUNT = 0,
                                PAYPAD_ID = 0,
                                DATE_BEGIN = DateTime.Now,
                                STATE_NOTIFICATION = 0,
                                STATE = false,
                                DESCRIPTION = "Transaccion iniciada"
                            };

                            var details = new TRANSACTION_DESCRIPTION
                            {
                                AMOUNT = transaction.Amount,
                                TRANSACTION_ID = data.ID,
                                REFERENCE = transaction.reference,
                                OBSERVATION = transaction.Enrollment.ToString(),
                                TRANSACTION_DESCRIPTION_ID = 0,
                                STATE = true
                            };

                            data.TRANSACTION_DESCRIPTION.Add(details);

                            if (data != null)
                            {
                                var responseTransaction = await api.CallApi("SaveTransaction", data);
                                if (responseTransaction != null)
                                {
                                    transaction.IdTransactionAPi = JsonConvert.DeserializeObject<int>(responseTransaction.ToString());

                                    if (transaction.IdTransactionAPi > 0)
                                    {
                                        data.TRANSACTION_ID = transaction.IdTransactionAPi;
                                        data.STATE = true;
                                    }
                                }
                                else
                                {
                                    SaveLog(new RequestLog
                                    {
                                        Reference = transaction.reference,
                                        Description = string.Concat(MessageResource.NoInsertTransaction, " en su primer intente ")
                                    }, ELogType.General);

                                    responseTransaction = await api.CallApi("SaveTransaction", data);
                                    if (responseTransaction != null)
                                    {
                                        transaction.IdTransactionAPi = JsonConvert.DeserializeObject<int>(responseTransaction.ToString());

                                        if (transaction.IdTransactionAPi > 0)
                                        {
                                            data.TRANSACTION_ID = transaction.IdTransactionAPi;
                                            data.STATE = true;
                                        }
                                    }
                                    else
                                    {
                                        SaveLog(new RequestLog
                                        {
                                            Reference = transaction.reference,
                                            Description = string.Concat(MessageResource.NoInsertTransaction, " en su segundo intente ")
                                        }, ELogType.General);
                                    }
                                }

                                transaction.TransactionId = DBManagment.SaveTransaction(data);
                            }
                        }
                        else
                        {
                            SaveLog(new RequestLog
                            {
                                Reference = transaction.reference,
                                Description = MessageResource.NoInsertPayment + transaction.payer.IDENTIFICATION
                            }, ELogType.General);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
        }

        public async static void SaveDetailsTransaction(int idTransactionAPi, decimal enterValue, int opt, int quantity, string code, string description)
        {
            try
            {
                var details = new RequestTransactionDetails
                {
                    Code = code,
                    Denomination = Convert.ToInt32(enterValue),
                    Operation = opt,
                    Quantity = quantity,
                    TransactionId = idTransactionAPi,
                    Description = description
                };

                var response = await api.CallApi("SaveTransactionDetail", details);

                if (response != null)
                {
                    DBManagment.SaveTransactionDetail(details, true);
                }
                else
                {
                    DBManagment.SaveTransactionDetail(details, false);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
        }

        public async static void UpdateTransaction(Transaction transaction, bool validatePaypad = false)
        {
            try
            {
                if (transaction != null)
                {
                    TRANSACTION tRANSACTION = DBManagment.UpdateTransaction(transaction);

                    if (tRANSACTION != null)
                    {

                        var responseTransaction = await api.CallApi("UpdateTransaction", tRANSACTION);
                        if (responseTransaction != null)
                        {
                            tRANSACTION.STATE = true;
                            DBManagment.UpdateTransactionState(tRANSACTION, 2);
                        }
                    }
                }
                if (validatePaypad)
                {
                    ValidatePaypad();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
        }

        public async static void UpdateTransaction(TRANSACTION tRANSACTION)
        {
            try
            {
                if (tRANSACTION != null)
                {
                    tRANSACTION.TRANSACTION_DESCRIPTION = null;

                    var responseTransaction = await api.CallApi("UpdateTransaction", tRANSACTION);
                    if (responseTransaction != null)
                    {
                        tRANSACTION.STATE = true;
                        DBManagment.UpdateTransactionState(tRANSACTION, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
        }

        public static async Task<bool> UpdateBalance(PaypadOperationControl paypadData)
        {
            try
            {
                string action = "";

                if (_dataPayPlus.StateBalanece)
                {
                    action = "UpdateBalance";
                }
                else
                {
                    action = "UpdateUpload";
                }

                var response = await api.CallApi(action, paypadData);

                if (response != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
                return false;
            }
        }

        public static async Task<bool> ValidateUser(string name, string pass)
        {
            try
            {
                var response = await api.CallApi("ValidateUserPayPad", new RequestAuth
                {
                    UserName = name,
                    Password = pass
                });

                if (response != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
                return false;
            }
        }

        public static async Task<PaypadOperationControl> DataListPaypad(ETypeAdministrator typeAdministrator)
        {
            try
            {
                string action = "";

                if (typeAdministrator == ETypeAdministrator.Balancing)
                {
                    action = "GetBalance";
                }
                else
                {
                    action = "GetUpload";
                }

                var response = await api.CallApi(action);

                if (response != null)
                {
                    var operationControl = JsonConvert.DeserializeObject<PaypadOperationControl>(response.ToString());

                    return operationControl;
                }

                return null;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
                return null;
            }
        }

        public static void NotificateInformation()
        {
            try
            {
                Task.Run(async () =>
                {
                    var transactions = DBManagment.GetTransactionNotific();
                    if (transactions.Count > 0)
                    {
                        foreach (var transaction in transactions)
                        {
                            var responseTransaction = await api.CallApi("UpdateTransaction", transaction);
                            if (responseTransaction != null)
                            {
                                transaction.STATE = true;
                                DBManagment.UpdateTransactionState(transaction, 2);
                            }
                        }

                        var detailTeansactions = DBManagment.GetDetailsTransaction();
                        foreach (var detail in detailTeansactions)
                        {
                            var response = await api.CallApi("SaveTransactionDetail", detail);
                            if (response != null)
                            {
                                detail.STATE = true;
                                DBManagment.UpdateTransactionDetailState(detail);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
        }

        private static async Task<bool> ValidateMoney(Transaction transaction)
        {
            try
            {
                if (transaction.Amount > 0)
                {
                    var isValidateMoney = await api.CallApi("ValidateDispenserAmount", transaction.Amount);
                    //transaction.IsReturn = (bool)isValidateMoney;
                    return (bool)isValidateMoney;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
            return false;
        }

        public static async Task<int> GetConsecutive(bool isIncrement = true)
        {
            try
            {
                var response = await api.CallApi("GetConsecutiveTransaction", isIncrement);

                if (response != null)
                {
                    var consecutive = JsonConvert.DeserializeObject<ResponseConsecutive>(response.ToString());

                    if (consecutive.IS_AVAILABLE == true)
                    {
                        return int.Parse(consecutive.RANGO_ACTUAL.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
            }
            return 0;
        }

        //public static void VerifyTransaction()
        //{
        //    try
        //    {
        //        Task.Run(() =>
        //        {
        //            var transactions = DBManagment.GetTransactionErrror();
        //            if (transactions != null && transactions.Count > 0)
        //            {
        //                ApiIntegration.Verify(transactions);
        //            }
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "InitPaypad", ex, MessageResource.StandarError);
        //    }
        //}
    }
}
