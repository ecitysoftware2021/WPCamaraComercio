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
        public LoginCancelTransUserControl()
        {
            InitializeComponent();
        }

        #region Eventos
        private void txtPassword_TextChanged(object sender, RoutedEventArgs e)
        {
            int lenght = txtPassword.Password.Length;
            if (lenght > 15)
            {
                txtPassword.Password = txtPassword.Password.Remove(txtPassword.Password.Length - 1);
            }
        }

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
        #endregion

        #region Metodos
        private bool existInEcity()
        {
            try
            {
                string user = txtDocument.Text;
                string pass = txtPassword.Password;

                bool correct = true;

                var t = Task.Run(() =>
                {
                    return Api.Login(user, pass).Result;
                });

                var c = t.ContinueWith((antecedent) => Dispatcher.BeginInvoke((Action)delegate
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        if (antecedent.Result == null)
                        {
                            MessageBox.Show("Usuario y/o Contraseña incorrectos, intente de nuevo", "Usuario Incorrecto", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtPassword.Password = string.Empty;
                            correct = false;
                        }
                    }
                    else if (t.Status == TaskStatus.Faulted)
                    {
                        MessageBox.Show(t.Exception.GetBaseException().Message);
                        correct = false;
                    }
                }));
                if (correct)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
