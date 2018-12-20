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
    /// Interaction logic for FrmDetailCompany.xaml
    /// </summary>
    public partial class FrmDetailCompany : Window
    {
        public FrmDetailCompany()
        {
            InitializeComponent();
            ComboBox ddlCantidad = new ComboBox();
            ddlCantidad.Items.Insert(0, "Seleccionar");
            for (int i = 1; i <= 5; i++)
            {
                ddlCantidad.Items.Insert(i, i);
            }
        }

        private void AssingProperties()
        {
            try
            {
                if (tipo == 1)
                {
                    GenerateVigencias();
                }
                else
                {
                    GenerateCuotas();
                }
            }
            catch (Exception ex)
            {
                //navigationService.NavigationTo(ex.Message);
            }
        }
    }
}
