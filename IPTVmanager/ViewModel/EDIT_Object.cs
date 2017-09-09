using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPTVman.Helpers;
using IPTVman.Model;

namespace IPTVman.ViewModel
{
    partial class ViewModelWindow1 : ViewModelMain
    {
        public static ParamCanal edit { get; set; }
        
        object _strPING;
        public object strPING
        {
            get
            {
                return _strPING;
            }
            set
            {
                if (_strPING != value)
                {
                    _strPING = value;
                    RaisePropertyChanged("strPING");
                }
            }
        }

        object _path;
        public object path
        {
            get
            {
                return _path;
            }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    RaisePropertyChanged("path");
                }
            }
        }

        public static bool _ch1 = false;
        public bool CH1
        {
            get { return _ch1; }
            set
            {
                if (_ch1) { _ch1 = false; data.type_player = 0; }
                else { _ch1 = true; _ch2 = false; data.type_player = 1; }  
                RaisePropertyChanged("CH1");
                RaisePropertyChanged("CH2");
            }
        }

        public static bool _ch2 = false;
        public bool CH2
        {
            get { return _ch2; }
            set
            {
                if (_ch2) { _ch2 = false; data.type_player = 0; }
                else { _ch2 = true; _ch1 = false; data.type_player = 2; }
                RaisePropertyChanged("CH1");
                RaisePropertyChanged("CH2");
            }
        }

    }
}
