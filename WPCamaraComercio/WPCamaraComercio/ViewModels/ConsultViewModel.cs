using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPCamaraComercio.Models;
using WPCamaraComercio.Service;
using WPCamaraComercio.WCFCamaraComercio;

namespace WPCamaraComercio.ViewModels
{
    class ConsultViewModel : INotifyPropertyChanged
    {
        private WCFServices service;

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

        public void ConsultConcidences(string value, int type)
        {
            try
            {
                this.headers = Visibility.Hidden;
                this.preload = Visibility.Visible;
                this.coincidences.Clear();

                Task.Run(async () =>
                {
                    service = new WCFServices();
                    Service.Response response  = await service.ConsultInformation(value, type);

                    if (response.Result != null)
                    {
                        RespuestaConsulta result = (RespuestaConsulta) response.Result;

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
                                    State = item.estado
                                });
                            }
                        }
                    }

                    this.headers = Visibility.Visible;
                    this.preload = Visibility.Hidden;
                });
            }
            catch (Exception ex)
            {

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }
    }
}
