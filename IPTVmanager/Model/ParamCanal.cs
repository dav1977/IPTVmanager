using System;
using System.ComponentModel;

namespace IPTVman.Model
{


    //public class Customer
    //{
    //    /// <summary>
    //    /// Gets or sets the id.
    //    /// </summary>
    //    /// <value>The id.</value>
    //    public string Title { get; set; }

    //    /// <summary>
    //    /// Gets or sets the name.
    //    /// </summary>
    //    /// <value>The name.</value>
    //    public string ExtFilter { get; set; }

    //    /// <summary>
    //    /// Some dummy data to give the instance a bigger memory footprint.
    //    /// </summary>
    //    //private byte[] data = new byte[100];
    //}


    public class ParamCanal : INotifyPropertyChanged//, IComparable<ParamCanal>
    {

        /// <summary>
        /// Some dummy data to give the instance a bigger memory footprint.
        /// </summary>
        //private byte[] data = new byte[100];

        string _name;
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("name");
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




        string _group_title;
        public string group_title
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


        string _tvg;
        public string tvg
        {
            get
            {
                return _tvg;
            }
            set
            {
                if (_tvg != value)
                {
                    _tvg = value;
                    RaisePropertyChanged("tvg");
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



        string _logo;
        public string logo
        {
            get
            {
                return _logo;
            }
            set
            {
                if (_logo != value)
                {
                    _logo = value;
                    RaisePropertyChanged("logo");
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
