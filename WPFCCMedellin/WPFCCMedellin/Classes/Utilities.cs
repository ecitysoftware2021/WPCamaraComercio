using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using WPFCCMedellin.Classes.Printer;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services.Object;
using WPFCCMedellin.Windows;

namespace WPFCCMedellin.Classes
{
    public class Utilities
    {
        #region "Referencias"

        public static Navigation navigator { get; set; }

        private static ModalWindow modal { get; set; }

        #endregion
        public static string GetConfiguration(string key, bool decodeString = false)
        {
            try
            {
                string value = "";
                AppSettingsReader reader = new AppSettingsReader();
                value = reader.GetValue(key, typeof(String)).ToString();
                if (decodeString)
                {
                    value = Encryptor.Decrypt(value);
                }
                return value;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return string.Empty;
            }
        }

        public static void ShowDetailsModal(object data, ETypeCertificate type)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                ModalDetailWindows modalDetails = new ModalDetailWindows(data, type);
                modalDetails.ShowDialog();
            });
        }

        public static bool ShowModal(string message, EModalType type, bool timer = false, bool openDialog = true)
        {
            bool response = false;
            try
            {
                ModalModel model = new ModalModel
                {
                    Tittle = "Estimado Cliente: ",
                    Messaje = message,
                    TypeModal = type,
                    ImageModal = ImagesUrlResource.AlertBlanck,
                };

                if (type == EModalType.Error)
                {
                    timer = true;
                    model.ImageModal = ImagesUrlResource.AlertError;
                }
                else if (type == EModalType.Information)
                {
                    model.ImageModal = ImagesUrlResource.AlertInfo;
                }
                else if (type == EModalType.NoPaper)
                {
                    model.ImageModal = ImagesUrlResource.AlertInfo;
                }

                TimerService.Close();

                if (timer)
                {
                    TimerService.CallBackTimerOut = result =>
                    {
                        CloseModal();
                    };

                    TimerService.Start(int.Parse(Utilities.GetConfiguration("DurationAlert")));
                }

                Application.Current.Dispatcher.Invoke(delegate
                {
                    modal = new ModalWindow(model);
                    if (openDialog)
                    {
                        modal.ShowDialog();
                    }
                    else
                    {
                        modal.Show();
                    }

                    if (modal.DialogResult.HasValue && modal.DialogResult.Value)
                    {
                        response = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
            }
            GC.Collect();
            return response;
        }

        public static void CloseModal() => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                if (modal != null)
                {
                    modal.Close();
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);

            }
        });

        public static void RestartApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(Directory.GetCurrentDirectory(), GetConfiguration("NAME_APLICATION"));
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, MessageResource.StandarError);
            }
        }

        public static string[] ReadFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllLines(path);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, MessageResource.StandarError);
            }
            return null;
        }

        public static void PrintVoucher(Transaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    var statePrint = AdminPayPlus.PrintService.StatusPrint();
                    SolidBrush color = new SolidBrush(Color.Black);
                    Font fontKey = new Font("Arial", 9, System.Drawing.FontStyle.Bold);
                    Font fontValue = new Font("Arial", 9, System.Drawing.FontStyle.Regular);
                    int y = 0;
                    int sum = 25;
                    int x = 150;
                    int xKey = 20;

                    var data = new List<DataPrinter>()
                        {
                            new DataPrinter{ image = GetConfiguration("ImageBoucher"),  x = 2, y = 2 },
                            new DataPrinter{ brush = color, font = fontKey, value = "NIT:", x = xKey, y = y+=120 },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = GetConfiguration("NIT") ?? string.Empty, x = x, y = y },
                            new DataPrinter{ brush = color, font = fontKey, value = "Trámite:", x = xKey, y = y+=sum },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = GetConfiguration("ProductName") ?? string.Empty, x = x, y = y },
                            new DataPrinter{ brush = color, font = fontKey, value = "Estado:", x = xKey, y = y+=sum },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = (transaction.State == ETransactionState.Success || transaction.State == ETransactionState.ErrorService || transaction.State == ETransactionState.Error)
                                ? "Aprobada" : "Cancelada", x = x, y = y },
                            new DataPrinter{ brush = color, font = fontKey, value = "Fecha de pago:", x = xKey, y = y+=sum },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = DateTime.Now.ToString(), x = x, y = y },
                            new DataPrinter{ brush = color, font = fontKey, value = "Referencia:", x = xKey, y = y+=sum },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = transaction.IdTransactionAPi.ToString() ?? string.Empty, x = x, y = y},
                            new DataPrinter{ brush = color, font = fontKey, value = "ID Compra:", x = xKey, y = y+=sum },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = transaction.consecutive ?? string.Empty, x = x, y = y },
                            new DataPrinter{ brush = color, font = fontKey, value = "Identificación:", x = xKey, y = y+=sum },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = transaction.payer.IDENTIFICATION ?? string.Empty, x = x, y = y },
                            new DataPrinter{ brush = color, font = fontKey, value = "Nombre:", x = xKey, y = y+=sum },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = transaction.payer.NAME ?? string.Empty, x = x, y = y },
                            new DataPrinter{ brush = color, font = fontKey, value = "Teléfono:", x = xKey, y = y+=sum },
                            new DataPrinter{ brush = color, font = fontValue,
                                value = transaction.payer.PHONE.ToString() ?? string.Empty, x = x, y = y },
                        };

                    data.Add(new DataPrinter { brush = color, font = fontKey, value = "Total:", x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", transaction.Payment.PayValue), x = x, y = y });

                    data.Add(new DataPrinter { brush = color, font = fontKey, value = "Total Ingresado:", x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", transaction.Payment.ValorIngresado), x = x, y = y });
                    data.Add(new DataPrinter { brush = color, font = fontKey, value = "Total Devuelto:", x = xKey, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", transaction.Payment.ValorDispensado), x = x, y = y });
                    if (transaction.State == ETransactionState.Success)
                    {
                        data.Add(new DataPrinter { brush = color, font = fontValue, value = "Su transacción se ha realizado exitosamente", x = 0, y = y += 50 });
                    }
                    else
                    {
                        data.Add(new DataPrinter { brush = color, font = fontValue, value = transaction.Observation ?? string.Empty, x = 0, y = y += 50 });
                    }
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "Recuerde siempre esperar la tirilla de soporte de su", x = 10, y = y += sum });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "transacción, es el único documento que lo respalda.", x = 10, y = y += 20 });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = "E-city Software", x = 100, y = y += sum });

                    AdminPayPlus.PrintService.Start(data);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PrintVoucher", ex);
            }
        }

        public static void PrintVoucher(PaypadOperationControl dataControl, ETypeAdministrator type)
        {
            try
            {
                SolidBrush color = new SolidBrush(Color.Black);
                Font fontKey = new Font("Arial", 8, System.Drawing.FontStyle.Bold);
                Font fontValue = new Font("Arial", 8, System.Drawing.FontStyle.Regular);
                int y = 0;
                int sum = 30;
                int x = 150;
                int xKey = 10;

                var data = new List<DataPrinter>()
                {
                    new DataPrinter{ image = GetConfiguration("ImageBoucher"),  x = 2, y = 2 },
                };
                if (type == ETypeAdministrator.Balancing)
                {
                    data.Add(new DataPrinter { brush = color, font = fontKey, value = "DISMINUCIÓN DE EFECTIVO", x = 80, y = y += sum });
                }
                else
                {
                    data.Add(new DataPrinter { brush = color, font = fontKey, value = "PROVISIÓN DE EFECTIVO", x = 80, y = y += sum });
                }
                data.Add(new DataPrinter { brush = color, font = fontKey, value = "USUARIO:", x = xKey, y = y += sum });
                data.Add(new DataPrinter { brush = color, font = fontValue, value = "PayPlus" ?? string.Empty, x = x, y = y });
                data.Add(new DataPrinter { brush = color, font = fontKey, value = "OFI", x = 30, y = y += sum });
                data.Add(new DataPrinter { brush = color, font = fontKey, value = "FECHA", x = 100, y = y });
                data.Add(new DataPrinter { brush = color, font = fontKey, value = "HORA", x = 170, y = y });
                data.Add(new DataPrinter { brush = color, font = fontValue, value = GetConfiguration("Terminal") ?? string.Empty, x = 30, y = y += 15 });
                data.Add(new DataPrinter { brush = color, font = fontValue, value = DateTime.Now.ToString("yyyy/MM/dd"), x = 100, y = y });
                data.Add(new DataPrinter { brush = color, font = fontValue, value = DateTime.Now.ToString("hh:mm:ss"), x = 170, y = y });
                data.Add(new DataPrinter { brush = color, font = fontKey, value = "DENOMINACION", x = 10, y = y += 15 });
                data.Add(new DataPrinter { brush = color, font = fontKey, value = "CANT", x = 130, y = y });
                data.Add(new DataPrinter { brush = color, font = fontKey, value = "MONTO", x = 230, y = y });
                foreach (var item in dataControl.DATALIST_FILTER())
                {
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", item.Denominacion), x = 10, y = y += 18 });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = item.Quantity.ToString(), x = 130, y = y });
                    data.Add(new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", item.Total), x = 230, y = y });
                }
                data.Add(new DataPrinter { brush = color, font = fontKey, value = "TOTAL TRANSACCIÓN  : ", x = xKey, y = y += sum });
                data.Add(new DataPrinter { brush = color, font = fontValue, value = string.Format("{0:C0}", dataControl.TOTAL), x = y, y = y });
                AdminPayPlus.PrintService.Start(data);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
            }
        }

        public static decimal RoundValue(decimal Total)
        {
            try
            {
                decimal roundTotal = 0;
                roundTotal = Math.Floor(Total / 100) * 100;
                return roundTotal;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return Total;
            }
        }

        public static bool ValidateModule(decimal module, decimal amount)
        {
            try
            {
                var result = (amount % module);
                return result == 0 ? true : false;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex);
                return false;
            }
        }


        public static bool IsValidEmailAddress(string email)
        {
            try
            {
                Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,8}$");
                return regex.IsMatch(email);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, ex.ToString());
                return false;
            }
        }


        public static T ConverJson<T>(string path)
        {
            T response = default(T);
            try
            {
                using (StreamReader file = new StreamReader(path, Encoding.UTF8))
                {
                    try
                    {
                        var json = file.ReadToEnd().ToString();
                        if (!string.IsNullOrEmpty(json))
                        {
                            response = JsonConvert.DeserializeObject<T>(json);
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, MessageResource.StandarError);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, MessageResource.StandarError);
            }
            return response;
        }

        public static string SaveFile(string nameFile, string directoryFile, byte[] file)
        {
            try
            {
                if (file != null && !string.IsNullOrEmpty(nameFile) && !string.IsNullOrEmpty(directoryFile))
                {
                    var path = Path.Combine(directoryFile, nameFile + ".pdf");
                    if (!Directory.Exists(directoryFile))
                    {
                        Directory.CreateDirectory(directoryFile);
                    }

                    using (FileStream fileStream = File.Create(path))
                    {
                        fileStream.Write(file, 0, file.Length);
                    }
                    return path;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, MessageResource.StandarError);
            }
            return string.Empty;
        }

        public static void UpdateApp()
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                {
                    Process pc = new Process();
                    Process pn = new Process();
                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = GetConfiguration("APLICATION_UPDATE");
                    pn.StartInfo = si;
                    pn.Start();
                    pc = Process.GetCurrentProcess();
                    pc.Kill();
                }));
                GC.Collect();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Utilities", ex, MessageResource.StandarError);
            }
        }

        public static void OpenKeyboard(bool keyBoard_Numeric, TextBox textBox, object thisView, int x = 0, int y = 0)
        {
            try
            {
                WPKeyboard.Keyboard.InitKeyboard(new WPKeyboard.Keyboard.DataKey
                {
                    control = textBox,
                    userControl = thisView is UserControl ? thisView as UserControl : null,
                    eType = (keyBoard_Numeric == true) ? WPKeyboard.Keyboard.EType.Numeric : WPKeyboard.Keyboard.EType.Standar,
                    window = thisView is Window ? thisView as Window : null,
                    X = x,
                    Y = y,
                });
            }
            catch (Exception ex)
            {
            }
        }
    }
}
