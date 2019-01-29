using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.WCFCamaraComercio;

namespace WPCamaraComercio.Service
{
    public class WCFServices
    {
        private WCFCamaraComercioClient WCFCamara;

        private Response response;

        public WCFServices()
        {
            WCFCamara = new WCFCamaraComercioClient();

            response = new Response();
        }

        public Task<Response> ConsultInformation(string searchText, int type)
        {
            Task<Response> task = null;

            task = Task.Run(() =>
            {
                Response response = new Response();
                try
                {
                    tipo_busqueda searchType;

                    if (type == 2)
                    {
                        searchType = tipo_busqueda.Nit;
                    }
                    else
                    {
                        searchType = tipo_busqueda.Nombre;
                    }

                    var r = WCFCamara.GetGeneralInformation(searchText, searchType);
                    if (r != null)
                    {
                        response.IsSuccess = true;
                        response.Result = r;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Result = null;
                    response.Message = ex.Message;
                }

                return response;
            });

            return task;
        }

        public Task<Response> ConsultDetailMerchant(string enrollment, string tpcm)
        {
            Task<Response> task = null;

            task = Task.Run(() =>
            {
                Response response = new Response();
                try
                {
                    var r = WCFCamara.GetDetalleComerciante(new PeticionDetalle
                    {
                        Matricula = enrollment,
                        Tpcm = tpcm
                    });

                    if (r != null)
                    {
                        response.IsSuccess = true;
                        response.Result = r;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Result = null;
                    response.Message = ex.Message;
                }

                return response;
            });

            return task;
        }

        public Task<Response> SendPayInformation(Datos data)
        {
            Task<Response> task = null;

            task = Task.Run(() =>
            {
                Response response = new Response();
                try
                {
                    var r = WCFCamara.SendPayInformation(data);
                    if (r != null)
                    {
                        response.IsSuccess = true;
                        response.Result = r;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Result = null;
                    response.Message = ex.Message;
                }

                return response;
            });

            return task;
        }

        public Task<Response> GetCertifiedString(CLSDatosCertificado data)
        {
            Task<Response> task = null;

            task = Task.Run(() =>
            {
                Response response = new Response();
                try
                {
                    var r = WCFCamara.GetCertifiedString(data);
                    if (r != null)
                    {
                        response.IsSuccess = true;
                        response.Result = r;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Result = null;
                    response.Message = ex.Message;
                }

                return response;
            });

            return task;
        }

        public Task<Response> InsertPayerDBCM(DatosPagador data)
        {
            Task<Response> task = null;

            task = Task.Run(() =>
            {
                Response response = new Response();
                try
                {
                    var r = WCFCamara.SavePayerDBCM(data);
                    if (r != null)
                    {
                        response.IsSuccess = true;
                        response.Result = r;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Result = null;
                    response.Message = ex.Message;
                }

                return response;
            });

            return task;
        }

        public Task<Response> InserTransactionDBCM(DatosTransaccionDBCamaraCM data)
        {
            Task<Response> task = null;

            task = Task.Run(() =>
            {
                Response response = new Response();
                try
                {
                    var r = WCFCamara.SaveTransactionDBCM(data);
                    if (r != null)
                    {
                        response.IsSuccess = true;
                        response.Result = r;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Result = null;
                    response.Message = ex.Message;
                }

                return response;
            });

            return task;
        }
    }
}
