using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPCamaraComercio.Service;
using System.Configuration;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Objects;
using Newtonsoft.Json;
using WPCamaraComercio.Views;

namespace WPCamaraComercio
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Api api;
        bool state;
        Utilities util;

        public MainWindow()
        {
            InitializeComponent();
            api = new Api();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                GetToken();
            });
        }

        /// <summary>
        /// Método encargado de obtener el token necesario para que el corresponsal pueda operar, seguido de esto se consulta el estado inicial del corresponsal
        /// para saber si se pueden realizar transacciones
        /// </summary>
        private async void GetToken()
        {
            try
            {
                state = await api.SecurityToken();
                if (state)
                {
                    var response = await api.GetResponse(new ObjectsApi.RequestApi(), "InitPaypad");
                    if (response.CodeError == 200)
                    {
                        DataPayPad data = JsonConvert.DeserializeObject<DataPayPad>(response.Data.ToString());
                        util = new Utilities(1);

                        if (data.StateAceptance || data.StateDispenser)
                        {
                            Utilities.dataPaypad = data;
                            
                            Utilities.control.callbackToken = isSucces =>
                            {
                                Utilities.GoToInicial();
                            };
                            Utilities.control.Start();
                        }
                        else
                        {
                            ShowModalError();
                        }
                    }
                    else
                    {
                        ShowModalError();
                    }
                }
                else
                {
                    ShowModalError();
                }
            }
            catch (Exception ex)
            {
                ShowModalError();
            }
        }

        private void ShowModalError()
        {
            Dispatcher.BeginInvoke((Action)delegate
            {
                FrmModal modal = new FrmModal(string.Concat("Lo sentimos,", Environment.NewLine, "el cajero no se encuentra disponible."), this);
                modal.ShowDialog();
                if (modal.DialogResult.HasValue)
                {
                    Utilities.RestartApp();
                }
            });
        }
    }
}
