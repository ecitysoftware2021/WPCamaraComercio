using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WPFCCMedellin.DataModel;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services.Object;

namespace WPFCCMedellin.Classes
{
    public static class DBManagment
    {
        internal static bool UpdateConfiguration(CONFIGURATION_PAYDAD config)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    var configuration = connection.CONFIGURATION_PAYDAD.FirstOrDefault();
                    if (configuration != null)
                    {
                        configuration = config;
                    }
                    else
                    {
                        connection.CONFIGURATION_PAYDAD.Add(config);
                    }

                    connection.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
                return false;
            }
        }

        internal static bool SaveLog(object log, ELogType type)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    if (type == ELogType.General)
                    {
                        connection.PAYPAD_LOG.Add(new PAYPAD_LOG
                        {
                            REFERENCE = ((RequestLog)log).Reference,
                            DATE = DateTime.Now,
                            DESCRIPTION = ((RequestLog)log).Description,
                            STATE = 1
                        });
                    }
                    else if (type == ELogType.Error)
                    {
                        connection.ERROR_LOG.Add((ERROR_LOG)log);
                    }
                    else
                    {
                        var logDevice = (RequestLogDevice)log;

                        connection.DEVICE_LOG.Add(new DEVICE_LOG
                        {
                            TRANSACTION_ID = logDevice.TransactionId,
                            CODE = logDevice.Code,
                            DATETIME = logDevice.Date,
                            DESCRIPTION = logDevice.Description
                        });
                    }
                    connection.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
            return false;
        }

        internal static void InsetConsoleError(PAYPAD_CONSOLE_ERROR consoleError)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    if (consoleError != null)
                    {
                        connection.PAYPAD_CONSOLE_ERROR.Add(consoleError);

                        connection.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
        }

        internal static int SaveTransaction(TRANSACTION data)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    if (data != null)
                    {
                        connection.TRANSACTION.Add(data);
                    }

                    connection.SaveChanges();
                }
                return data.ID;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
                return 0;
            }
        }

        internal static object SaveTransactionDetail(RequestTransactionDetails detail, bool state)
        {
            try
            {
                var tRANSACTION_DETAIL = new TRANSACTION_DETAIL
                {
                    TRANSACTION_ID = detail.TransactionId,
                    DENOMINATION = detail.Denomination,
                    QUANTITY = detail.Quantity,
                    OPERATION = detail.Operation,
                    CODE = detail.Code,
                    DESCRIPTION = detail.Description,
                    STATE = state
                };

                using (var connection = new PayPlusLocalBDEntities())
                {
                    if (tRANSACTION_DETAIL != null)
                    {
                        connection.TRANSACTION_DETAIL.Add(tRANSACTION_DETAIL);
                    }

                    connection.SaveChanges();
                }
                return tRANSACTION_DETAIL;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
                return null;
            }
        }

        internal static TRANSACTION UpdateTransaction(Transaction transaction)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    connection.Configuration.LazyLoadingEnabled = false;
                    var data = connection.TRANSACTION.Where(t => t.ID == transaction.TransactionId).FirstOrDefault();

                    if (data != null)
                    {
                        data.TOTAL_AMOUNT = transaction.Payment.PayValue;
                        data.INCOME_AMOUNT = transaction.Payment.ValorIngresado;
                        data.RETURN_AMOUNT = transaction.Payment.ValorDispensado;
                        data.DESCRIPTION = MessageResource.TransactionFinishSucces;
                        data.STATE_TRANSACTION_ID = (int)transaction.State;
                        data.DATE_END = DateTime.Now;
                    }

                    connection.SaveChanges();
                    return data;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
            return null;
        }

        internal static void UpdateTransactionState(TRANSACTION transaction, int type)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    var data = connection.TRANSACTION.Where(t => t.ID == transaction.ID).FirstOrDefault();
                    if (data != null)
                    {
                        if (type == 1)
                        {
                            data.STATE_NOTIFICATION = transaction.STATE_NOTIFICATION;
                        }
                        else if (type == 2)
                        {
                            data.STATE = transaction.STATE;
                        }
                        else
                        {
                            data.STATE_TRANSACTION_ID = transaction.STATE_TRANSACTION_ID;
                            data.STATE = transaction.STATE;
                        }
                    }
                    connection.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
        }

        internal static List<TRANSACTION> GetTransactionNotific()
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    connection.Configuration.LazyLoadingEnabled = false;
                    return connection.TRANSACTION.Where(t => t.STATE_NOTIFICATION == 0).ToList();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
                return null;
            }
        }

        internal static List<TRANSACTION_DETAIL> GetDetailsTransaction()
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    return connection.TRANSACTION_DETAIL.Where(t => t.STATE == false).ToList();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
                return null;
            }
        }

        internal static void UpdateTransactionDetailState(TRANSACTION_DETAIL transactionDetail)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    var detail = connection.TRANSACTION_DETAIL.Where(t => t.TRANSACTION_DETAIL_ID == transactionDetail.TRANSACTION_DETAIL_ID).FirstOrDefault();
                    if (detail != null)
                    {
                        detail.STATE = true;
                    }
                    connection.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
        }

        internal static object GetTransactionErrorService(int idTransaction)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    connection.Configuration.LazyLoadingEnabled = false;
                    var transactionError = connection.TRANSACTION_ERROR_SERVICE.Where(t => t.TRANSACTION_ID == idTransaction).FirstOrDefault();
                    if (transactionError != null)
                    {
                        return JsonConvert.DeserializeObject<object>(transactionError.JSON.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
            return null;
        }

        internal static List<TRANSACTION> GetTransactionErrror()
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    connection.Configuration.LazyLoadingEnabled = false;
                    var transactionsError = connection.TRANSACTION.Where(t => t.STATE_TRANSACTION_ID == (int)ETransactionState.ErrorService).ToList();
                    if (transactionsError != null && transactionsError.Count > 0)
                    {
                        return transactionsError;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
            return null;
        }

        internal static void InsertTransactionError(object payed, int idTransaction)
        {
            try
            {
                var tRANSACTION_ERROR_SERVICE = new TRANSACTION_ERROR_SERVICE
                {
                    TRANSACTION_ID = idTransaction,
                    JSON = JsonConvert.SerializeObject(payed).ToString(),
                    DESCRIPTION = "Transaccion",
                    NOTIFICATION_INTENT = 3,
                    STATE = 0
                };

                using (var connection = new PayPlusLocalBDEntities())
                {
                    if (tRANSACTION_ERROR_SERVICE != null)
                    {
                        connection.TRANSACTION_ERROR_SERVICE.Add(tRANSACTION_ERROR_SERVICE);
                    }

                    connection.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
        }

        internal static TRANSACTION UpdateTransactionError(TRANSACTION transaction, bool response)
        {
            try
            {
                using (var connection = new PayPlusLocalBDEntities())
                {
                    connection.Configuration.LazyLoadingEnabled = false;
                    if (transaction != null)
                    {
                        var transactionError = connection.TRANSACTION_ERROR_SERVICE.Where(t => t.TRANSACTION_ID == transaction.ID).FirstOrDefault();
                        if (transactionError != null)
                        {
                            if (response)
                            {
                                connection.TRANSACTION_ERROR_SERVICE.Remove(transactionError);
                                transaction.STATE_TRANSACTION_ID = (int)ETransactionState.Success;
                            }
                            else
                            {
                                transactionError.NOTIFICATION_INTENT -= 1;
                                if (transactionError.NOTIFICATION_INTENT <= 0)
                                {
                                    connection.TRANSACTION_ERROR_SERVICE.Remove(transactionError);
                                    transaction.STATE_TRANSACTION_ID = (int)ETransactionState.Success;
                                }
                            }
                        }
                    }
                    connection.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "DBManagment", ex, MessageResource.StandarError);
            }
            return transaction;
        }
    }
}