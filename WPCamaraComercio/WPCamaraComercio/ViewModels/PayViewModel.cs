using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPCamaraComercio.ViewModels
{
    public class PayViewModel : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attribute
        private string _valorIngresado;
        private string _valorSobrante;
        private string _valorFaltante;
        private Visibility _imgLeyendoBillete;
        private Visibility _imgRecibo;
        private Visibility _imgIngreseBillete;
        private Visibility _imgCancel;
        private Visibility _ImgEspereCambio;
        #endregion

        #region Properties
        public Visibility ImgEspereCambio
        {
            get { return _ImgEspereCambio; }
            set
            {
                if (_ImgEspereCambio != value)
                {
                    _ImgEspereCambio = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImgEspereCambio)));
                }
            }
        }

        public Visibility ImgIngreseBillete
        {
            get { return _imgIngreseBillete; }
            set
            {
                if (_imgIngreseBillete != value)
                {
                    _imgIngreseBillete = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImgIngreseBillete)));
                }
            }
        }

        public Visibility ImgCancel
        {
            get { return _imgCancel; }
            set
            {
                if (_imgCancel != value)
                {
                    _imgCancel = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImgCancel)));
                }
            }
        }

        public Visibility ImgLeyendoBillete
        {
            get { return _imgLeyendoBillete; }
            set
            {
                if (_imgLeyendoBillete != value)
                {
                    _imgLeyendoBillete = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImgLeyendoBillete)));
                }
            }
        }



        public Visibility ImgRecibo
        {
            get { return _imgRecibo; }
            set
            {
                if (_imgRecibo != value)
                {
                    _imgRecibo = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImgRecibo)));
                }
            }
        }


        public string ValorIngresado
        {
            get { return _valorIngresado; }
            set
            {
                if (_valorIngresado != value)
                {
                    _valorIngresado = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorIngresado)));
                }
            }
        }

        public string ValorSobrante
        {
            get { return _valorSobrante; }
            set
            {
                if (_valorSobrante != value)
                {
                    _valorSobrante = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorSobrante)));
                }

            }
        }

        public string ValorFaltante
        {
            get { return _valorFaltante; }
            set
            {
                if (_valorFaltante != value)
                {
                    _valorFaltante = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorFaltante)));
                }
            }
        }
        #endregion
    }
}
