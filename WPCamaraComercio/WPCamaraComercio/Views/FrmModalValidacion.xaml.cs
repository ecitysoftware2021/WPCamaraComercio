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
    /// Lógica de interacción para FrmModalValidacion.xaml
    /// </summary>
    public partial class FrmModalValidacion : Window
    {
        Utilities utilities;

        #region "Constructor"
        public FrmModalValidacion(string message)
        {
            InitializeComponent();
            LblMessage.Text = message;
            utilities = new Utilities();
        }
        #endregion

        /// <summary>
        /// Método de cancelación de la información
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgCancel_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                DialogResult = false;
            }
            catch (Exception ex)
            {
                utilities.saveLogError("imgAceptar_PreviewStylusDown", "ModalValidation", ex.ToString());
            }
        }

        /// <summary>
        /// Método de la aceptación de la información
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgAceptar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception ex)
            {
                utilities.saveLogError("imgAceptar_PreviewStylusDown", "ModalValidation", ex.ToString());
            }
        }
    }
}