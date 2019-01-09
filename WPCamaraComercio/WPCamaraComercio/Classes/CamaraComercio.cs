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

        Datos datos = new Datos();

        Dictionary<string, string> objResponse = new Dictionary<string, string>();

        //TblTransaccion objTransaccion = new TblTransaccion();

        //TblCertificadosXTransaccion objCertificados = new TblCertificadosXTransaccion();

        private string FileName { get; set; }

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
                Utilities.ReferenciaPago = CLSUtil.IDTransaccionDB.ToString();
                datos.AutorizaEnvioEmail = CLSUtil.AutorizaEnvioEmail;
                datos.AutorizaEnvioSMS = CLSUtil.AutorizaEnvioSMS;
                datos.CodigoDepartamentoComprador = CLSUtil.CodigoDepartamentoComprador;
                datos.CodigoMunicipioComprador = CLSUtil.CodigoMunicipioComprador;
                datos.CodigoPaisComprador = CLSUtil.CodigoPaisComprador;
                datos.DireccionComprador = CLSUtil.DireccionComprador;
                datos.EmailComprador = CLSUtil.CorreoElectronico;
                datos.IdentificacionComprador = CLSUtil.IdentificacionComprador;
                datos.MunicipioComprador = CLSUtil.MunicipioComprador;
                datos.NombreComprador = CLSUtil.NombreComprador;
                datos.PlataformaCliente = CLSUtil.PlataformaCliente;
                datos.PrimerApellidoComprador = CLSUtil.PrimerApellidoComprador;
                datos.PrimerNombreComprador = CLSUtil.PrimerNombreComprador;
                datos.ReferenciaPago = CLSUtil.ReferenciaPago;
                datos.SegundoApellidoComprador = CLSUtil.SegundoApellidoComprador;
                datos.SegundoNombreComprador = CLSUtil.SegundoNombreComprador;
                datos.TelefonoComprador = CLSUtil.Telefono;
                datos.TipoComprador = CLSUtil.TipoComprador;
                datos.TipoIdentificacionComprador = CLSUtil.TipoIdentificacionComprador;
                datos.ValorCompra = decimal.Parse(CLSUtil.ValorPagar.ToString());
                datos.Certificados = CLSUtil.ListCertificados.ToArray();
                objResponse = WCFCamara.SendPayInformation(objDatos);
                objResponse.TryGetValue("IDCompra", out idCompra);
                utilities.LlenarLogError(idCompra, "Resultado al Confirmar la Compra");
                IDCompra = idCompra;
                utilities.IDCompra = idCompra;
                return IDCompra;
            }
            catch (Exception ex)
            {
                Utilities.EstadoImpresion = false;
                LlenarLogError(ex.Message, "Metodo ConfirmarCompra");
            }
            return string.Empty;
        }

        void LlenarLogError(string error, string operacion)
        {
            logError.Add(new LogError
            {
                Fecha = DateTime.Now,
                IDTrsansaccion = Utilities.IDTransaccionDB,
                Operacion = operacion,
                Error = error
            });
            utilities.CrearLogErrores(logError);
            logError.Clear();
        }

        public bool ListCertificadosiMPORT()
        {
            try
            {
                PrinterName = Utilities.GetConfiguration("PrinterName");
                foreach (var item in Utilities.ListCertificados)
                {
                    CLSDatosCertificado datosCertificado = new CLSDatosCertificado();
                    datosCertificado.IdCertificado = item.IdCertificado;
                    datosCertificado.idcompra = utilities.IDCompra;
                    datosCertificado.matricula = item.matricula;
                    datosCertificado.matriculaest = item.MatriculaEst;
                    datosCertificado.referenciaPago = Utilities.IDTransaccionDB.ToString();
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
                        utilities.LlenarLogError(urlArchivo, $"URL del Certificado {datosCertificado.copia}");
                        path = GuardarArchivo(urlArchivo, nombreArchivo);
                        LRutasCertificados.Add(path);
                    }
                }
                if (Utilities.EstadoImpresion)
                {
                    foreach (var item in LRutasCertificados)
                    {
                        Imprimir(item);
                    }
                }
                return Utilities.EstadoImpresion;
            }
            catch (Exception ex)
            {
                Mensaje(ex.Message);
                Utilities.EstadoImpresion = false;
            }
            return Utilities.EstadoImpresion;
        }

        public string GuardarArchivo(string PatchFile, FileName nombreArchivo)
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
                utilities.LlenarLogError(PatchFile, "Contenido de PatchFile");
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
                        Utilities.EstadoImpresion = false;
                    }
                }
                else
                {
                    Mensaje("PDF Null");
                    Utilities.EstadoImpresion = false;
                }
            }
            catch (Exception ex)
            {
                Mensaje(ex.Message);
            }
            return path;
        }

        private void Imprimir(string rutaArchivo)
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
            utilities.LlenarLogError(mensaje, "Descarga Certificado");

        }

        //public void ImprimirComprobante(string Estado)
        //{
        //    print.Cedula = Utilities.IdentificacionComprador;
        //    print.FechaPago = DateTime.Now;
        //    print.Nombre = string.Concat(Utilities.PrimerNombreComprador, " ", Utilities.PrimerApellidoComprador);
        //    print.Referencia = Utilities.IDTransaccionDB.ToString();
        //    print.Valor = Utilities.ValorPagar;
        //    print.Estado = Estado;
        //    print.ValorDevuelto = Utilities.ValorDevolver;
        //    print.IDCompra = IDCompra;
        //    print.Tramite = "Certificados Electrónicos";
        //    print.Logo = Path.Combine(Directory.GetCurrentDirectory(), @"LogoComprobante\LCamaraComercio.png");
        //    print.ImprimirComprobante();
        //    InsertTransaccionLocalDB();
        //}

        //private void InsertTransaccionLocalDB()
        //{
        //    try
        //    {
        //        using (var conexion = new DB_CamaraCMEntities())
        //        {
        //            objTransaccion.FechaTransaccion = DateTime.Now;
        //            objTransaccion.IDCompra = IDCompra;
        //            objTransaccion.Valor = objDatos.ValorCompra;
        //            conexion.TblTransaccions.Add(objTransaccion);
        //            conexion.SaveChanges();
        //            CLSUtil.idTransaccion = objTransaccion.IDTransaccion;
        //        }
        //        using (var conexion = new DB_CamaraCMEntities())
        //        {
        //            foreach (var item in objDatos.Certificados)
        //            {
        //                objCertificados.CantidadCertificados = int.Parse(item.NumeroCertificados);
        //                objCertificados.CertificadosImpresos = 0;
        //                objCertificados.IDCertificado = int.Parse(item.IdCertificado);
        //                objCertificados.MatriculaEst = item.MatriculaEst;
        //                objCertificados.Matricula = item.matricula;
        //                objCertificados.IDTransaccion = CLSUtil.idTransaccion;
        //                objCertificados.Tpcm = item.tpcm;
        //                conexion.TblCertificadosXTransaccions.Add(objCertificados);
        //                conexion.SaveChanges();
        //            }
        //        }
        //    }
        //    catch { }
        //}

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
}
