using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WPCamaraComercio.Classes;
using WPCamaraComercio.Models;
using WPCamaraComercio.Service;
using WPCamaraComercio.Views;
using WPCamaraComercio.WCFCamaraComercio;
using static WPCamaraComercio.Objects.ObjectsApi;

namespace WPCamaraComercio.ViewModels
{
    class ConsultViewModel : INotifyPropertyChanged
    {
        private WCFServices service;

        string modalMessage = string.Concat("Lo sentimos, ",
                            Environment.NewLine,
                            "En este momento el servicio no se encuentra disponible.");

        public Action<bool> callbackSearch;

        private List<Coincidence> _coincidences;

        public List<Coincidence> coincidences
        {
            get
            {
                return _coincidences;
            }
            set
            {
                _coincidences = value;
                OnPropertyRaised("coincidences");
            }
        }

        private CollectionViewSource _viewList;

        public CollectionViewSource viewList
        {
            get
            {
                return _viewList;
            }
            set
            {
                _viewList = value;
                OnPropertyRaised("viewList");
            }
        }

        private Visibility _preload;

        public Visibility preload
        {
            get
            {
                return _preload;
            }
            set
            {
                _preload = value;
                OnPropertyRaised("preload");
            }
        }

        private Visibility _headers;

        public Visibility headers
        {
            get
            {
                return _headers;
            }
            set
            {
                _headers = value;
                OnPropertyRaised("headers");
            }
        }

        private int _typeSearch;

        public int typeSearch
        {
            get
            {
                return _typeSearch;
            }
            set
            {
                _typeSearch = value;
                OnPropertyRaised("typeSearch");
            }
        }


        private int _countConcidences;

        public int countConcidences
        {
            get
            {
                return coincidences.Count();
            }
        }


        private string _sourceCheckNit;

        public string sourceCheckNit
        {
            get
            {
                return _sourceCheckNit;
            }
            set
            {
                _sourceCheckNit = value;
                OnPropertyRaised("sourceCheckNit");
            }
        }


        private string _sourceCheckName;

        public string sourceCheckName
        {
            get
            {
                return _sourceCheckName;
            }
            set
            {
                _sourceCheckName = value;
                OnPropertyRaised("sourceCheckName");
            }
        }

        private string _message;

        public string message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyRaised("message");
            }
        }

        private Visibility _error;

        public Visibility error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
                OnPropertyRaised("error");
            }
        }

        public async void ConsultConcidences(string value, int type)
        {
            //bool stateConsult = false;
            try
            {
                Api api = new Api();

                NewConsultApi newConsult = new NewConsultApi
                {
                    uno = value,
                    dos = type
                };

                var response = await api.GetResponse(new RequestApi
                {
                    Data = newConsult
                }, "GetGeneralInformation");

                //    this.preload = Visibility.Visible;

                //    service = new WCFServices();

                //    var task = service.ConsultInformation(value, type);
                //    if (await Task.WhenAny(task, Task.Delay(10000000)) == task)
                //    {
                //        var response = task.Result;
                //        if (response.IsSuccess)
                //        {
                //            Utilities.RespuestaConsulta = (RespuestaConsulta)response.Result;

                //            if (Utilities.RespuestaConsulta.response.resultados.Count() > 0)
                //            {
                //                foreach (var item in Utilities.RespuestaConsulta.response.resultados)
                //                {
                //                    _coincidences.Add(new Coincidence
                //                    {
                //                        BusinessName = item.nombre,
                //                        Nit = item.nit,
                //                        Municipality = item.municipio.Split(')')[1],
                //                        EstabliCoincide = item.EstablecimientosConCoincidencia,
                //                        State = item.estado,
                //                        Enrollment = item.matricula,
                //                        Tpcm = item.tpcm
                //                    });
                //                }

                //                stateConsult = true;
                //            }
                //            else
                //            {
                //                FrmModal modal = new FrmModal(modalMessage);
                //                modal.ShowDialog();
                //            }
                //        }
                //        else
                //        {
                //            FrmModal modal = new FrmModal(modalMessage);
                //            modal.ShowDialog();
                //        }
                //        this.preload = Visibility.Hidden;
                //        callbackSearch?.Invoke(stateConsult);
                //    }
            }
            catch (Exception ex)
            {
                //this.preload = Visibility.Hidden;
                //callbackSearch?.Invoke(stateConsult);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }
    }
}

