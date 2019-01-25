using System;
using System.Windows;
using System.Windows.Input;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmModal.xaml
    /// </summary>
    public partial class FrmModal : Window
    {
        #region References
        Window w;
        #endregion

        #region LoadMethods
        public FrmModal(string mensaje, Window w, bool state = false)
        {
            InitializeComponent();
            LblMessage.Text = mensaje;
            this.w = w;
            if (this.w != null)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    w.Opacity = 0.6;
                });
            }
        }

        #endregion

        #region Events
        private void Image_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            if (this.w != null)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    w.Opacity = 1;
                });
            }
            DialogResult = true;
        }
        #endregion
    }
}
