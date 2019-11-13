using System;
using System.Windows.Controls;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Resources;

namespace WPFCCMedellin.UserControls.Administrator
{
    /// <summary>
    /// Lógica de interacción para ConfigurateUserControl.xaml
    /// </summary>
    public partial class ConfigurateUserControl : UserControl
    {
        private AdminPayPlus init;

        public ConfigurateUserControl()
        {
            InitializeComponent();

            if (init == null)
            {
                init = new AdminPayPlus();
            }

            Initial();
        }

        private async void Initial()
        {
            try
            {
                init.callbackResult = result =>
                {
                    ProccesResult(result);
                };

                init.Start();
            }
            catch (Exception ex)
            {
            }
        }

        private async void ProccesResult(bool result)
        {
            try
            {
                if (AdminPayPlus.DataPayPlus.StateBalanece)
                {
                    Utilities.navigator.Navigate(UserControlView.Login, false, 1);
                }
                else if (AdminPayPlus.DataPayPlus.StateUpload)
                {
                    Utilities.navigator.Navigate(UserControlView.Login, false, 2);
                }
                else
                {
                    if (result)
                    {
                        if (!AdminPayPlus.DataPayPlus.StateBalanece && !AdminPayPlus.DataPayPlus.StateUpload)
                        {
                            Utilities.navigator.Navigate(UserControlView.Main);
                        }
                    }
                    else
                    {
                        Utilities.ShowModal(MessageResource.NoService, EModalType.Error, false);
                        Initial();
                    }
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowModal(MessageResource.NoService, EModalType.Error, false);
                Initial();
            }
        }
    }
}
