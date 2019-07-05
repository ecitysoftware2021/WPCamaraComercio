using System;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;
//using WPCamaraComercio.WCFPayPad;
using static WPCamaraComercio.Objects.ObjectsApi;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPagoCancelado.xaml
    /// </summary>
    public partial class FrmCancelledPayment : Window
    {
        #region References
        CamaraComercio camaraComercio;
        Api api;
        private TransactionDetails transactionDetails;
        private FrmLoading frmLoading;
        //ServicePayPadClient WCFPayPadInsert;
        private LogErrorGeneral log;
        private Utilities utilities;
        private NavigationService navigationService;
        #endregion

        #region LoadMethods
        public FrmCancelledPayment(decimal valueReturn)
        {
            InitializeComponent();
            camaraComercio = new CamaraComercio();
            api = new Api();
            transactionDetails = new TransactionDetails();
            frmLoading = new FrmLoading();
            //WCFPayPadInsert = new ServicePayPadClient();
            log = new LogErrorGeneral();
            utilities = new Utilities();
            navigationService = new NavigationService(this);
            lblValue.Content = string.Format("{0:C0}", valueReturn);
            Utilities.control.StartValues();
            ReturnMoney(valueReturn);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Método que se encarga de devolver el dinero ya sea por que se canceló la transacción o por que hay valor sobrante
        /// </summary>
        /// <param name="returnValue">valor a devolver</param>
        private void ReturnMoney(decimal returnValue)
        {
            try
            {
                Utilities.ValueReturned = returnValue;

                Utilities.control.callbackTotalOut = totalOut =>
                {
                    EndDispenserMoney(totalOut, 3, false);
                };

                Utilities.control.callbackOut = quiantityOut =>
                {
                    EndDispenserMoney(quiantityOut, 3, false);
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                };

                Utilities.control.StartDispenser(returnValue);
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("EndDispenserMoney", "FrmCancelledPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        /// <summary>
        /// Método usado para finalizar la dispensación de dinero, descontar el dinero en la bd y 
        /// actualizar el estado de la transacción
        /// </summary>
        /// <param name="quiantity">cantidad dispensada</param>
        /// <param name="state">estado para actualizar la transacción</param>
        private async void EndDispenserMoney(decimal quiantity, int stateTrans = 2, bool state = true)
        {
            try
            {
                Utilities.ValueDelivery = (long)quiantity;
                await Task.Run(() =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                });
                var resp = await AdminPaypad.UpdateTransaction(Utilities.IDTransactionDB, Utilities.EnterTotal, 3, Utilities.ValueDelivery);

                transactionDetails.Description = Utilities.control.LogMessage;
                RequestApi requestApi = new RequestApi
                {
                    Data = transactionDetails
                };
                //var response = await api.GetResponse(requestApi, "SaveTransactionDetail");
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    Utilities.Loading(frmLoading, false, this);
                }));

                if (state)
                {
                    Utilities.CrearLogTransactional(Utilities.log);
                    CreateLog();
                    camaraComercio.ImprimirComprobante("Cancelada");
                    Utilities.GoToInicial();
                }
                else
                {
                    Utilities.CrearLogTransactional(Utilities.log);
                    CreateLog();
                    camaraComercio.ImprimirComprobante("Cancelada");
                    Utilities.GoToInicial();
                }
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("EndDispenserMoney", "FrmCancelledPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }

        private void CreateLog()
        {
            try
            {
                log.IdTransaction = Utilities.IDTransactionDB;
                log.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                log.Description = "Se actualiza la transacción y se deja en estado Cancelada";
                log.ValuePay = Utilities.ValueToPay;
                log.IDCorresponsal = int.Parse(Utilities.GetConfiguration("IDCorresponsal"));
                log.State = "Cancelada";
                Utilities.SaveLogTransactions(log, "LogTransacciones\\Canceladas");
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("CreateLog", "FrmCancelledPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }
        #endregion
    }
}
