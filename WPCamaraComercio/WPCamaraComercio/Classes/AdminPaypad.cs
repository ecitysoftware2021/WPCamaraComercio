using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.Service;
using static WPCamaraComercio.Objects.ObjectsApi;

namespace WPCamaraComercio.Classes
{
    public class AdminPaypad
    {
        public static Api apilocal = new Api();

        /// <summary>
        /// Método encargado de crear la transacciòn en bd y retornar el id de esta misma   
        /// </summary>
        /// <param name="Amount">Cantdiad a pagaar o retirar</param>
        public static async Task<bool> CreateTransaction(string identificacion, string nombre, string apellido, decimal telefono)
        {
            try
            {
                PAYER payer = new PAYER
                {
                    IDENTIFICATION = identificacion,
                    NAME = nombre,
                    LAST_NAME = apellido,
                    PHONE = telefono,
                };

                var resultPayer = await apilocal.GetResponse(new RequestApi
                {
                    Data = payer
                }, "SavePayer");

                if (int.Parse(resultPayer.Data.ToString()) < 0 || string.IsNullOrEmpty(resultPayer.Data.ToString()))
                {
                    return false;
                }

                var data = new TRANSACTION
                {
                    TYPE_TRANSACTION_ID = 19,
                    PAYER_ID = int.Parse(resultPayer.Data.ToString()),
                    STATE_TRANSACTION_ID = 1,
                    TOTAL_AMOUNT = Utilities.ValueToPay,
                    TRANSACTION_ID = 0,
                    RETURN_AMOUNT = 0,
                    INCOME_AMOUNT = 0,
                    PAYPAD_ID = 0,
                    DATE_BEGIN = DateTime.Now,
                    STATE_NOTIFICATION = 0,
                    STATE = 0,
                    DESCRIPTION = "Transaccion iniciada"
                };

                foreach (var item in Utilities.ListCertificates)
                {
                    var details = new TRANSACTION_DESCRIPTION
                    {
                        AMOUNT = item.EstablishCertificate.CertificateCost,
                        TRANSACTION_ID = 0,
                        REFERENCE = $"Matricula: {item.matricula}",
                        OBSERVATION = item.EstablishCertificate.NombreCertificado,
                        TRANSACTION_DESCRIPTION_ID = 0,
                        STATE = true,
                    };

                    data.TRANSACTION_DESCRIPTION.Add(details);
                }

                var response = await apilocal.GetResponse(new RequestApi
                {
                    Data = data
                }, "SaveTransaction");

                if (response != null)
                {
                    if (response.CodeError == 200)
                    {
                        Utilities.IDTransactionDB = int.Parse(response.Data.ToString());
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Mètodo para actualizar la transacción en base de datos por el estado que coorresponda
        /// </summary>
        /// <param name="Enter">Valor ingresado. Sólo aplica para pago</param>
        /// <param name="state">Estaado por el cual se actualizará</param>
        /// <param name="Return">Valor a devolver, por defecto es 0 ya que hay transacciones donde no hay que devolver</param>
        /// <returns>Retorna un verdadero o un falso dependiendo el resultado del update</returns>
        public static async Task<bool> UpdateTransaction(int idTrans, decimal Enter, int state, decimal Return = 0)
        {
            try
            {
                TRANSACTION Transaction = new TRANSACTION
                {
                    STATE_TRANSACTION_ID = state,
                    INCOME_AMOUNT = Enter,
                    RETURN_AMOUNT = Return,
                    DATE_END = DateTime.Now,
                    DATE_BEGIN = DateTime.Now,
                    DESCRIPTION = "Finalizando",
                    TRANSACTION_ID = idTrans,
                    TRANSACTION_REFERENCE = Utilities.BuyID,
                    TOTAL_AMOUNT = Utilities.ValueToPay,
                };

                var response = await apilocal.GetResponse(new RequestApi
                {
                    Data = Transaction
                }, "UpdateTransaction");

                if (response != null)
                {
                    if (response.CodeError == 200)
                    {
                        return true;
                    }
                    return false;
                }

                return false;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

    }
}
