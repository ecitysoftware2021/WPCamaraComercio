﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPCamaraComercio.Classes;

namespace WPCamaraComercio.Views
{
    /// <summary>
    /// Lógica de interacción para FrmPagoCancelado.xaml
    /// </summary>
    public partial class FrmCancelledPayment : Window
    {
        public FrmCancelledPayment()
        {
            InitializeComponent();
            lblValue.Content = Utilities.ValueToPay;
        }
    }
}
