using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;

namespace WPFCCMedellin.Services.Object
{
    class RequestApi
    {
        public int Session { get; set; }

        public int User { get; set; }

        public object Data { get; set; }
    }

    public class RequestAuth
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public int Type { get; set; }
    }

    public class RequestTransactionDetails
    {
        public int TransactionId { get; set; }

        public int Denomination { get; set; }

        public int Operation { get; set; }

        public int Quantity { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }
    }

    public class PaypadOperationControl : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        public int ID { get; set; }

        private decimal _TOTAL;

        private string _DESCRIPTION;

        private List<List> _DATALIST;

        private CollectionViewSource _viewList;

        #endregion

        public CollectionViewSource viewList
        {
            get { return _viewList; }
            set
            {
                _viewList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(viewList)));
            }
        }

        #region Properties

        public List<List> DATALIST
        {
            get { return _DATALIST; }
            set
            {
                _DATALIST = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DATALIST)));
            }
        }

        public decimal TOTAL
        {
            get { return _TOTAL; }
            set
            {
                _TOTAL = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TOTAL)));
            }
        }

        public string DESCRIPTION
        {
            get { return _DESCRIPTION; }
            set
            {
                _DESCRIPTION = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DESCRIPTION)));
            }
        }

        public List<DenominationMoney> DATALIST_FILTER()
        {
            List<DenominationMoney> dataListsNew = new List<DenominationMoney>();
            try
            {
                foreach (var item in this.DATALIST)
                {
                    if (item.AMOUNT_NEW > 0)
                    {
                        var itemUpdate = dataListsNew.Where(d => d.Denominacion == item.VALUE).FirstOrDefault();
                        if (itemUpdate != null)
                        {
                            itemUpdate.Quantity += itemUpdate.Quantity;
                            itemUpdate.Total = (int)itemUpdate.Quantity * (int)itemUpdate.Denominacion;
                        }
                        else
                        {
                            dataListsNew.Add(new DenominationMoney
                            {
                                Denominacion = (decimal)item.VALUE,
                                Quantity = (decimal)item.AMOUNT_NEW,
                                Total = item.TOTAL_AMOUNT
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return dataListsNew;
        }
        #endregion
    }

    public class List : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private int? _AMOUNT_NEW;

        private int? _AMOUNT;

        public int? ID { get; set; }

        public int? DEVICE_PAYPAD_ID { get; set; }

        private string _DESCRIPTION;

        private int? _VALUE;

        public int? CURRENCY_DENOMINATION_ID { get; set; }

        public int? DEVICE_TYPE_ID { get; set; }

        private decimal _TOTAL_AMOUNT;

        public int? AMOUNT_NEW
        {
            get { return _AMOUNT_NEW; }
            set
            {
                _AMOUNT_NEW = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AMOUNT_NEW)));
            }
        }

        public int? AMOUNT
        {
            get { return _AMOUNT; }
            set
            {
                _AMOUNT = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AMOUNT)));
            }
        }

        public string DESCRIPTION
        {
            get { return _DESCRIPTION; }
            set
            {
                _DESCRIPTION = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DESCRIPTION)));
            }
        }

        public int? VALUE
        {
            get { return _VALUE; }
            set
            {
                _VALUE = value;
                TOTAL_AMOUNT = (int)_AMOUNT_NEW * (int)value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VALUE)));
            }
        }

        public decimal TOTAL_AMOUNT
        {
            get { return _TOTAL_AMOUNT; }
            set
            {
                _TOTAL_AMOUNT = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TOTAL_AMOUNT)));
            }
        }
    }
}
