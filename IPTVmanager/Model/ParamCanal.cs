using System;
using System.ComponentModel;

namespace IPTVman.Model
{

        public class ParamCanal : ICloneable , INotifyPropertyChanged //, IComparable<ParamCanal>
    {

        /// <summary>
        /// Some dummy data to give the instance a bigger memory footprint.
        /// </summary>
        //private byte[] data = new byte[100];

        public ParamCanal()
        {
        
                
        }

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

        string _playing;
        public string playing
        {
            get
            {
                return _playing;
            }
            set
            {
                if (_playing != value)
                {
                    _playing = value;
                    RaisePropertyChanged("playing");
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


        string _tvg_name;
        public string tvg_name
        {
            get
            {
                return _tvg_name;
            }
            set
            {
                if (_tvg_name != value)
                {
                    _tvg_name = value;
                    RaisePropertyChanged("tvg_name");
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


        string _ping;
        public string ping
        {
            get
            {
                return _ping;
            }
            set
            {
                if (_ping != value)
                {
                    _ping = value;
                    RaisePropertyChanged("ping");
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

        public object Clone()
        {
            return new ParamCanal
            {
                name = this.name,
                ExtFilter = this.ExtFilter,
                group_title = this.group_title,
                logo = this.logo,
                http = this.http,
                tvg_name = this.tvg_name,
                ping = this.ping,
            };
        }

        public string Compare()
        {
            return this.name + this.ExtFilter + this.group_title + this.logo + this.http + this.tvg_name;
        }


        public event PropertyChangedEventHandler PropertyChanged2 = delegate { };


    }
}
