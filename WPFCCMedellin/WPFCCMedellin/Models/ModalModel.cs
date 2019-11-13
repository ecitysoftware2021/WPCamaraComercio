using System.ComponentModel;
using WPFCCMedellin.Classes;

namespace WPFCCMedellin.Models
{
    public class ModalModel : INotifyPropertyChanged
    {
        private string _Messaje;

        public string Messaje
        {
            get
            {
                return _Messaje;
            }
            set
            {
                _Messaje = value;
                OnPropertyRaised("Messaje");
            }
        }

        private string _Tittle;

        public string Tittle
        {
            get
            {
                return _Tittle;
            }
            set
            {
                _Tittle = value;
                OnPropertyRaised("Tittle");
            }
        }

        private EModalType _TypeModal;

        public EModalType TypeModal
        {
            get
            {
                return _TypeModal;
            }
            set
            {
                _TypeModal = value;
                OnPropertyRaised("TypeModal");
            }
        }

        private string _ImageModal;

        public string ImageModal
        {
            get
            {
                return _ImageModal;
            }
            set
            {
                _ImageModal = value;
                OnPropertyRaised("ImageModal");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

        }
    }
}