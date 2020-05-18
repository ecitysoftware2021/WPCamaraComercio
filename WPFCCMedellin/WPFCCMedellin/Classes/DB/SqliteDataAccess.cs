using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services.Object;
using WPFCCMedellin.DataModel;

namespace WPFCCMedellin.Classes.DB
{
    static class SqliteDataAccess
    {
        public static TRANSACTION GetTRANSACTION(int idTransaction)
        {
            try
            {
                var query = string.Concat("SELECT * FROM 'TRANSACTION' WHERE TRANSACTION_ID = ", idTransaction);
                var transaction = Select<ITRANSACTION>(query).FirstOrDefault();

                return new TRANSACTION
                {
                    TYPE_TRANSACTION_ID = transaction.TYPE_TRANSACTION_ID,
                    PAYER_ID = transaction.PAYER_ID,
                    STATE_TRANSACTION_ID = transaction.STATE_TRANSACTION_ID,
                    TOTAL_AMOUNT = transaction.TOTAL_AMOUNT,
                    DATE_END = DateTime.Parse(transaction.DATE_END),
                    TRANSACTION_ID = transaction.TRANSACTION_ID,
                    RETURN_AMOUNT = transaction.RETURN_AMOUNT,
                    INCOME_AMOUNT = transaction.INCOME_AMOUNT,
                    PAYPAD_ID = transaction.PAYER_ID,
                    DATE_BEGIN = DateTime.Parse(transaction.DATE_BEGIN),
                    STATE_NOTIFICATION = transaction.STATE_NOTIFICATION,
                    STATE = transaction.STATE,
                    DESCRIPTION = transaction.DESCRIPTION,
                    TRANSACTION_REFERENCE = transaction.TRANSACTION_REFERENCE,
                    ID = transaction.ID
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<TRANSACTION_DETAIL> GetDetailsTransaction()
        {
            try
            {
                return Select<TRANSACTION_DETAIL>("SELECT * FROM 'TRANSACTION_DETAIL' WHERE STATE = 0");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool UpdateConfiguration(CONFIGURATION_PAYDAD config)
        {
            try
            {
                if (config != null)
                {
                    var configuration = Execute<object>("DELETE FROM CONFIGURATION_PAYDAD", null);
                    Execute<CONFIGURATION_PAYDAD>("INSERT INTO CONFIGURATION_PAYDAD (" +
                                "USER_API, " +
                                "PASSWORD_API," +
                                "USER, " +
                                "PASSWORD, " +
                                "TYPE, " +
                                "ID_PAYPAD, " +
                                "ID_SESSION, " +
                                "TOKEN_API) VALUES (" +
                                "@USER_API, " +
                                "@PASSWORD_API, " +
                                "@USER, " +
                                "@PASSWORD, " +
                                "@TYPE, " +
                                "@ID_PAYPAD, " +
                                "@ID_SESSION, " +
                                "@TOKEN_API)", config);
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public static int SaveTransaction(TRANSACTION transaction)
        {
            try
            {
                if (transaction != null)
                {
                    var idTransaccion = Execute<ITRANSACTION>("INSERT INTO 'TRANSACTION' (" +
                           "TRANSACTION_ID, " +
                           "PAYPAD_ID," +
                           "TYPE_TRANSACTION_ID, " +
                           "DATE_BEGIN, " +
                           "DATE_END, " +
                           "TOTAL_AMOUNT, " +
                           "INCOME_AMOUNT, " +
                           "RETURN_AMOUNT, " +
                           "DESCRIPTION, " +
                           "PAYER_ID, " +
                           "STATE_TRANSACTION_ID, " +
                           "STATE_NOTIFICATION, " +
                           "STATE, " +
                           "TRANSACTION_REFERENCE) VALUES (" +
                           "@TRANSACTION_ID, " +
                           "@PAYPAD_ID, " +
                           "@TYPE_TRANSACTION_ID, " +
                           "@DATE_BEGIN, " +
                           "@DATE_END, " +
                           "@TOTAL_AMOUNT, " +
                           "@INCOME_AMOUNT, " +
                           "@RETURN_AMOUNT, " +
                           "@DESCRIPTION, " +
                           "@PAYER_ID, " +
                           "@STATE_TRANSACTION_ID, " +
                           "@STATE_NOTIFICATION, " +
                           "@STATE, " +
                           "@TRANSACTION_REFERENCE) ",
                           new ITRANSACTION
                           {
                               TYPE_TRANSACTION_ID = transaction.TYPE_TRANSACTION_ID,
                               PAYER_ID = transaction.PAYER_ID,
                               STATE_TRANSACTION_ID = transaction.STATE_TRANSACTION_ID,
                               TOTAL_AMOUNT = transaction.TOTAL_AMOUNT,
                               DATE_END = transaction.DATE_END.ToString(),
                               TRANSACTION_ID = transaction.TRANSACTION_ID,
                               RETURN_AMOUNT = transaction.RETURN_AMOUNT,
                               INCOME_AMOUNT = transaction.INCOME_AMOUNT,
                               PAYPAD_ID = transaction.PAYER_ID,
                               DATE_BEGIN = transaction.DATE_BEGIN.ToString(),
                               STATE_NOTIFICATION = transaction.STATE_NOTIFICATION,
                               STATE = transaction.STATE,
                               DESCRIPTION = transaction.DESCRIPTION,
                               TRANSACTION_REFERENCE = transaction.TRANSACTION_REFERENCE
                           });
                    if (idTransaccion > 0)
                    {
                        string query = "INSERT INTO TRANSACTION_DESCRIPTION (" +
                               "TRANSACTION_ID, " +
                               "TRANSACTION_PRODUCT_ID," +
                               "AMOUNT, " +
                               "DESCRIPTION, " +
                               "EXTRA_DATA, " +
                               "STATE) VALUES (" +
                               "@TRANSACTION_ID, " +
                               "@TRANSACTION_PRODUCT_ID, " +
                               "@AMOUNT, " +
                               "@DESCRIPTION, " +
                               "@EXTRA_DATA, " +
                               "@STATE)";

                        foreach (var description in transaction.TRANSACTION_DESCRIPTION)
                        {
                            description.TRANSACTION_ID = idTransaccion;
                            Execute<TRANSACTION_DESCRIPTION>(query, description);
                        }

                        return GetTRANSACTION((int)transaction.TRANSACTION_ID).ID;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        public static TRANSACTION UpdateTransaction(Transaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    TRANSACTION data = GetTRANSACTION(transaction.IdTransactionAPi);

                    if (data != null)
                    {
                        data.TOTAL_AMOUNT = transaction.Payment.PayValue;
                        data.INCOME_AMOUNT = transaction.Payment.ValorIngresado;
                        data.RETURN_AMOUNT = transaction.Payment.ValorDispensado;
                        data.DESCRIPTION = "Transaccion finalizada correctamente";
                        data.STATE_TRANSACTION_ID = (int)transaction.State;
                        data.DATE_END = DateTime.Now;
                        data.TRANSACTION_REFERENCE = transaction.consecutive;
                        data.STATE_NOTIFICATION = transaction.StateNotification;

                        if (transaction.State != ETransactionState.ErrorService)
                        {
                            data.STATE_NOTIFICATION = 1;
                        }

                        Execute<TRANSACTION>("UPDATE 'TRANSACTION' SET INCOME_AMOUNT = @INCOME_AMOUNT, " +
                                    "RETURN_AMOUNT = @RETURN_AMOUNT, " +
                                    "DESCRIPTION = @DESCRIPTION, " +
                                    "STATE_TRANSACTION_ID = @STATE_TRANSACTION_ID, " +
                                    "DATE_END = @DATE_END, " +
                                    "STATE_NOTIFICATION = @STATE_NOTIFICATION, " +
                                    "TRANSACTION_REFERENCE = @TRANSACTION_REFERENCE," +
                                    "STATE = @STATE " +
                                    "WHERE ID = " + transaction.TransactionId, data);

                        return data;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public static void UpdateTransactionState(TRANSACTION tRANSACTION)
        {
            try
            {
                Execute<TRANSACTION>(string.Concat("UPDATE 'TRANSACTION' SET STATE = ", tRANSACTION.STATE, " WHERE ID = ", tRANSACTION.ID), null);
            }
            catch (Exception ex)
            {

            }
        }

        public static void UpdateTransactionDetailState(TRANSACTION_DETAIL transactionDetail)
        {
            try
            {
                Execute<TRANSACTION_DETAIL>(string.Concat("UPDATE 'TRANSACTION_DETAIL' SET STATE = ", transactionDetail.STATE, " WHERE TRANSACTION_DETAIL_ID = ", transactionDetail.TRANSACTION_ID), null);
            }
            catch (Exception ex)
            {

            }
        }

        public static object SaveLog(object log, ELogType type)
        {
            try
            {
                string query = "";
                object data = null;
                if (type == ELogType.General)
                {
                    data = new PAYPAD_LOG
                    {
                        REFERENCE = ((RequestLog)log).Reference,
                        DESCRIPTION = ((RequestLog)log).Description,
                        STATE = ((RequestLog)log).State,
                        DATE = DateTime.Now.ToString()
                    };

                    query = "INSERT INTO PAYPAD_LOG (" +
                                    "REFERENCE," +
                                    "DESCRIPTION, " +
                                    "DATE, " +
                                    "STATE) VALUES (" +
                                    "@REFERENCE, " +
                                    "@DESCRIPTION, " +
                                    "@DATE, " +
                                    "@STATE)";
                }
                else if (type == ELogType.Error)
                {
                    data = (ERROR_LOG)log;

                    query = "INSERT INTO ERROR_LOG (" +
                                    "NAME_CLASS, " +
                                    "NAME_FUNCTION," +
                                    "MESSAGE_ERROR, " +
                                    "DESCRIPTION, " +
                                    "DATE, " +
                                    "TYPE," +
                                    "STATE) VALUES (" +
                                    "@NAME_CLASS, " +
                                    "@NAME_FUNCTION, " +
                                    "@MESSAGE_ERROR, " +
                                    "@DESCRIPTION, " +
                                    "@DATE, " +
                                    "@TYPE, " +
                                    "@STATE)";
                }
                else
                {
                    var logDevice = (RequestLogDevice)log;

                    data = new DEVICE_LOG
                    {
                        TRANSACTION_ID = logDevice.TransactionId,
                        CODE = logDevice.Code,
                        DATETIME = logDevice.Date,
                        DESCRIPTION = logDevice.Description
                    };

                    query = "INSERT INTO DEVICE_LOG (" +
                                    "TRANSACTION_ID, " +
                                    "DESCRIPTION," +
                                    "DATETIME, " +
                                    "CODE) VALUES (" +
                                    "@TRANSACTION_ID, " +
                                    "@DESCRIPTION, " +
                                    "@DATETIME, " +
                                    "@CODE)";
                }
                if (!string.IsNullOrEmpty(query) && data != null)
                {
                    return Execute<object>(query, data);
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public static bool InsetConsoleError(PAYPAD_CONSOLE_ERROR error)
        {
            try
            {
                if (error != null)
                {
                    Execute<PAYPAD_CONSOLE_ERROR>("INSERT INTO PAYPAD_CONSOLE_ERROR (" +
                       "PAYPAD_ID, " +
                       "ERROR_ID," +
                       "ERROR_LEVEL_ID, " +
                       "DEVICE_PAYPAD_ID, " +
                       "DESCRIPTION, " +
                       "DATE, " +
                       "OBSERVATION, " +
                       "REFERENCE, " +
                       "STATE) VALUES (" +
                       "@PAYPAD_ID, " +
                       "@ERROR_ID, " +
                       "@ERROR_LEVEL_ID, " +
                       "@DEVICE_PAYPAD_ID, " +
                       "@DESCRIPTION, " +
                       "@DATE, " +
                       "@OBSERVATION, " +
                       "@REFERENCE, " +
                       "@STATE)", error);
                    return true;

                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public static int SaveTransactionDetail(RequestTransactionDetails detail, int state)
        {
            try
            {
                if (detail != null)
                {
                    return Execute<TRANSACTION_DETAIL>("INSERT INTO TRANSACTION_DETAIL (" +
                               "TRANSACTION_ID, " +
                               "CODE," +
                               "DENOMINATION, " +
                               "OPERATION, " +
                               "QUANTITY, " +
                               "DESCRIPTION, " +
                               "STATE) VALUES (" +
                               "@TRANSACTION_ID, " +
                               "@CODE, " +
                               "@DENOMINATION, " +
                               "@OPERATION, " +
                               "@QUANTITY, " +
                               "@DESCRIPTION, " +
                               "@STATE)", new TRANSACTION_DETAIL
                               {
                                   TRANSACTION_ID = detail.TransactionId,
                                   DENOMINATION = detail.Denomination,
                                   QUANTITY = detail.Quantity,
                                   OPERATION = detail.Operation,
                                   CODE = detail.Code,
                                   DESCRIPTION = detail.Description,
                                   STATE = state
                               });
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        internal static List<TRANSACTION> GetTransactionNotific()
        {
            try
            {
                var query = string.Concat("SELECT * FROM 'TRANSACTION' WHERE STATE = 0");
                var transactions = Select<ITRANSACTION>(query);

                if (transactions != null && transactions.Count > 0)
                {
                    List<TRANSACTION> dataList = new List<TRANSACTION>();
                    foreach (var transaction in transactions)
                    {
                        dataList.Add(new TRANSACTION
                        {
                            TYPE_TRANSACTION_ID = transaction.TYPE_TRANSACTION_ID,
                            PAYER_ID = transaction.PAYER_ID,
                            STATE_TRANSACTION_ID = transaction.STATE_TRANSACTION_ID,
                            TOTAL_AMOUNT = transaction.TOTAL_AMOUNT,
                            DATE_END = DateTime.Parse(transaction.DATE_END),
                            TRANSACTION_ID = transaction.TRANSACTION_ID,
                            RETURN_AMOUNT = transaction.RETURN_AMOUNT,
                            INCOME_AMOUNT = transaction.INCOME_AMOUNT,
                            PAYPAD_ID = transaction.PAYER_ID,
                            DATE_BEGIN = DateTime.Parse(transaction.DATE_BEGIN),
                            STATE_NOTIFICATION = transaction.STATE_NOTIFICATION,
                            STATE = transaction.STATE,
                            DESCRIPTION = transaction.DESCRIPTION,
                            TRANSACTION_REFERENCE = transaction.TRANSACTION_REFERENCE,
                            ID = transaction.ID
                        });

                    }
                    return dataList;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "SqliteDataAccess", ex, MessageResource.StandarError);
            }
            return null;
        }

        internal static List<TRANSACTION> GetTransactionErrror()
        {
            try
            {
                var transactions = Select<ITRANSACTION>(string.Concat("SELECT * FROM 'TRANSACTION' WHERE STATE_NOTIFICATION = 0 AND STATE_TRANSACTION_ID <> ", (int)ETransactionState.Initial));
                if (transactions != null && transactions.Count > 0)
                {
                    List<TRANSACTION> dataList = new List<TRANSACTION>();
                    foreach (var transaction in transactions)
                    {
                        dataList.Add(new TRANSACTION
                        {
                            TYPE_TRANSACTION_ID = transaction.TYPE_TRANSACTION_ID,
                            PAYER_ID = transaction.PAYER_ID,
                            STATE_TRANSACTION_ID = transaction.STATE_TRANSACTION_ID,
                            TOTAL_AMOUNT = transaction.TOTAL_AMOUNT,
                            DATE_END = DateTime.Parse(transaction.DATE_END),
                            TRANSACTION_ID = transaction.TRANSACTION_ID,
                            RETURN_AMOUNT = transaction.RETURN_AMOUNT,
                            INCOME_AMOUNT = transaction.INCOME_AMOUNT,
                            PAYPAD_ID = transaction.PAYER_ID,
                            DATE_BEGIN = DateTime.Parse(transaction.DATE_BEGIN),
                            STATE_NOTIFICATION = transaction.STATE_NOTIFICATION,
                            STATE = transaction.STATE,
                            DESCRIPTION = transaction.DESCRIPTION,
                            TRANSACTION_REFERENCE = transaction.TRANSACTION_REFERENCE,
                            ID = transaction.ID
                        });

                    }
                    return dataList;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
            return null;
        }

        internal static TRANSACTION UpdateTransactionError(TRANSACTION transaction, bool response)
        {
            try
            {
                if (transaction != null)
                {
                    if (response)
                    {
                        Execute<TRANSACTION_ERROR_SERVICE>(string.Concat("DELETE FROM 'TRANSACTION_ERROR_SERVICE' WHERE TRANSACTION_ID = ", transaction.ID), null);
                        Execute<TRANSACTION>(string.Concat("UPDATE 'TRANSACTION' SET STATE_TRANSACTION_ID = ", (int)ETransactionState.Success, ", STATE_NOTIFICATION = 1, STATE = 0 WHERE ID = ", transaction.ID), null);
                    }
                    else
                    {
                        var transactionError = Select<TRANSACTION_ERROR_SERVICE>(string.Concat("SELECT * FROM 'TRANSACTION_ERROR_SERVICE' WHERE TRANSACTION_ID = ", transaction.ID)).FirstOrDefault();

                        if (transactionError != null)
                        {
                            transactionError.NOTIFICATION_INTENT -= 1;
                            if (transactionError.NOTIFICATION_INTENT <= 0)
                            {
                                Execute<TRANSACTION_ERROR_SERVICE>(string.Concat("DELETE FROM 'TRANSACTION_ERROR_SERVICE' WHERE TRANSACTION_ID = ", transaction.ID), null);
                                Execute<TRANSACTION>(string.Concat("UPDATE 'TRANSACTION' SET STATE_TRANSACTION_ID = ", (int)ETransactionState.ErrorService, ", STATE_NOTIFICATION = 2 WHERE ID = ", transaction.ID), null);
                            }
                            else
                            {
                                Execute<TRANSACTION_ERROR_SERVICE>(string.Concat("UPDATE 'TRANSACTION_ERROR_SERVICE' SET NOTIFICATION_INTENT = ", transactionError.NOTIFICATION_INTENT, " WHERE TRANSACTION_ID = ", transaction.ID), null);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
            return transaction;
        }

        private static IDbConnection LoadConnectionString(string id = "ConnectionString")
        {
            return new SQLiteConnection(@"" + Utilities.GetConfiguration(id, false).ToString());
        }

        public static List<T> Select<T>(string query)
        {
            List<T> result = default(List<T>);
            try
            {
                using (IDbConnection connection = LoadConnectionString())
                {
                    try
                    {
                        result = connection.Query<T>(query).ToList();
                    }
                    catch (InvalidOperationException ex)
                    {
                        // String passed is not XML, simply return defaultXmlClass
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public static int Execute<T>(string query, T data)
        {
            object result = 0;
            try
            {
                using (IDbConnection connection = LoadConnectionString())
                {
                    try
                    {
                        if (data == null)
                        {
                            result = (int)connection.Execute(query);
                        }
                        else
                        {
                            result = connection.Execute(query, data);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        // String passed is not XML, simply return defaultXmlClass
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return (int)result;
        }
    }
}
