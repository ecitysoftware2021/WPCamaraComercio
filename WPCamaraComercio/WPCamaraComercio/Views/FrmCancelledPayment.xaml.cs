using System;
using System.Windows;
using WPCamaraComercio.Classes;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPagoCancelado.xaml
    /// </summary>
    public partial class FrmCancelledPayment : Window
    {
        #region References
        CamaraComercio camaraComercio;
        #endregion

        #region LoadMethods
        public FrmCancelledPayment(decimal valueReturn)
        {
            InitializeComponent();
            camaraComercio = new CamaraComercio();
            lblValue.Content = valueReturn;
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
                Utilities.control.callbackValueOut = valueOut =>
                {
                    if (valueOut > 0)
                    {

                    }
                };

                Utilities.control.callbackTotalOut = totalOut =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                    camaraComercio.ImprimirComprobante("Cancelada");
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Utilities.GoToInicial();
                    });
                };

                Utilities.control.callbackError = error =>
                {
                    Utilities.SaveLogDispenser(ControlPeripherals.log);
                };

                Utilities.control.StartDispenser(returnValue);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        } 
        #endregion
    }
}
