using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Service;

namespace WPCamaraComercio
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        WCFServices services;
        Api api;
        bool state;

        public App()
        {
            //services = new WCFServices();
            //api = new Api();
            //GetToken();
        }

        private async void GetToken()
        {
            state = await api.SecurityToken();
            if (state)
            {
                //matar y volver a empezar si no es verdadero
            }
        }
    }
}

