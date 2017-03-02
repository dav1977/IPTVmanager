using System;
using System.ComponentModel;

namespace IPTVman.Model
{
    public class ParamCanal : INotifyPropertyChanged//, IComparable<ParamCanal>
    {
        string _Title;
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }

        string _ExtFilter;
        public string ExtFilter
        {
            get
            {
                return _ExtFilter;
            }
            set
            {
                if (_ExtFilter != value)
                {
                    _ExtFilter = value;
                    RaisePropertyChanged("ExtFilter");
                }
            }
        }




        int _group_title;
        public int group_title
        {
            get
            {
                return _group_title;
            }
            set
            {
                if (_group_title != value)
                {
                    _group_title = value;
                    RaisePropertyChanged("group_title");
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
