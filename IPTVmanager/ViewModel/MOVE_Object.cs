using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPTVman.Helpers;
using IPTVman.Model;

namespace IPTVman.ViewModel
{
    partial class ViewModelWindow2 : ViewModelMain 
    {
        public static ParamCanal p { get; set; }


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


    }
}
