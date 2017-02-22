using System;
using System.ComponentModel;

namespace IPTVman.Model
{
    public class ParamCanal : INotifyPropertyChanged//, IComparable<ParamCanal>
    {
        string _FirstName;
        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                if (_FirstName != value)
                {
                    _FirstName = value;
                    RaisePropertyChanged("FirstName");
                }
            }
        }

        string _LastName;
        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                    RaisePropertyChanged("LastName");
                }
            }
        }

        string _http;
        public string http
        {
            get
            {
                return _http;
            }
            set
            {
                if (_http != value)
                {
                    _http = value;
                    RaisePropertyChanged("http");
                }
            }
        }


        int _Age;
        public int Age
        {
            get
            {
                return _Age;
            }
            set
            {
                if (_Age != value)
                {
                    _Age = value;
                    RaisePropertyChanged("Age");
                }
            }
        }

        void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
       
            }//вызывется событие  изменение параметра
        }


        public event PropertyChangedEventHandler PropertyChanged;//событие глобальное изменение параметра




        /// <summary>
        /// сортировка
        /// </summary>
        public string sort
        {
            get { return _foo; }
            set { _foo = value; PropertyChanged2(this, new PropertyChangedEventArgs("Foo")); }
        }
        private string _foo;

        public int CompareTo(ParamCanal other)
        {
            if (other == null) return -1;
            return string.Compare(this.sort, other.sort, ignoreCase: true);
        }

        public override string ToString() { return sort; }
        public event PropertyChangedEventHandler PropertyChanged2 = delegate { };


    }
}
