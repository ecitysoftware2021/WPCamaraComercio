using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WPFCCMedellin.Classes;
using WPFCCMedellin.DataModel;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;
using WPFCCMedellin.Services;
using WPFCCMedellin.Services.Object;

namespace WPFCCMedellin.ViewModel
{
    class DataListViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private string _tittle;

        public string Tittle
        {
            get
            {
                return _tittle;
            }
            set
            {
                if (_tittle != value)
                {
                    _tittle = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Tittle)));
                }
            }
        }

        private string _colum1;

        public string Colum1
        {
            get
            {
                return _colum1;
            }
            set
            {
                if (_colum1 != value)
                {
                    _colum1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Colum1)));
                }
            }
        }

        private string _colum2;

        public string Colum2
        {
            get
            {
                return _colum2;
            }
            set
            {
                if (_colum2 != value)
                {
                    _colum2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Colum2)));
                }
            }
        }

        private string _colum3;

        public string Colum3
        {
            get
            {
                return _colum3;
            }
            set
            {
                if (_colum3 != value)
                {
                    _colum3 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Colum3)));
                }
            }
        }

        private string _colum4;

        public string Colum4
        {
            get
            {
                return _colum4;
            }
            set
            {
                if (_colum4 != value)
                {
                    _colum4 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Colum4)));
                }
            }
        }

        private string _sourceCheckId;

        public string SourceCheckId
        {
            get
            {
                return _sourceCheckId;
            }
            set
            {
                _sourceCheckId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceCheckId)));
            }
        }

        private string _sourceCheckName;

        public string SourceCheckName
        {
            get
            {
                return _sourceCheckName;
            }
            set
            {
                _sourceCheckName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SourceCheckName)));
            }
        }

        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
            }
        }

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Amount)));
            }
        }

        private int _currentPageIndex;

        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                if (_currentPageIndex != value)
                {
                    _currentPageIndex = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPageIndex)));
                }
            }
        }

        public int CuantityItems
        {
            get { return int.Parse(Utilities.GetConfiguration("CuantityItemsList")); }
        }

        private int _totalPage;

        public int TotalPage
        {
            get { return _totalPage; }
            set
            {
                if (_totalPage != value)
                {
                    _totalPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TotalPage)));
                }
            }
        }

        private Visibility _visibilityPagination;

        public Visibility VisibilityPagination
        {
            get
            {
                return _visibilityPagination;
            }
            set
            {
                _visibilityPagination = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityPagination)));
            }
        }

        private Visibility _visibilityName;

        public Visibility VisibilityName
        {
            get
            {
                return _visibilityName;
            }
            set
            {
                _visibilityName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityName)));
            }
        }

        private Visibility _visibilityId;

        public Visibility VisibilityId
        {
            get
            {
                return _visibilityId;
            }
            set
            {
                _visibilityId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityId)));
            }
        }

        private Visibility _visibilityPrevius;

        public Visibility VisibilityPrevius
        {
            get
            {
                return _visibilityPrevius;
            }
            set
            {
                _visibilityPrevius = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityPrevius)));
            }
        }

        private Visibility _visibilityNext;

        public Visibility VisibilityNext
        {
            get
            {
                return _visibilityNext;
            }
            set
            {
                _visibilityNext = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VisibilityNext)));
            }
        }

        private EtypeConsult _typeConsult;

        public EtypeConsult TypeConsult
        {
            get
            {
                return _typeConsult;
            }
            set
            {
                _typeConsult = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypeConsult)));
            }
        }

        private ETypeCertificate _typeCertificates;

        public ETypeCertificate TypeCertificates
        {
            get
            {
                return _typeCertificates;
            }
            set
            {
                _typeCertificates = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypeCertificates)));
            }
        }

        private CollectionViewSource _viewList;

        public CollectionViewSource ViewList
        {
            get
            {
                return _viewList;
            }
            set
            {
                _viewList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViewList)));
            }
        }

        private List<ItemList> _dataList;

        public List<ItemList> DataList
        {
            get { return _dataList; }
            set
            {
                _dataList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataList)));
            }
        }

        private List<ItemList> _dataListAux;

        public List<ItemList> DataListAux
        {
            get { return _dataListAux; }
            set
            {
                _dataListAux = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataListAux)));
            }
        }
        #endregion

        internal void ConfigurateDataList(object data, ETransactionType type)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        internal void RefreshView(bool activate)
        {
            try
            {
                if (activate)
                {
                    VisibilityPagination = Visibility.Visible;
                    if (_dataList.Count > CuantityItems)
                    {
                        VisibilityNext = Visibility.Visible;
                    }
                }
                else
                {
                    VisibilityPagination = Visibility.Hidden;
                    VisibilityNext = Visibility.Hidden;
                    VisibilityPrevius = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        internal void RefreshViewDetail()
        {
            try
            {
                VisibilityPagination = Visibility.Visible;
                if (_dataListAux.Count > CuantityItems)
                {
                    VisibilityNext = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        public async Task<bool> ConsultConcidences(string reference, int type)
        {
            try
            {
                if (DataList != null && DataList.Count > 0)
                {
                    DataList.Clear();
                }

                var response = await AdminPayPlus.ApiIntegration.GetData(new RequestConsult
                {
                    busqueda = reference,
                    tipo_busqueda = type
                }, "GetGeneralInformation");

                if (response.CodeError == 200)
                {
                    var result = JsonConvert.DeserializeObject<ResultGeneral>(response.Data.ToString());

                    if (result != null && int.Parse(result.response.registros) > 0)
                    {
                        foreach (var item in result.response.resultados)
                        {
                            _dataList.Add(new ItemList
                            {
                                Item1 = item.nombre,
                                Item2 = string.Concat("Nit: ", item.nit),
                                Item3 = item.municipio.Split(')')[1],
                                Item4 = item.estado,
                                ImageSourse = ImagesUrlResource.ImgSelect,
                                Data = item
                            });
                        }

                        TotalPage = (int)Math.Ceiling(((decimal)_dataList.Count / CuantityItems));
                         
                        //if (_dataList.Count % CuantityItems != 0)
                        //{
                        //    TotalPage += 1;
                        //}

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }

            return false;
        }

        public async Task<Transaction> ConsultDetails(ItemList item)
        {
            try
            {
                var data = (ResultadoGeneral)item.Data;

                var response = await AdminPayPlus.ApiIntegration.GetData(new RequestDetail
                {
                    Matricula = data.matricula,
                    Tpcm = data.tpcm
                }, "GetDetalle");

                if (response.CodeError == 200)
                {
                    var result = JsonConvert.DeserializeObject<ResponseDetalleComerciante>(response.Data.ToString());

                    if (result != null && result.Result != null && result.Result.response != null && result.Result.response.resultados != null)
                    {
                        return new Transaction
                        {
                            Files = result.Result.response.resultados,
                            Type = ETransactionType.PaymentFile,
                            Enrollment = data.matricula,
                            Tpcm = data.tpcm,
                            reference = data.nit,
                            State = ETransactionState.Initial,
                            payer = new PAYER
                            {
                                IDENTIFICATION = data.nit,
                                NAME = data.nombre,
                                ADDRESS = data.municipio
                            },
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }

            return null;
        }

        public void LoadDataList(Transaction transaction, ETypeCertificate type)
        {
            try
            {
                
                if (type == ETypeCertificate.Merchant)
                {
                    if (transaction.Files[0] != null && transaction.Files[0].certificados != null)
                    {
                        Colum1 = "CERTIFICADO";
                        Colum2 = "VALOR";
                        Colum3 = "CANTIDAD";
                        if (DataList != null && DataList.Count == 0)
                        {
                            foreach (var file in transaction.Files[0].certificados)
                            {
                                DataList.Add(new ItemList
                                {
                                    Item1 = file.NombreCertificado,
                                    Item5 = decimal.Parse(file.ValorCertificado),
                                    Item6 = 0,
                                    Index = DataList.Count,
                                    Data = file
                                });
                            }

                            TotalPage = (int)Math.Ceiling(((decimal)_dataList.Count / CuantityItems));
                        }
                        RefreshView(true);
                    }
                }
                else
                {
                    if (transaction.Files[0] != null && transaction.Files[0].establecimientos != null)
                    {
                        Colum1 = "ESTABLECIMIENTO";
                        Colum2 = "";
                        Colum3 = "";
                        if (DataListAux != null && DataListAux.Count == 0)
                        {
                            foreach (var establishment in transaction.Files[0].establecimientos)
                            {
                                if (establishment.CertificadosEstablecimiento != null)
                                {
                                    foreach (var file in establishment.CertificadosEstablecimiento)
                                    {
                                        DataListAux.Add(new ItemList
                                        {
                                            Item1 = establishment.NombreEstablecimiento,
                                            Item5 = decimal.Parse(file.ValorCertificado),
                                            Item6 = 0,
                                            Index = DataListAux.Count,
                                            Item3 = file.NombreCertificado,
                                            Data = file,
                                            Detail = establishment
                                        });
                                    }
                                }
                            }

                            TotalPage = (int)Math.Ceiling(((decimal)DataListAux.Count / CuantityItems));
                            
                        }
                        RefreshViewDetail();
                    }
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        public Transaction GetListFiles(Transaction transaction)
        {
            try
            {
                List<Product> products = new List<Product>();

                if (DataList != null && DataList.Count > 0)
                {
                    foreach (var item in DataList)
                    {
                        if (item.Item6 > 0)
                        {
                            var certificate = (CertificadoComerciante)item.Data;
                            products.Add(new Product
                            {
                                NumeroCertificados = item.Item6.ToString(),
                                CodigoGeneracion = certificate.CodigoGeneracion,
                                IdCertificado = certificate.IdCertificado,
                                MatriculaEst = certificate.MatriculaEstablecimiento,
                                matricula = transaction.Enrollment,
                                tpcm = transaction.Tpcm,
                                EstablishCertificate = new EstablishCertificate
                                {
                                    CertificateCost = decimal.Parse(certificate.ValorCertificado),
                                    CertificateId = certificate.IdCertificado,
                                    EstablishEnrollment = certificate.MatriculaEstablecimiento,
                                    GenerationCode = certificate.CodigoGeneracion,
                                    NombreCertificado = certificate.NombreCertificado
                                }
                            });
                        }
                    }
                }
                if (DataListAux != null && DataListAux.Count > 0)
                {
                    foreach (var item in DataListAux)
                    {
                        if (item.Item6 > 0)
                        {
                            var certificate = (CertificadosEstablecimiento)item.Data;
                            products.Add(new Product
                            {
                                NumeroCertificados = item.Item6.ToString(),
                                CodigoGeneracion = certificate.CodigoGeneracion,
                                IdCertificado = certificate.IdCertificado,
                                MatriculaEst = certificate.MatriculaEstablecimiento,
                                matricula = transaction.Enrollment,
                                tpcm = transaction.Tpcm,
                                EstablishCertificate = new EstablishCertificate
                                {
                                    CertificateCost = decimal.Parse(certificate.ValorCertificado),
                                    CertificateId = certificate.IdCertificado,
                                    EstablishEnrollment = certificate.MatriculaEstablecimiento,
                                    GenerationCode = certificate.CodigoGeneracion
                                }
                            });
                        }
                    }
                }
                transaction.Products = products;
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
            return transaction;
        }
    }
}
