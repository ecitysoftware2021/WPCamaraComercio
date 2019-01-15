using System;
using System.Windows;
using System.Windows.Input;
using WPCamaraComercio.Classes;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmModalValidacion.xaml
    /// </summary>
    public partial class FrmModalValidation : Window
    {
        #region References
        Utilities utilities; 
        #endregion

        #region "Constructor"
        public FrmModalValidation(string message)
        {
            InitializeComponent();
            LblMessage.Text = message;
            utilities = new Utilities();
        }
        #endregion

        #region Methods
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
                //utilities.saveLogError("imgAceptar_PreviewStylusDown", "ModalValidation", ex.ToString());
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
                //utilities.saveLogError("imgAceptar_PreviewStylusDown", "ModalValidation", ex.ToString());
            }
        } 
        #endregion
    }
}