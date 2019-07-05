using System;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Service;
//using WPCamaraComercio.WCFPayPad;

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
        //ServicePayPadClient WCFPayPadInsert = new ServicePayPadClient();
        NavigationService navigationService;
        private int count = 0;
        private LogErrorGeneral log;
        decimal enterValue = 0;
        decimal returnValue = 0;
        #endregion

        #region LoadMethods
        public FinishPayment(decimal _enterValue, decimal _returnValue)
        {
            InitializeComponent();
            this.enterValue = _enterValue;
            this.returnValue = _returnValue;
            log = new LogErrorGeneral();
            navigationService = new NavigationService(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PDFBYTE();
        }
        #endregion

        #region Methods
        public Task PDFBYTE()
        {
            try
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
                            var state = AdminPaypad.UpdateTransaction(Utilities.IDTransactionDB, this.enterValue, 2, this.returnValue);
                            Utilities.CrearLogTransactional(Utilities.log);
                            CreateLog();
                            camaraComercio.ImprimirComprobante("Aprobada");

                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                FrmInitial initial = new FrmInitial();
                                initial.Show();
                                this.Close();
                            });
                        }
                        else
                        {
                            Dispatcher.BeginInvoke((Action)delegate
                            {
                                FrmModal modal = new FrmModal(string.Concat("No se pudo imprimir el certificado.", Environment.NewLine,
                                        "Se cancelará la transacción y se le devolverá el dinero.", Environment.NewLine,
                                        "Comuniquese con servicio al cliente o diríjase a las taquillas."), this);
                                modal.ShowDialog();
                                if (modal.DialogResult.Value)
                                {
                                    FrmCancelledPayment cancel = new FrmCancelledPayment(enterValue);
                                    cancel.Show();
                                    this.Close();
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
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("PDFBYTE", "FinishPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
                return null;
            }
        }

        private void CreateLog()
        {
            try
            {
                log.IdTransaction = Utilities.IDTransactionDB;
                log.Date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                log.Description = "Se actualiza la transacción y se deja en estado Aprobada";
                log.ValuePay = Utilities.ValueToPay;
                log.IDCorresponsal = int.Parse(Utilities.GetConfiguration("IDCorresponsal"));
                log.State = "Aprobada";
                Utilities.SaveLogTransactions(log, "LogTransacciones\\Aprobadas");
            }
            catch (Exception ex)
            {
                utilities.SaveLogErrorMethods("CreateLog", "FinishPayment", ex.ToString());
                navigationService.NavigatorModal("Lo sentimos ha ocurrido un error, intente mas tarde.");
            }
        }
        #endregion
    }
}
