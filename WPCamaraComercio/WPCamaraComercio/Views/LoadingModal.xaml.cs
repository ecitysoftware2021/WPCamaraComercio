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
using WPCamaraComercio.Models;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for LoadingModal.xaml
    /// </summary>
    public partial class LoadingModal : Window
    {
        private Coincidence coincidence;
        
        public LoadingModal(Coincidence coincidence)
        {
            InitializeComponent();

            this.coincidence = coincidence;

            SearchDetails();
        }

        private void SearchDetails()
        {
            Task.Run(() =>
            {

            });
        }
    }
}
