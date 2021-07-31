﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace IPTVman.ViewModel
{
    partial class ViewModelMain : ViewModelBase
    {
        //******************************************
        // Объект коллекция
        //********************************************
        public object mycol
        {
            get
            {
                //if (false)
                // {
                //     //DataContext = new List<Customer>(customerProvider.FetchRange(0, customerProvider.FetchCount()));
                // }
                // else if (false)
                // {
                //     //DataContext = new VirtualizingCollection<Customer>(customerProvider, pageSize);
                // }
                // else if (true)
                // {
                //  RaisePropertyChanged("mycol");
                
               // UPDATE_FILTER();
                if (ACOLL!=null) ACOLL.UPDATE();
                
               
                if (Event_UpdateLIST != null && myLISTbase!=null) Event_UpdateLIST(myLISTbase.Count);
                RaisePropertyChanged("numberCANALS");
                RaisePropertyChanged("CH1m");
                return ACOLL;
            }
        }



        /// <summary>
        /// только обновлять
        /// </summary>
        public static bool chek_upd;
        public bool CH1m
        {
            get { return chek_upd; }
            set
            {
                if (chek_upd) chek_upd = false;  else chek_upd = true;
                Model.Script.enable_update = chek_upd;
                RaisePropertyChanged("CH1m");
            }
        }
        /// <summary>
        /// обрезать скобки
        /// </summary>
        bool chek__hoop=true;
        public bool CH2m
        {
            get { return chek__hoop; }
            set
            {
                if (chek__hoop) chek__hoop = false; else chek__hoop = true;
                RaisePropertyChanged("CH2m");
            }
        }

        object _best1;
        public object best1
        {
            get
            {
                return _best1;
            }
            set
            {
                if (_best1 != value)
                {
                    _best1 = value;
                    _update.UPDATE_BEST(_best1.ToString(), null);
                    RaisePropertyChanged("best1");

                }
            }
        }
        object _best2;
        public object best2
        {
            get
            {
                return _best2;
            }
            set
            {
                if (_best2 != value)
                {
                    _best2 = value;
                    _update.UPDATE_BEST(null, _best2.ToString());
                    RaisePropertyChanged("best2");

                }
            }
        }



        object _SelectedParamCanal;
        public object SelectedParamCanal
        {
            get
            {
                return _SelectedParamCanal;
            }
            set
            {
                if (_SelectedParamCanal != value)
                {
                    _SelectedParamCanal = value;

                    RaisePropertyChanged("SelectedParamCanal");

                }
            }
        }

        public object numberCANALS
        {
            get
            {
                if (myLISTbase == null) return "Всего каналов: 0";
                return "Всего каналов: " + myLISTfull.Count.ToString() +"   Отфильтрованных: "+myLISTbase.Count.ToString();
            }
       
        }


        public object memory
        {
            get
            {
                return "(C) 2017 Memory Usage: " + string.Format("{0:0.00} MB", GC.GetTotalMemory(true) / 1024.0 / 1024.0);
            }
         
        }


        string _newChannel;
        public string newChannel
        {
            get
            {
                return _newChannel;
            }
            set
            {
                if (_newChannel != value)
                {
                    _newChannel = value;
                    RaisePropertyChanged("newChannel");
                }
            }
        }



        string _text_title;
        public string text_title
        {
            get
            {
                return _text_title;
            }
            set
            {
                if (_text_title != value)
                {
                    _text_title = value;
                    RaisePropertyChanged("text_title");
                }
            }
        }


        static string _filter1 = "";
        public string filter1
        {
            get
            {
                return _filter1;
            }
            set
            {
                if (_filter1 != value)
                {
                    _filter1 = value;
               
                }
            }
        }

        static string _filter2 = "";
        public string filter2
        {
            get
            {
                return _filter2;
            }
            set
            {
                if (_filter2 != value)
                {
                    _filter2 = value;
             
                }
            }
        }
        static string _filter3 = "";
        public string filter3
        {
            get
            {
                return _filter3;
            }
            set
            {
                if (_filter3 != value)
                {
                    _filter3 = value;
           
                }
            }
        }

        static string _filter4 = "";
        public string filter4
        {
            get
            {
                return _filter4;
            }
            set
            {
                if (_filter4 != value)
                {
                    _filter4 = value;

                }
            }
        }

  

        /// <summary>
        /// Gets or sets a value indicating whether the collection is loading.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this collection is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoading
        {
            get
            {
                if (ACOLL == null) return false;
                else return ACOLL.IsLoading;
            }
            set
            {
              
            }
        }


    }//class

}//namespace
