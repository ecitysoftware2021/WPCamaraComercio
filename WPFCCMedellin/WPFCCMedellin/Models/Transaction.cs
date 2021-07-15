using System;
using System.Collections.Generic;
using System.ComponentModel;
using WPFCCMedellin.Classes;
using WPFCCMedellin.DataModel;
using WPFCCMedellin.Services;
using WPFCCMedellin.Services.Object;
using WPFCCMedellin.ViewModel;

namespace WPFCCMedellin.Models
{
    public class Transaction : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        //--------------------- Cancelar transacción -----------------------------

        public int idCompra { get; set; }

        public decimal valorCompra { get; set; }

        public string referenciaPago { get; set; }

        public string observaciones { get; set; }

        public CancelPayment cancelPayment { get; set; }

        //--------------------- Fin de cacelar transacción --------------------------
        public string message { get; set; }

        public string consecutive { get; set; }

        public string reference { get; set; }

        public string Enrollment { get; set; }

        public string Tpcm { get; set; }

        public DateTime DateTransaction { get; set; }

        public PaymentViewModel Payment { get; set; }

        public bool IsReturn { get; set; }

        public ETransactionState State { get; set; }

        public int StateNotification { get; set; }

        public string Observation { get; set; }

        public ETransactionType Type { get; set; }

        public PAYER payer { get; set; }

        public List<Product> Products { get; set; }

        public List<ResultadoDetalle> Files { get; set; }

        private decimal _Amount;

        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
                OnPropertyRaised("Amount");
            }
        }

        private int _transactionId { get; set; }

        public int TransactionId
        {
            get
            {
                return _transactionId;
            }
            set
            {
                _transactionId = value;
                OnPropertyRaised("TransactionId");
            }
        }

        private int _idTransactionAPi { get; set; }

        public int IdTransactionAPi
        {
            get
            {
                return _idTransactionAPi;
            }
            set
            {
                _idTransactionAPi = value;
                OnPropertyRaised("IdTransactionAPi");
            }
        }
    }
}
