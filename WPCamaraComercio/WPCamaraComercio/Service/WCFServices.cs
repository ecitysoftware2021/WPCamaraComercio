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

        public async Task<Response> ConsultInformation(string searchText, int type)
        {
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
        }

        public async Task<Response> ConsultDetailMerchant(string enrollment, string tpcm)
        {
            response = new Response();
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
        }

    }
}
