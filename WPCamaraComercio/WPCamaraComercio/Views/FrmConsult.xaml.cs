using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using WPCamaraComercio.Keyboard;
//
namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmConsult.xaml
    /// </summary>
    public partial class FrmConsult : Window
    {
        #region References

        Utilities utilities;
        TouchScreenKeyNumeric numericKey;

        #endregion

        #region LoadMethods

        public FrmConsult()
        {
            InitializeComponent();
            utilities = new Utilities();
        }

        #endregion

        #region Timer

        private void Window_PreviewStylusDown(object sender, StylusDownEventArgs e) => Utilities.time = TimeSpan.Parse(Utilities.Duration);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Utilities.Timer(tbTimer);
        }

        #endregion

        #region HeaderButtons

        private void BtnBack_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.ResetTimer();
            Dispatcher.BeginInvoke((Action)delegate
            {
                FrmMenu menu = new FrmMenu();
                menu.Show();
                this.Close();
            });
            GC.Collect();
        }

        private void BtnExit_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            Utilities.GoToInicial();
        }

        #endregion

        #region Events

        private void Txt_GotFocus(object sender, RoutedEventArgs e) => lblError.Visibility = Visibility.Hidden;

        private void chkIdentification_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            string tag = chkIdentification.Tag.ToString();
            string identificationTag, nameTag, identificationPath, namePath;
            switch (tag)
            {
                case "S":
                    identificationTag = "N";
                    nameTag = "S";
                    identificationPath = "circulo";
                    namePath = "ok";
                    ChangeStateTextbox(TxtIdentificacion);
                    ChangeStateTextbox(TxtName, true);
                    break;
                case "N":
                    identificationTag = "S";
                    nameTag = "N";
                    identificationPath = "ok";
                    namePath = "circulo";
                    ChangeStateTextbox(TxtIdentificacion, true);
                    ChangeStateTextbox(TxtName);
                    break;
                default:
                    identificationTag = "N";
                    nameTag = "S";
                    identificationPath = "circulo";
                    namePath = "ok";
                    ChangeStateTextbox(TxtIdentificacion);
                    ChangeStateTextbox(TxtName, true);
                    break;
            }

            chkIdentification.Tag = identificationTag;
            chkName.Tag = nameTag;
            chkIdentification.Source = Utilities.SetButtonImage("Others", identificationPath, "png");
            chkName.Source = Utilities.SetButtonImage("Others", namePath, "png");
        }

        private void chkName_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            string tag = chkName.Tag.ToString();
            string identificationTag, nameTag, identificationPath, namePath;
            switch (tag)
            {
                case "S":
                    identificationTag = "S";
                    nameTag = "N";
                    identificationPath = "ok";
                    namePath = "circulo";
                    ChangeStateTextbox(TxtIdentificacion, true);
                    ChangeStateTextbox(TxtName);
                    break;
                case "N":
                    identificationTag = "N";
                    nameTag = "S";
                    identificationPath = "circulo";
                    namePath = "ok";
                    ChangeStateTextbox(TxtIdentificacion);
                    ChangeStateTextbox(TxtName, true);
                    break;
                default:
                    identificationTag = "S";
                    nameTag = "N";
                    identificationPath = "ok";
                    namePath = "circulo";
                    ChangeStateTextbox(TxtIdentificacion, true);
                    ChangeStateTextbox(TxtName);
                    break;
            }

            chkIdentification.Tag = identificationTag;
            chkName.Tag = nameTag;
            chkIdentification.Source = Utilities.SetButtonImage("Others", identificationPath, "png");
            chkName.Source = Utilities.SetButtonImage("Others", namePath, "png");
        }

        private void BtnConsultar_PreviewStylusDown(object sender, StylusDownEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtIdentificacion.Text) && string.IsNullOrEmpty(TxtName.Text))
                {
                    lblError.Visibility = Visibility.Visible;
                    return;
                }

                load_gif.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                utilities.saveLogError("BtnConsultar_PreviewMouseDown", "FrmSearch", ex.ToString());
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cambia el estado del textbox que se le envía
        /// </summary>
        /// <param name="txt">textbox a ocultar/mostrar</param>
        /// <param name="state">false(pred.)oculta y true muestra</param>
        private void ChangeStateTextbox(TextBox txt, bool state = false)
        {
            txt.Text = string.Empty;
            if (!state)
            {
                txt.Visibility = Visibility.Hidden;
            }
            else
            {
                txt.Visibility = Visibility.Visible;
            }
        } 

        #endregion
    }
}
