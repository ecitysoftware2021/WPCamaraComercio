using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;

namespace WPFCCMedellin.Windows
{
    /// <summary>
    /// Lógica de interacción para ModalWindow.xaml
    /// </summary>
    public partial class ModalWindow : Window
    {
        private ModalModel modal;

        public ModalWindow(ModalModel modal)
        {
            InitializeComponent();

            this.modal = modal;

            this.DataContext = this.modal;

            ConfigureModal();
        }

        private void ConfigureModal()
        {
            try
            {
                if (this.modal.TypeModal == EModalType.Preload)
                {
                    this.BtnOk.Visibility = Visibility.Hidden;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Visible;
                }
                else if (this.modal.TypeModal == EModalType.NotExistAccount)
                {
                    this.BtnOk.Visibility = Visibility.Visible;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Hidden;
                    //InitTimer();
                }
                else if (this.modal.TypeModal == EModalType.MaxAmount || this.modal.TypeModal == EModalType.Error)
                {
                    this.BtnOk.Visibility = Visibility.Hidden;
                    this.BtnYes.Visibility = Visibility.Hidden;
                    this.BtnNo.Visibility = Visibility.Hidden;
                    this.LblMessageTouch.Visibility = Visibility.Visible;
                    GifLoadder.Visibility = Visibility.Hidden;
                    //InitTimer();
                }
                else if (this.modal.TypeModal == EModalType.Information || this.modal.TypeModal == EModalType.NoPaper)
                {
                    this.BtnOk.Visibility = Visibility.Hidden;
                    this.BtnYes.Visibility = Visibility.Visible;
                    this.BtnNo.Visibility = Visibility.Visible;
                    this.LblMessageTouch.Visibility = Visibility.Hidden;
                    GifLoadder.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void Grid_TouchDown(object sender, TouchEventArgs e)
        {
            if (this.modal.TypeModal == EModalType.MaxAmount || this.modal.TypeModal == EModalType.Error)
            {
                this.DialogResult = true;
            }
        }

        private void BtnOk_TouchDown(object sender, TouchEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnYes_TouchDown(object sender, TouchEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnNo_TouchDown(object sender, TouchEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

