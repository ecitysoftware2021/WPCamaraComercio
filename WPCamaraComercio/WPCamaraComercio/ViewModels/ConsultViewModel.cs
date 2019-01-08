using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WPCamaraComercio.Models;
using WPCamaraComercio.Service;
using WPCamaraComercio.WCFCamaraComercio;

namespace WPCamaraComercio.ViewModels
{
    class ConsultViewModel : INotifyPropertyChanged
    {
        private WCFServices service;

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

        public void ConsultConcidences(string value, int type)
        {
            bool stateConsult = false;
            try
            {
                
                this.preload = Visibility.Visible;
                

                Task.Run(async () =>
                {
                    service = new WCFServices();
                    Service.Response response = await service.ConsultInformation(value, type);

                    if (response.Result != null)
                    {
                        RespuestaConsulta result = (RespuestaConsulta)response.Result;

                        if (result.response.resultados.Count() > 0)
                        {
                            foreach (var item in result.response.resultados)
                            {
                                _coincidences.Add(new Coincidence
                                {
                                    BusinessName = item.nombre,
                                    Nit = item.nit,
                                    Municipality = item.municipio.Split(')')[1],
                                    EstabliCoincide = item.EstablecimientosConCoincidencia,
                                    State = item.estado,
                                    Enrollment = item.matricula,
                                    Tpcm = item.tpcm
                                });
                            }

                            stateConsult = true;
                        }
                    }
                    this.preload = Visibility.Hidden;
                    callbackSearch?.Invoke(stateConsult);
                });
            }
            catch (Exception ex)
            {
                this.preload = Visibility.Hidden;
                callbackSearch?.Invoke(stateConsult);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }
    }
}

