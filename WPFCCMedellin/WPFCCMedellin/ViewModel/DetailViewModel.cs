using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using WPFCCMedellin.Classes;
using WPFCCMedellin.Models;
using WPFCCMedellin.Resources;

namespace WPFCCMedellin.ViewModel
{
    class DetailViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private string _row1;

        public string Row1
        {
            get
            {
                return _row1;
            }
            set
            {
                if (_row1 != value)
                {
                    _row1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row1)));
                }
            }
        }

        private string _row2;

        public string Row2
        {
            get
            {
                return _row2;
            }
            set
            {
                if (_row2 != value)
                {
                    _row2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row2)));
                }
            }
        }

        private string _row3;

        public string Row3
        {
            get
            {
                return _row3;
            }
            set
            {
                if (_row3 != value)
                {
                    _row3 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row3)));
                }
            }
        }

        private string _row4;

        public string Row4
        {
            get
            {
                return _row4;
            }
            set
            {
                if (_row4 != value)
                {
                    _row4 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row4)));
                }
            }
        }

        private string _row5;

        public string Row5
        {
            get
            {
                return _row5;
            }
            set
            {
                if (_row5 != value)
                {
                    _row5 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row5)));
                }
            }
        }

        private string _row6;

        public string Row6
        {
            get
            {
                return _row6;
            }
            set
            {
                if (_row6 != value)
                {
                    _row6 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row6)));
                }
            }
        }

        private string _row7;

        public string Row7
        {
            get
            {
                return _row7;
            }
            set
            {
                if (_row7 != value)
                {
                    _row7 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row7)));
                }
            }
        }

        private string _row8;

        public string Row8
        {
            get
            {
                return _row8;
            }
            set
            {
                if (_row8 != value)
                {
                    _row8 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row8)));
                }
            }
        }

        private string _row9;

        public string Row9
        {
            get
            {
                return _row9;
            }
            set
            {
                if (_row9 != value)
                {
                    _row9 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row9)));
                }
            }
        }

        private string _row10;

        public string Row10
        {
            get
            {
                return _row10;
            }
            set
            {
                if (_row10 != value)
                {
                    _row10 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row10)));
                }
            }
        }

        private string _value1;

        public string Value1
        {
            get
            {
                return _value1;
            }
            set
            {
                if (_value1 != value)
                {
                    _value1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value1)));
                }
            }
        }

        private string _value2;

        public string Value2
        {
            get
            {
                return _value2;
            }
            set
            {
                if (_value2 != value)
                {
                    _value2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value2)));
                }
            }
        }

        private string _value3;

        public string Value3
        {
            get
            {
                return _value3;
            }
            set
            {
                if (_value3 != value)
                {
                    _value3 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value3)));
                }
            }
        }

        private string _value4;

        public string Value4
        {
            get
            {
                return _value4;
            }
            set
            {
                if (_value4 != value)
                {
                    _value4 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value4)));
                }
            }
        }

        private string _value5;

        public string Value5
        {
            get
            {
                return _value5;
            }
            set
            {
                if (_value5 != value)
                {
                    _value5 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value5)));
                }
            }
        }

        private string _value6;

        public string Value6
        {
            get
            {
                return _value6;
            }
            set
            {
                if (_value6 != value)
                {
                    _value6 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value6)));
                }
            }
        }

        private string _value7;

        public string Value7
        {
            get
            {
                return _value7;
            }
            set
            {
                if (_value7 != value)
                {
                    _value7 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value7)));
                }
            }
        }

        private string _value8;

        public string Value8
        {
            get
            {
                return _value8;
            }
            set
            {
                if (_value8 != value)
                {
                    _value8 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value8)));
                }
            }
        }

        private string _value9;

        public string Value9
        {
            get
            {
                return _value9;
            }
            set
            {
                if (_value9 != value)
                {
                    _value9 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value9)));
                }
            }
        }

        private string _value10;

        public string Value10
        {
            get
            {
                return _value10;
            }
            set
            {
                if (_value10 != value)
                {
                    _value10 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value10)));
                }
            }
        }

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

        private ETypePayer _typePayer;
        public ETypePayer TypePayer
        {
            get
            {
                return _typePayer;
            }
            set
            {
                _typePayer = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TypePayer)));
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

        private List<MockupsModel> _optionsList;

        public List<MockupsModel> OptionsList
        {
            get { return _optionsList; }
            set
            {
                _optionsList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OptionsList)));
            }
        }

        private List<MockupsModel> _departmentList;

        public List<MockupsModel> DepartmentList
        {
            get { return _departmentList; }
            set
            {
                _departmentList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DepartmentList)));
            }
        }

        private CollectionViewSource _departmenEntries;

        public CollectionViewSource DepartmenEntries
        {
            get
            {
                return _departmenEntries;
            }
            set
            {
                _departmenEntries = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DepartmenEntries)));
            }
        }

        private List<MockupsModel> _cityList;

        public List<MockupsModel> CityList
        {
            get { return _cityList; }
            set
            {
                _cityList = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CityList)));
            }
        }

        private CollectionViewSource _cityEntries;

        public CollectionViewSource CityEntries
        {
            get
            {
                return _cityEntries;
            }
            set
            {
                _cityEntries = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CityEntries)));
            }
        }

        private CollectionViewSource _optionsEntries;

        public CollectionViewSource OptionsEntries
        {
            get
            {
                return _optionsEntries;
            }
            set
            {
                _optionsEntries = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OptionsEntries)));
            }
        }


        public void LoadList(ETypePayer type)
        {
            try
            {

                var response = Utilities.ConverJson<List<MockupsModel>>(Utilities.GetConfiguration("PathTypeDocument"));
                if (response != null && response.Count > 0)
                {
                    OptionsList.Clear();
                    OptionsList = response.FindAll(t => t.Type == (int)type);
                    OptionsEntries.Source = OptionsList;
                }
                var responseDepartmentList = Utilities.ConverJson<List<MockupsModel>>(Utilities.GetConfiguration("PathDepartmentList"));
                if (responseDepartmentList != null && responseDepartmentList.Count > 0)
                {
                    DepartmentList.Clear();
                    DepartmentList = responseDepartmentList;
                    DepartmenEntries.Source = DepartmentList;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }

        public void LoadListCity(List<MockupsModel> responseCityList)
        {
            try
            {
                if (responseCityList != null && responseCityList.Count > 0)
                {
                    CityList.Clear();
                    CityList = responseCityList;
                    CityEntries.Source = CityList;
                }
            }
            catch (Exception ex)
            {
                Error.SaveLogError(MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex, MessageResource.StandarError);
            }
        }
    }
}
