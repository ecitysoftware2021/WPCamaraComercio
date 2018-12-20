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
        WCFCamaraComercioClient WCFCamara = new WCFCamaraComercioClient();
        public Task<Response> ConsultInformation(string searchText, tipo_busqueda searchType)
        {
            Task<Response> task = null;
            task = Task.Run(() =>
            {
                Response response = new Response();

                try
                {
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

        public Task<Response> ConsultDetailMerchant(PeticionDetalle detailPetition)
        {
            Task<Response> task = null;
            task = Task.Run(() =>
            {
                Response response = new Response();

                try
                {
                    var r = WCFCamara.GetDetalleComerciante(detailPetition);
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
