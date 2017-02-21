using System;
using System.ComponentModel;

namespace MvvmExample.Model
{
    public class Person : INotifyPropertyChanged
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

        string _param3;
        public string param3
        {
            get
            {
                return _param3;
            }
            set
            {
                if (_param3 != value)
                {
                    _param3 = value;
                    RaisePropertyChanged("param3");
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
            if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
