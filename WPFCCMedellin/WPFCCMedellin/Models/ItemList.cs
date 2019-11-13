using System.ComponentModel;

namespace WPFCCMedellin.Models
{
    class ItemList : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        private string _item1;

        public string Item1
        {
            get
            {
                return _item1;
            }
            set
            {
                if (_item1 != value)
                {
                    _item1 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item1)));
                }
            }
        }

        private string _item2;

        public string Item2
        {
            get
            {
                return _item2;
            }
            set
            {
                if (_item2 != value)
                {
                    _item2 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item2)));
                }
            }
        }

        private string _item3;

        public string Item3
        {
            get
            {
                return _item3;
            }
            set
            {
                if (_item3 != value)
                {
                    _item3 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item3)));
                }
            }
        }

        private string _item4;

        public string Item4
        {
            get
            {
                return _item4;
            }
            set
            {
                if (_item4 != value)
                {
                    _item4 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item4)));
                }
            }
        }

        private decimal _item5;

        public decimal Item5
        {
            get
            {
                return _item5;
            }
            set
            {
                if (_item5 != value)
                {
                    _item5 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item5)));
                }
            }
        }

        private int _item6;

        public int Item6
        {
            get
            {
                return _item6;
            }
            set
            {
                if (_item6 != value)
                {
                    _item6 = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item6)));
                }
            }
        }

        private int _index;

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Index)));
                }
            }
        }

        private string _imageSourse;

        public string ImageSourse
        {
            get
            {
                return _imageSourse;
            }
            set
            {
                if (_imageSourse != value)
                {
                    _imageSourse = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ImageSourse)));
                }
            }
        }

        private object _data;

        public object Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (_data != value)
                {
                    _data = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
                }
            }
        }
    }
}
