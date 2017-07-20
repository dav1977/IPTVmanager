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


 
        public bool CH1
        {
            get { return data.type_player; }
            set
            {
                if (data.type_player) data.type_player = false; else data.type_player = true;
                RaisePropertyChanged("CH1");
            }
        }
    


    }
}
