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
using WPCamaraComercio.Service;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Classes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Globalization;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmTransactionsList.xaml
    /// </summary>
    public partial class FrmTransactionsList : Window
    {
        Api api;
        public FrmTransactionsList()
        {
            InitializeComponent();
            api = new Api();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                GridTransactionList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<ResponseTransactionList>> DataTransactionList()
        {
            try
            {
                RequestTransactions requestTransactions = new RequestTransactions();
                requestTransactions.PayPadId = 1034;
                requestTransactions.TransactId = 0;
                requestTransactions.StateId = 2;
                string nowDay = DateTime.Now.ToString("MM/dd/yyyy");
                requestTransactions.StartDate = DateTime.Parse(nowDay, CultureInfo.InvariantCulture);
                string nextDay = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
                requestTransactions.FinishDate = DateTime.Parse(nextDay, CultureInfo.InvariantCulture);
                requestTransactions.DateStartString = requestTransactions.StartDate.ToString("MM/dd/yyyy");
                requestTransactions.DateFinishString = requestTransactions.FinishDate.ToString("MM/dd/yyyy");

                List<ResponseTransactionList> respList = new List<ResponseTransactionList>();
                var res = await api.GetResponse(requestTransactions, "GetTransactionsCM");

                respList = JsonConvert.DeserializeObject<List<ResponseTransactionList>>(res.Data.ToString());

                return respList;
            }
            catch (Exception err)
            {
                throw;
            }
        }

        public async void GridTransactionList()
        {
            try
            {
                CollectionViewSource view = new CollectionViewSource();
                ObservableCollection<ResponseTransactionList> lstPager = new ObservableCollection<ResponseTransactionList>();
                List<ResponseTransactionList> TransactionList = new List<ResponseTransactionList>();
                TransactionList = await DataTransactionList();
                foreach (var item in TransactionList)
                {
                    lstPager.Add(item);
                }
                view.Source = lstPager;
                lv_Transactions.DataContext = view;
            }
            catch (Exception err)
            {
                throw;
            }
        }
    }
}
