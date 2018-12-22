using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPCamaraComercio.Models;
using WPCamaraComercio.Service;

namespace WPCamaraComercio.ViewModels
{
    class ConsultViewModel
    {
        private WCFServices service;

        public ConsultViewModel()
        {
            service = new WCFServices();
        }

        private List<Coincidence> _coincidences;

        public List<Coincidence> coincidences
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                OnPropertyRaised("code");
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

        private string _messageError;

        public string messageError
        {
            get
            {
                return _messageError;
            }
            set
            {
                _messageError = value;
                OnPropertyRaised("messageError");
            }
        }

        public string messageError
        {
            get
            {
                return _messageError;
            }
            set
            {
                _messageError = value;
                OnPropertyRaised("messageError");
            }
        }

        public void ConsultConcidences()
        {

        }

        public int CountConcidences()
        {
            return coincidences.Count();
        }
    }
}
