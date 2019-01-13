using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Objects;
using WPCamaraComercio.Service;

namespace WPCamaraComercio
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        //WCFServices services;
        //Api api;
        //bool state;

        public App()
        {
            //services = new WCFServices();
            //api = new Api();
            //GetToken();
        }

        /// <summary>
        /// Método encargado de obtener el token necesario para que el corresponsal pueda operar, seguido de esto se consulta el estado inicial del corresponsal
        /// para saber si se pueden realizar transacciones
        /// </summary>
        //private async void GetToken()
        //{
        //    try
        //    {
        //        //state = await api.SecurityToken();
        //        state = true;
        //        await Task.Run(() =>
        //        {
        //            Thread.Sleep(5000);
        //        });

        //        if (state)
        //        {
        //            //var response = await api.GetResponse(new Uptake.RequestApi(), "GetInitDataPaypad");
        //            //if (response.CodeError == 200)
        //            //{
        //            //DataPaypad data = (DataPaypad)response.Data;
        //            var data = new DataPayPad
        //            {
        //                stateAceptance = true,
        //                stateDispenser = true,
        //            };
        //            //Utilities.ImagesSlider = (List<string>)data.ListImages;
        //            //Utilities.CountSlider = 1;
        //            if (data.stateAceptance || data.stateDispenser)
        //            {
        //                Utilities.dataPaypad = data;
        //                await Task.Run(() =>
        //                {
        //              //      ConsultImagesSlider();
        //                });
        //                Utilities util = new Utilities(1);
        //                Utilities.control.callbackToken = isSucces =>
        //                {
        //                    //Dispatcher.BeginInvoke((Action)delegate
        //                    //{
        //                    //    MainWindow main = new MainWindow();
        //                    //    main.Show();
        //                    //});
        //                };
        //                Utilities.control.Start();
        //            }
        //            else
        //            {
        //                //ShowModalError();
        //            }
        //            //}
        //            //else
        //            //{
        //            //        ShowModalError();
        //            //    }
        //        }
        //        else
        //        {
        //            //ShowModalError();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ShowModalError();
        //    }
        //}
    }
}

