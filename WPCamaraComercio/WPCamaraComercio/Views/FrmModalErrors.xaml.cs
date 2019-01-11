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

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Interaction logic for FrmModalErrors.xaml
    /// </summary>
    public partial class FrmModalErrors : Window
    {
        public FrmModalErrors(string message)
        {
            InitializeComponent();
            LblMessage.Text = message;
        }

        private void BtnOk_StylusDown(object sender, StylusDownEventArgs e)
        {
            this.Close();
        }
    }
}
