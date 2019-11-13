﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services.Object;
using WPFCCMedellin.ViewModel;

namespace WPFCCMedellin.UserControls
{
    /// <summary>
    /// Lógica de interacción para CancelUserControl.xaml
    /// </summary>
    public partial class ReturnMonyUserControl : UserControl
    {
        private Transaction transaction;

        private PaymentViewModel viewModel;

        public ReturnMonyUserControl(Transaction transaction)
        {
            InitializeComponent();

            this.transaction = transaction;

            this.DataContext = transaction.Payment;

            OrganizeValues();
        }


        private void OrganizeValues()
        {
            try
            {
                if (transaction.Payment == null && transaction.Type == ETransactionType.Withdrawal)
                {
                    this.viewModel = new PaymentViewModel
                    {
                        PayValue = 0,
                        ValorFaltante = 0,
                        ValorSobrante = transaction.Amount,
                        ValorIngresado = 0,
                        ValorDispensado = 0,
                        StatePay = false,
                        Message = MessageResource.MessageReturnMony
                    };
                }
                else
                {
                    if (transaction.Payment != null)
                    {
                        viewModel = transaction.Payment;
                        viewModel.StatePay = false;
                        viewModel.ValorSobrante = transaction.Payment.ValorIngresado - transaction.Payment.ValorDispensado;
                        viewModel.Message = MessageResource.TransactionCancel;
                    }
                }

                this.DataContext = this.viewModel;

                FinishCancelPay();
                //ReturnMoney();
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void ReturnMoney()
        {
            try
            {
                Task.Run(() =>
                {
                    AdminPayPlus.ControlPeripherals.callbackTotalOut = totalOut =>
                    {
                        if (!this.viewModel.StatePay)
                        {
                            transaction.Payment.ValorDispensado = totalOut;

                            if (transaction.Type != ETransactionType.Withdrawal)
                            {
                                transaction.Payment.ValorSobrante = transaction.Payment.ValorIngresado;
                                transaction.State = ETransactionState.Cancel;
                            }

                            FinishCancelPay();
                        }
                    };

                    AdminPayPlus.ControlPeripherals.callbackError = error =>
                    {
                        AdminPayPlus.SaveLog(new RequestLogDevice
                        {
                            Code = error.Item1,
                            Date = DateTime.Now,
                            Description = error.Item2,
                            Level = ELevelError.Medium,
                            TransactionId = transaction.IdTransactionAPi
                        }, ELogType.Device);
                    };

                    AdminPayPlus.ControlPeripherals.callbackOut = valueOut =>
                    {
                        AdminPayPlus.ControlPeripherals.callbackOut = null;
                        if (!this.viewModel.StatePay)
                        {
                            transaction.Payment.ValorDispensado = valueOut;

                            if (transaction.Payment.ValorDispensado != transaction.Payment.ValorSobrante)
                            {
                                if (transaction.Type != ETransactionType.Withdrawal)
                                {
                                    transaction.State = ETransactionState.CancelError;
                                }
                                else
                                {
                                    transaction.State = ETransactionState.Error;
                                }

                                transaction.Observation += MessageResource.IncompleteMony;
                            }
                            FinishCancelPay();
                        }
                    };

                    AdminPayPlus.ControlPeripherals.callbackLog = log =>
                    {
                        AdminPayPlus.SaveDetailsTransaction(transaction.IdTransactionAPi, 0, 0, 0, string.Empty, log);
                    };

                    AdminPayPlus.ControlPeripherals.StartDispenser(transaction.Payment.ValorSobrante);
                });
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        private void FinishCancelPay()
        {
            try
            {
                if (!this.viewModel.StatePay)
                {
                    this.viewModel.StatePay = true;
                    transaction.Payment = viewModel;

                    // AdminPayPlus.ControlPeripherals.ClearValues();

                    if (transaction.Type == ETransactionType.Withdrawal)
                    {
                        Task.Run(async () =>
                        {
                            //transaction = await AdminPayPlus.ApiIntegration.NotifycTransaction(transaction);

                            Utilities.CloseModal();
                            if (transaction.IdTransactionAPi > 0 && transaction.State == ETransactionState.Success)
                            {
                                Utilities.navigator.Navigate(UserControlView.PaySuccess, false, this.transaction);
                            }
                            else
                            {
                                AdminPayPlus.SaveErrorControl(MessageResource.NoInsertTransaction, this.transaction.TransactionId.ToString(), EError.Api, ELevelError.Strong);
                                Utilities.ShowModal(MessageResource.NoInsertTransaction, EModalType.Error);

                                Utilities.navigator.Navigate(UserControlView.PaySuccess, false, this.transaction);
                            }
                        });

                        Utilities.ShowModal(MessageResource.FinishTransaction, EModalType.Preload);
                    }
                    else
                    {
                        AdminPayPlus.UpdateTransaction(transaction);

                        Utilities.PrintVoucher(this.transaction);

                        Thread.Sleep(6000);

                        Utilities.navigator.Navigate(UserControlView.Main);
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}