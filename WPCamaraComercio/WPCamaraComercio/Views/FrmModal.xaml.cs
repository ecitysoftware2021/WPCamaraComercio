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
        #region "Referencias"
        private Utilities utilities;
        #endregion

        #region "Constructor
        public FrmModal(string mensaje)
        {
            InitializeComponent();
            LblMessage.Text = mensaje;
            utilities = new Utilities();
        }
        #endregion

        #region "Button"
        private void Image_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception ex)
            {
                utilities.saveLogError("Image_PreviewStylusDown", "FrmModal", ex.ToString());
            }
        }
        #endregion
    }
}
