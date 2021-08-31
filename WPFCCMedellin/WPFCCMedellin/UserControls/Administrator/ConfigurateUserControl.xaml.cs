using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
            try
            {
                InitializeComponent();

                if (init == null)
                {
                    init = new AdminPayPlus();
                }

                txt_description.DataContext = init;

                Initial();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
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
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private async void ProccesResult(bool result)
        {
            try
            {
                if (AdminPayPlus.DataPayPlus.StateUpdate)
                {
                    Utilities.ShowModal(MessageResource.UpdateAplication, EModalType.Error, false);
                    Utilities.UpdateApp();
                }
                else if (AdminPayPlus.DataPayPlus.StateBalanece)
                {
                    Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Balancing);
                }
                else if (AdminPayPlus.DataPayPlus.StateUpload)
                {
                    Utilities.navigator.Navigate(UserControlView.Login, false, ETypeAdministrator.Upload);
                }
                else
                {
                    //Utilities.navigator.Navigate(UserControlView.Main);
                    Finish(result);
                }
            }
            catch (Exception ex)
            {
                Utilities.ShowModal(string.Concat(init.DescriptionStatusPayPlus, " ", MessageResource.NoService), EModalType.Error, false);
                Initial();
            }
        }

        private void Finish(bool state)
        {
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                if (state)
                {
                    Utilities.navigator.Navigate(UserControlView.Main);
                }
                else
                {
                    Utilities.ShowModal(string.Concat(init.DescriptionStatusPayPlus, " ", MessageResource.NoService), EModalType.Error, false);
                    Initial();
                }
            });
        }
    }
}
