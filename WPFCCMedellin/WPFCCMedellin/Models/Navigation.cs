using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Services.Object;
using WPFCCMedellin.UserControls;
using WPFCCMedellin.UserControls.Administrator;
using WPFCCMedellin.UserControls.Administrator.CancelBuy;

namespace WPFCCMedellin.Models
{
    public class Navigation : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private UserControl _view;

        public UserControl View
        {
            get
            {
                return _view;
            }
            set
            {
                _view = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(View)));
            }
        }

        public void Navigate(UserControlView newWindow, bool initTimer = false, object data = null, object complement = null) => Application.Current.Dispatcher.Invoke((Action)delegate
        {
            try
            {
                switch (newWindow)
                {
                    case UserControlView.Main:
                        View = data != null ? new MainUserControl((bool)data) : new MainUserControl();
                        break;
                    case UserControlView.Consult:
                        View = new ConsultUserControl();
                        break;
                    case UserControlView.PaySuccess:
                        View = new SussesUserControl((Transaction)data);
                        break;
                    case UserControlView.Pay:
                        View = new PaymentUserControl((Transaction)data);
                        break;
                    case UserControlView.ReturnMony:
                        View = new ReturnMonyUserControl((Transaction)data);
                        break;
                    case UserControlView.Login:
                        View = new LoginAdministratorUserControl((ETypeAdministrator)data);
                        break;
                    case UserControlView.Config:
                        View = new ConfigurateUserControl();
                        break;
                    case UserControlView.Admin:
                        View = new AdministratorUserControl((PaypadOperationControl)data, (ETypeAdministrator)complement);
                        break;
                    case UserControlView.Certificates:
                        View = new CertificatesUserControl((Transaction)data);
                        break;
                    case UserControlView.Payer:
                        View = new PayerUserControl((Transaction)data);
                        break;
                    case UserControlView.PrintFile:
                        View = new PrintFileUserControl((Transaction)data);
                        break;

                    case UserControlView.LoginCancelTrans:
                        View = new LoginCancelTransUserControl();
                        break;
                    case UserControlView.CancelTransaction:
                        View = new CancelTransactionUserControl();
                        break;
                }

                TimerService.Close();


                if (initTimer)
                {
                    TimerService.CallBackTimerOut = response =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)delegate
                        {
                            WPKeyboard.Keyboard.CloseKeyboard(View);
                            View = new MainUserControl();
                        });
                        GC.Collect();
                    };

                    TimerService.Start(int.Parse(Utilities.GetConfiguration("DurationView")));
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "Navigate", ex);
            }
            GC.Collect();
        });
    }
}