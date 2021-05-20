using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;

namespace WPFCCMedellin.UserControls.Administrator.CancelBuy
{
    /// <summary>
    /// Lógica de interacción para CancelTransactionUserControl.xaml
    /// </summary>
    public partial class CancelTransactionUserControl : UserControl
    {

        private Transaction transaction;
        public CancelTransactionUserControl()
        {
            InitializeComponent();

            transaction = new Transaction();
        }

        private bool checkFields()
        {
            if (txtIdCompra.Text == "" || txtValorCompra.Text == "" || txtReferenciaPago.Text == "")
            {
                return false;
            }

            return true;
        }

        private void txtform_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(true, (TextBox)sender, this);
        }

        private void txtObservaciones_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, (TextBox)sender, this);
        }

        private void txtObservaciones_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lenght = txtObservaciones.Text.Length;
            if (lenght > 20)
            {
                txtObservaciones.Text = txtObservaciones.Text.Remove(txtObservaciones.Text.Length - 1);
            }
            if (checkFields())
            {
                btn_consult.IsEnabled = true;
                btn_consult.Opacity = 1;
            }
        }

        private void txtReferenciaPago_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lenght = txtReferenciaPago.Text.Length;
            if (lenght > 20)
            {
                txtReferenciaPago.Text = txtReferenciaPago.Text.Remove(txtReferenciaPago.Text.Length - 1);
            }
            if (checkFields())
            {
                btn_consult.IsEnabled = true;
                btn_consult.Opacity = 1;
            }
        }

        private void txtValorCompra_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lenght = txtValorCompra.Text.Length;
            if (lenght > 20)
            {
                txtValorCompra.Text = txtValorCompra.Text.Remove(txtValorCompra.Text.Length - 1);
            }
            if (checkFields())
            {
                btn_consult.IsEnabled = true;
                btn_consult.Opacity = 1;
            }
        }

        private void txtIdCompra_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lenght = txtIdCompra.Text.Length;
            if (lenght > 20)
            {
                txtIdCompra.Text = txtIdCompra.Text.Remove(txtIdCompra.Text.Length - 1);
            }
            if (checkFields())
            {
                btn_consult.IsEnabled = true;
                btn_consult.Opacity = 1;
            }
        }

        private void BtnConsult_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (checkFields())
                {
                    transaction.State = ETransactionState.CancelError;

                    transaction.idCompra = int.Parse(txtIdCompra.Text);
                    transaction.valorCompra = decimal.Parse(txtValorCompra.Text);
                    transaction.referenciaPago = txtReferenciaPago.Text;
                    if (txtObservaciones.Text != "")
                    {
                        transaction.observaciones = txtObservaciones.Text;
                    }
                    Task.Run(async () =>
                    {
                        transaction = await AdminPayPlus.ApiIntegration.NotifycCancelTransaction(transaction);
                        Utilities.CloseModal();

                        if (transaction.State == ETransactionState.Cancel)
                        {
                            transaction.IdTransactionAPi = int.Parse(transaction.referenciaPago);

                            Utilities.ShowModal("Transacción cancelada exitosamente", EModalType.Information);
                            
                            AdminPayPlus.UpdateTransaction(transaction);

                            Utilities.navigator.Navigate(UserControlView.Main);
                        }
                        else
                        {
                            Utilities.ShowModal("No se ha podido cancelar la transacción, por favor intentelo más tarde", EModalType.Information);
                        }
                    });
                    Utilities.ShowModal(MessageResource.FinishTransaction, EModalType.Preload);
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void BtnBack_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.LoginCancelTrans);
        }
    }
}
