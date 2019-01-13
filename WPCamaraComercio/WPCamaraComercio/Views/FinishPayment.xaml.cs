using System;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Service;
using WPCamaraComercio.WCFPayPad;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FinishPayment.xaml
    /// </summary>
    public partial class FinishPayment : Window
    {
        #region Referencias
        Utilities utilities = new Utilities();
        CamaraComercio camaraComercio = new CamaraComercio();
        ServicePayPadClient WCFPayPadInsert = new ServicePayPadClient();
        NavigationService navigationService;
        private int count = 0;
        private LogErrorGeneral logError;
        decimal enterValue = 0;
        decimal returnValue = 0;
        #endregion

        public FinishPayment(decimal _enterValue, decimal _returnValue)
        {
            InitializeComponent();
            this.enterValue = _enterValue;
            this.returnValue = _returnValue;
            logError = new LogErrorGeneral();
            navigationService = new NavigationService(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PDFBYTE();
        }

        public Task PDFBYTE()
        {
            var t = Task.Run(() =>
            {
                return camaraComercio.ListCertificadosiMPORT();
            });

            var c = t.ContinueWith((antecedent) =>
            {
                if (t.Status == TaskStatus.RanToCompletion)
                {
                    if (antecedent.Result)
                    {
                        utilities.UpdateTransaction(enterValue, 2, returnValue, Utilities.BuyID);
                        camaraComercio.ImprimirComprobante("Aprobada");
                        Utilities.GoToInicial();
                    }
                    else
                    {
                        utilities.UpdateTransaction(enterValue, 3, returnValue, Utilities.BuyID);
                        Dispatcher.BeginInvoke((Action)delegate
                        {
                            FrmModal modal = new FrmModal(string.Concat("No se pudo imprimir el certificado.", Environment.NewLine,
                            "Se cancelará la transacción y se le devolverá el dinero.", Environment.NewLine,
                        "Comuniquese con servicio al cliente o diríjase a las taquillas."), this);
                            modal.ShowDialog();
                            if (modal.DialogResult.Value)
                            {
                                navigationService.NavigationTo("FrmCancelledPayment");
                            }
                        });
                    }
                }
                else if (t.Status == TaskStatus.Faulted)
                {
                    //objUtil.Exception(t.Exception.GetBaseException().Message);
                }
            });
            return t;
        }
    }
}
