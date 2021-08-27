using System;
using System.Collections.Generic;
using System.Linq;
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
using WPFCCMedellin.Services;

namespace WPFCCMedellin.UserControls.Administrator.CancelBuy
{
    /// <summary>
    /// Lógica de interacción para LoginCancelTransUserControl.xaml
    /// </summary>
    public partial class LoginCancelTransUserControl : UserControl
    {
        private string pass;
        public LoginCancelTransUserControl()
        {
            InitializeComponent();
        }

        #region Eventos

        private void txtDocument_TextChanged(object sender, TextChangedEventArgs e)
        {
            int lenght = txtDocument.Text.Length;
            if (lenght > 20)
            {
                txtDocument.Text = txtDocument.Text.Remove(txtDocument.Text.Length - 1);
            }
        }

        private void txtform_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, (TextBox)sender, this);
        }

        private void BtnConsult_TouchDown(object sender, TouchEventArgs e)
        {
            if (existInEcity())
            {
                Utilities.navigator.Navigate(UserControlView.CancelTransaction);
            }
            else
            {
                Utilities.ShowModal("No se pudo encontrar información del usuario, por favor intente de nuevo.", EModalType.Error);
            }
        }

        private void btn_salir_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.navigator.Navigate(UserControlView.Main);
        }
        #endregion

        #region Metodos
        private bool existInEcity()
        {
            try
            {
                string user = txtDocument.Text;
                string pass = txtPassword.Password;

                var result = Api.Login(user, pass).Result;



                if (result == null)
                {
                    txtPassword.Password = string.Empty;
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        private void txtpass_TouchDown(object sender, TouchEventArgs e)
        {
            Utilities.OpenKeyboard(false, (PasswordBox)sender, this);
        }

        private void txtPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            int lenght = txtPassword.Password.Length;
            if (lenght > 35)
            {
                txtPassword.Password = txtPassword.Password.Remove(txtPassword.Password.Length - 1);
            }
        }
    }
}
