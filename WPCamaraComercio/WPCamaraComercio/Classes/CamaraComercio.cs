using Ghostscript.NET;
using Ghostscript.NET.Processor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;
using WPCamaraComercio.WCFCamaraComercio;

namespace WPCamaraComercio.Classes
{
    public class CamaraComercio
    {
        Certificado Certificate = new Certificado();

        Print print = new Print();

        LocalPrintServer server = new LocalPrintServer();

        Utilities utilities = new Utilities();

        List<DetailCartificate> ListDetailCert = new List<DetailCartificate>();

        WCFCamaraComercioClient WCFCamara = new WCFCamaraComercioClient();

        WCFServices service = new WCFServices();

        Datos datos = new Datos();

        Dictionary<string, string> responseDic = new Dictionary<string, string>();

        private string FileName { get; set; }

        private bool printState { get; set; }

        private static string IDCompra { get; set; }

        public static string Delimitador { get { return "-"; } }

        private string DirectoryFile { get; set; }

        private string path { get; set; }

        private byte[] bytePDF { get; set; }

        public static string PrinterName { get; set; }

        private List<string> LRutasCertificados = new List<string>();

        List<LogError> logError = new List<LogError>();

        public string ConfirmarCompra()
        {
            try
            {
                string idCompra = string.Empty;
                //Utilities.ReferenciaPago = Utilities.IDTransactionDB.ToString();
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
                Task.Run(async () =>
                {
                    var response = await service.SendPayInformation(datos);

                    if (response.IsSuccess)
                    {
                        responseDic = (Dictionary<string, string>)response.Result;
                    }
                });
                responseDic.TryGetValue("IDCompra", out idCompra);
                utilities.FillLogError(idCompra, "Resultado al Confirmar la Compra");
                IDCompra = idCompra;
                return IDCompra;
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

        public bool ListCertificadosiMPORT()
        {
            try
            {
                PrinterName = Utilities.GetConfiguration("PrinterName");
                foreach (var item in Utilities.ListCertificates)
                {
                    CLSDatosCertificado datosCertificado = new CLSDatosCertificado();
                    datosCertificado.IdCertificado = item.IdCertificado;
                    datosCertificado.idcompra = IDCompra;
                    datosCertificado.matricula = item.matricula;
                    datosCertificado.matriculaest = item.MatriculaEst;
                    datosCertificado.referenciaPago = Utilities.IDTransactionDB.ToString();
                    datosCertificado.tpcm = item.tpcm;

                    bytePDF = null;

                    for (int i = 0; i < int.Parse(item.NumeroCertificados); i++)
                    {
                        datosCertificado.copia = (i + 1).ToString();
                        string urlArchivo = WCFCamara.GetCertifiedString(datosCertificado);
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
            return printState;
        }

        public string SaveFile(string PatchFile, FileName nombreArchivo)
        {
            try
            {
                FileName = string.Concat(
                    IDCompra,
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
                DirectoryFile = Utilities.GetConfiguration("DirectoryFile");
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

        private void Print(string rutaArchivo)
        {
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
            print.FechaPago = DateTime.Now;
            print.Nombre = string.Concat(Utilities.PayerData.FirstNameBuyer, " ", Utilities.PayerData.LastNameBuyer);
            print.Referencia = Utilities.IDTransactionDB.ToString();
            print.Valor = Utilities.ValueToPay;
            print.Estado = Estado;
            print.ValorDevuelto = Utilities.ValorDevolver;
            print.IDCompra = IDCompra;
            print.Tramite = "Certificados Electrónicos";
            print.Logo = Path.Combine(Directory.GetCurrentDirectory(), @"LogoComprobante\LCamaraComercio.png");
            print.ImprimirComprobante();
        }
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
