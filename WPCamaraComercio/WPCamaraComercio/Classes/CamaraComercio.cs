using Ghostscript.NET;
using Ghostscript.NET.Processor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;
using WPCamaraComercio.Views;
using static WPCamaraComercio.Objects.ObjectsApi;

namespace WPCamaraComercio.Classes
{
    public class CamaraComercio
    {
        #region References
        CertificadoComerciante Certificate = new CertificadoComerciante();
        Print print = new Print();
        LocalPrintServer server = new LocalPrintServer();
        Utilities utilities = new Utilities();
        List<DetailCartificate> ListDetailCert = new List<DetailCartificate>();
        WCFServices service = new WCFServices();
        Datos datos = new Datos();
        string responseDic = string.Empty;
        private string FileName { get; set; }
        private bool printState = true;
        private static string IDCompra { get; set; }
        public static string Delimitador { get { return "-"; } }
        private string DirectoryFile { get; set; }
        private string path { get; set; }
        private byte[] bytePDF { get; set; }
        public static string PrinterName { get; set; }
        private List<string> LRutasCertificados = new List<string>();
        List<LogError> logError = new List<LogError>();
        string message = string.Concat("Lo sentimos, ",
                           Environment.NewLine,
                           "En este momento el servicio no se encuentra disponible.");


        Api api = new Api();
        #endregion

