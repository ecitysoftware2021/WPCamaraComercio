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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Views;

namespace WPCamaraComercio
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region "Referencias"
        private List<string> images;
        private Utilities utilities;
        private ImageSleader imageSleader;
        #endregion

        #region "Constructor"
        public MainWindow()
        {
            InitializeComponent();
            utilities = new Utilities();

            images = LoadImageFolder();

            imageSleader = new ImageSleader(images);

            this.DataContext = imageSleader.imageModel;

            imageSleader.time = 2;

            imageSleader.isRotate = true;

            init();

        }
        #endregion

        #region "Eventos"
        /// <summary>
        /// Evento que me redirecciona a la ventana del menú
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    FrmMenu Menu = new FrmMenu();
                    Menu.Show();
                    this.Close();
                });
                GC.Collect();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("Grid_MouseDown_1", "MainWindow", ex.ToString());
            }

        }
        #endregion

        #region "Métodos"
        /// <summary>
        /// Método que me inicia la rotación de las imagenes
        /// </summary>
        private void init()
        {
            try
            {
                imageSleader.star();
            }
            catch (Exception ex)
            {
                utilities.saveLogError("init", "MainWindow", ex.ToString());
            }
        }

        /// <summary>
        /// Método que me carga una lista de imagenes
        /// </summary>
        /// <returns></returns>
        private List<string> LoadImageFolder()
        {
            try
            {
                return new List<string> {
                "/WPCamaraComercio;component/Images/Backgrounds/fondo01.jpg"
                };
            }
            catch (Exception ex)
            {
                utilities.saveLogError("LoadImageFolder", "MainWindow", ex.ToString());
                return null;
            }
        }

        #endregion
    }
}
