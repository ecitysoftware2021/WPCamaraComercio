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
using System.Windows.Shapes;
using WPCamaraComercio.Classes;

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
            Dispatcher.BeginInvoke((Action)delegate
            {
                w.Opacity = 0.6;
            });
        }

        public FrmModal(string mensaje, bool state = false)
        {
            InitializeComponent();
            LblMessage.Text = mensaje;
            Dispatcher.BeginInvoke((Action)delegate
            {
                Opacity = 0.6;
            });
        }
        #endregion

        #region Events
        private void Image_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            if (w !=  null)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    w.Opacity = 1;
                });
                DialogResult = true;
            }
            else
            {
                DialogResult = true;
            }
        }
        #endregion
    }
}
