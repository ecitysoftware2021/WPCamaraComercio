using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
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

        public static bool ShowModal(string message, EModalType type, bool stopTimer = true, bool restarApp = false)
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
                    model.ImageModal = ImagesUrlResource.AlertBlanck;
                }
                else if (type == EModalType.Information)
                {
                    model.ImageModal = ImagesUrlResource.AlertBlanck;
                }
                else if (type == EModalType.NoPaper)
                {
                    model.ImageModal = ImagesUrlResource.AlertBlanck;
                }

                Application.Current.Dispatcher.Invoke(delegate
                {
                    modal = new ModalWindow(model);
                    modal.ShowDialog();

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

        public static void PrintVoucher(Transaction transaction)
        {
            try
            {
                var data = new List<DataPrinter>()
                {
                    new DataPrinter{ image = GetConfiguration("imageBoucher"),  x = 50, y = 20 },
                    new DataPrinter{ brush = new SolidBrush(Color.Black),
                                     font = new Font("Arial", 8, System.Drawing.FontStyle.Regular),
                                     key = "Esteban ", value = GetConfiguration("Est") ?? string.Empty,
                                     x = 50, y = 20 },
                    new DataPrinter{ brush = new SolidBrush(Color.Black),
                                     font = new Font("Arial", 8, System.Drawing.FontStyle.Regular),
                                     key = "penagos", value = GetConfiguration("Est") ?? string.Empty,
                                     x = 100, y = 20 },
                };
                AdminPayPlus.PrintService.Start(data);
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
                var data = new List<DataPrinter>()
                {
                    new DataPrinter{ image = GetConfiguration("imageBoucher"),  x = 50, y = 20 },
                    new DataPrinter{ brush = new SolidBrush(Color.Black),
                                     font = new Font("Arial", 8, System.Drawing.FontStyle.Regular),
                                     key = "Est", value = GetConfiguration("Est") ?? string.Empty,
                                     x = 50, y = 20 },
                };
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

        public static T ConverJson<T>(string path)
        {
            T response = default(T);
            try
            {
                using (StreamReader file = new StreamReader(path))
                {
                    try
                    {
                        if (string.IsNullOrEmpty(file.ReadToEnd().ToString()))
                        {
                            response = JsonConvert.DeserializeObject<T>(file.ReadToEnd().ToString());
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
    }
}
