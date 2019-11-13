using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;

namespace WPFCCMedellin.ViewModel
{
    public class PaymentViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private decimal _payValue;

        private decimal _valorIngresado;

        private decimal _valorFaltante;

        private decimal _valorSobrante;

        public decimal _valorDispensado;

        public bool _statePay;

        private List<DenominationMoney> _denominations;

        private List<DenominationMoney> listdenominationIn = new List<DenominationMoney>();

        private Visibility _imgCancel;

        private Visibility _imgContinue;

        private Visibility _imgCambio;

        private string _message;

        #endregion

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(viewList)));
            }
        }

        #region Properties

        public decimal PayValue
        {
            get
            {
                return _payValue;
            }
            set
            {
                if (_payValue != value)
                {
                    _payValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PayValue)));
                }
            }
        }

        public List<DenominationMoney> Denominations
        {
            get { return _denominations; }
            set
            {
                _denominations = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Denominations)));
            }
        }
        public decimal ValorIngresado
        {
            get
            {

                return _valorIngresado;
            }
            set
            {
                if (_valorIngresado != value)
                {
                    _valorIngresado = value;
                    ValorFaltante = (ValorIngresado < PayValue) ? PayValue - ValorIngresado : 0;
                    ValorSobrante = (ValorIngresado > PayValue) ? ValorIngresado - PayValue : 0;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorIngresado)));
                }
            }
        }

        public decimal ValorFaltante
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

        public decimal ValorSobrante
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

        public decimal ValorDispensado
        {
            get { return _valorDispensado; }
            set
            {
                if (_valorDispensado != value)
                {
                    _valorDispensado = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ValorDispensado)));
                }
            }
        }

        public bool StatePay
        {
            get { return _statePay; }
            set
            {
                if (_statePay != value)
                {
                    _statePay = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StatePay)));
                }
            }
        }

        public Visibility ImgCancel
        {
            get { return _imgCancel; }
            set
            {
                _imgCancel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImgCancel)));
            }
        }

        public Visibility ImgContinue
        {
            get { return _imgContinue; }
            set
            {
                _imgContinue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImgContinue)));
            }
        }

        public Visibility ImgCambio
        {
            get { return _imgCambio; }
            set
            {
                _imgCambio = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImgCambio)));
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
                }
            }
        }

        public void RefreshListDenomination(int denomination, int quantity)
        {
            try
            {
                //listdenominationIn.AddRange(_denominations);
                var itemDenomination = this.Denominations.Where(d => d.Denominacion == denomination).FirstOrDefault();
                if (itemDenomination == null)
                {
                    this.Denominations.Add(new DenominationMoney
                    {
                        Denominacion = denomination,
                        Quantity = quantity,
                        Total = denomination * quantity
                    });
                }
                else
                {
                    itemDenomination.Quantity += quantity;
                    itemDenomination.Total = denomination * itemDenomination.Quantity;
                }
                // Denominations.AddRange(listdenominationIn);
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, "PaymentViewModel", ex);
            }
        }
        #endregion
    }
}