        #region Methods
        public async Task<string> ConfirmarCompra()
        {
            try
            {

                string idCompra = string.Empty;
                datos.AutorizaEnvioEmail = "NO";
                datos.AutorizaEnvioSMS = "NO";
                datos.CodigoDepartamentoComprador = Utilities.PayerData.CodeDepartmentBuyer;
                datos.CodigoMunicipioComprador = Utilities.PayerData.CodeTownBuyer;
                datos.CodigoPaisComprador = Utilities.PayerData.CodeCountryBuyer;
                datos.DireccionComprador = Utilities.PayerData.BuyerAddress;
                datos.EmailComprador = Utilities.PayerData.Email;
                datos.IdentificacionComprador = Utilities.PayerData.BuyerIdentification;
                datos.MunicipioComprador = string.Empty;
                datos.NombreComprador = Utilities.PayerData.FullNameBuyer;
                datos.PlataformaCliente = Utilities.PayerData.ClientPlataform;
                datos.PrimerApellidoComprador = Utilities.PayerData.SecondNameBuyer;
                datos.PrimerNombreComprador = Utilities.PayerData.FirstNameBuyer;
                datos.ReferenciaPago = Utilities.IDTransactionDB.ToString();
                datos.SegundoApellidoComprador = string.Empty;
                datos.SegundoNombreComprador = Utilities.PayerData.SecondNameBuyer;
                datos.TelefonoComprador = Utilities.PayerData.Phone;
                datos.TipoComprador = Utilities.PayerData.TypeBuyer;
                datos.TipoIdentificacionComprador = Utilities.PayerData.TypeIdBuyer;
                datos.ValorCompra = decimal.Parse(Utilities.ValueToPay.ToString());
                datos.Certificados = Utilities.ListCertificates.ToArray();
                var request = JsonConvert.SerializeObject(datos);
                var task = api.GetData(new RequestApi
                {
                    Data = request
                }, "SendPay");

                if (await Task.WhenAny(task, Task.Delay(50000)) == task)
                {
                    var response = task.Result;

                    if (response.CodeError==200)
                    {
                        int validator = 0;
                        responseDic = response.Data.ToString();
                        if (int.TryParse(responseDic, out validator))
                        {
                            idCompra = responseDic;
                            utilities.FillLogError(idCompra, "Resultado al Confirmar la Compra");

                            return idCompra;
                        }
                        else
                        {
                            await Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                            {
                                FrmModal mod4al = new FrmModal(message);
                                mod4al.ShowDialog();
                            }));
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    await Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                    {
                        FrmModal mod4al = new FrmModal(message);
                        mod4al.ShowDialog();
                    }));
                }
            }
            catch (Exception ex)
            {
                printState = false;
                LlenarLogError(ex.Message, "Metodo ConfirmarCompra");
            }
            return string.Empty;
        }

        void LlenarLogError(string error, string operacion)
        {
            logError.Add(new LogError
            {
                Fecha = DateTime.Now,
                IDTrsansaccion = Utilities.IDTransactionDB,
                Operacion = operacion,
                Error = error
            });
            utilities.CreateLogError(logError);
            logError.Clear();
        }

        public async Task<bool> ListCertificadosiMPORT()
        {
            try
            {
                PrinterName = Utilities.GetConfiguration("PrinterName");
                foreach (var item in Utilities.ListCertificates)
                {
                    CLSDatosCertificado datosCertificado = new CLSDatosCertificado();
                    datosCertificado.IdCertificado = item.IdCertificado;
                    datosCertificado.idcompra = Utilities.BuyID;
                    datosCertificado.matricula = item.matricula;
                    datosCertificado.matriculaest = item.MatriculaEst;
                    datosCertificado.referenciaPago = Utilities.IDTransactionDB.ToString();
                    datosCertificado.tpcm = item.tpcm;

                    bytePDF = null;

                    for (int i = 0; i < int.Parse(item.NumeroCertificados); i++)
                    {
                        datosCertificado.copia = (i + 1).ToString();
                        var request = JsonConvert.SerializeObject(datosCertificado);
                        var task = api.GetData(new RequestApi
                        {
                            Data = request
                        }, "SendPay");
                        if (await Task.WhenAny(task, Task.Delay(50000)) == task)
                        {
                            var response = task.Result;

                            if (response.CodeError==200)
                            {
                                string urlArchivo = (string)response.Data;
                                if (!string.IsNullOrEmpty(urlArchivo))
                                {
                                    FileName nombreArchivo = new FileName
                                    {
                                        matricula = item.matricula,
                                        matriculaest = item.MatriculaEst,
                                        tpcm = item.tpcm,
                                        IdCertificado = item.IdCertificado,
                                        Copia = (i + 1)
                                    };
                                    utilities.FillLogError(urlArchivo, $"URL del Certificado {datosCertificado.copia}");
                                    path = SaveFile(urlArchivo, nombreArchivo);
                                    LRutasCertificados.Add(path);
                                }
                                else
                                {
                                    printState = false;
                                }
                            }
                        }
                        else
                        {
                            printState = false;
                            await Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
                            {
                                FrmModal mod4al = new FrmModal(message);
                                mod4al.ShowDialog();
                            }));
                        }
                    }
                }
                if (printState)
                {
                    foreach (var item in LRutasCertificados)
                    {
                        Print(item);
                    }
                }
                return printState;
            }
            catch (Exception ex)
            {
                Mensaje(ex.Message);
                printState = false;
            }
            return true;
        }

        public string SaveFile(string PatchFile, FileName nombreArchivo)
        {
            try
            {
                FileName = string.Concat(
                    Utilities.BuyID,
                    Delimitador,
                    nombreArchivo.IdCertificado,
                    Delimitador,
                    nombreArchivo.matricula,
                    Delimitador,
                    nombreArchivo.matriculaest,
                    Delimitador,
                    nombreArchivo.tpcm,
                    Delimitador,
                    nombreArchivo.Copia);
                DirectoryFile = "C:\\CertificadosElectronicos";
                utilities.FillLogError(PatchFile, "Contenido de PatchFile");
                WebClient myWebClient = new WebClient();
                bytePDF = myWebClient.DownloadData(PatchFile);
                path = Path.Combine(DirectoryFile, FileName + ".pdf");
                if (!Directory.Exists(DirectoryFile))
                {
                    Directory.CreateDirectory(DirectoryFile);
                }

                using (FileStream fs = File.Create(path))
                {
                    fs.Write(bytePDF, 0, bytePDF.Length);
                }
                if (bytePDF != null)
                {
                    if (bytePDF.Length < 1000)
                    {
                        Mensaje("Tamaño del PDF es:" + bytePDF.Length.ToString());
                        printState = false;
                    }
                }
                else
                {
                    Mensaje("PDF Null");
                    printState = false;
                }
            }
            catch (Exception ex)
            {
                Mensaje(ex.Message);
            }
            return path;
        }

        public void Print(string rutaArchivo)
        {
            //rutaArchivo = "C:\\CertificadosElectronicos\\248979-10-317282-0-12-1.pdf";
            try
            {
                using (GhostscriptProcessor processor = new GhostscriptProcessor(GhostscriptVersionInfo.GetLastInstalledVersion(), true))
                {
                    List<string> switches = new List<string>();
                    switches.Add("-empty");
                    switches.Add("-dPrinted");
                    switches.Add("-dBATCH");
                    switches.Add("-dPDFFitPage");
                    switches.Add("-dNOPAUSE");
                    switches.Add("-dNOSAFER");
                    switches.Add("-dNOPROMPT");
                    switches.Add("-dQUIET");
                    switches.Add("-sDEVICE=mswinpr2");
                    switches.Add("-sOutputFile=%printer%" + Utilities.GetConfiguration("PrinterName").Trim());
                    switches.Add("-dNumCopies=1");
                    switches.Add(rutaArchivo);
                    processor.StartProcessing(switches.ToArray(), null);
                }
            }
            catch (Exception ex)
            {
                Mensaje(ex.Message);
            }
        }


        void Mensaje(string mensaje)
        {
            utilities.FillLogError(mensaje, "Descarga Certificado");
        }

        public void ImprimirComprobante(string Estado)
        {
            print.Cedula = Utilities.PayerData.BuyerIdentification;
            print.Telefono = Utilities.PayerData.Phone;
            print.FechaPago = DateTime.Now;
            print.Nombre = Utilities.PayerData.FullNameBuyer;
            print.Referencia = Utilities.IDTransactionDB.ToString();
            print.Valor = Utilities.ValueToPay;
            print.Estado = Estado;
            print.ValorDevuelto = Utilities.ValueReturned;
            print.IDCompra = Utilities.BuyID;
            print.Tramite = "Certificados Electrónicos";
            print.ImprimirComprobante();
        }
        #endregion
    }

        public class FileName
    {
        public string IdCertificado { get; set; }
        public string matricula { get; set; }
        public string matriculaest { get; set; }
        public string tpcm { get; set; }
        public int Copia { get; set; }
    }
}
