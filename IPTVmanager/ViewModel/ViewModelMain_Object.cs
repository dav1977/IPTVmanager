using System;
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
                
                UPDATE_FILTER();
                if (ACOLL!=null) ACOLL.UPDATE();
                
               
                if (Event_UpdateLIST != null) Event_UpdateLIST(myLISTbase.Count);
                RaisePropertyChanged("numberCANALS");
                return ACOLL;
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
                return "Всего каналов: " + myLISTfull.Count.ToString() +"   Отфильтрованных="+myLISTbase.Count.ToString();
            }
       
        }


        public object memory
        {
            get
            {
                return "Memory Usage: " + string.Format("{0:0.00} MB", GC.GetTotalMemory(true) / 1024.0 / 1024.0);
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

 
        static string _filter="";
        public string filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (_filter != value)
                {
                    _filter = value; 
                    //RaisePropertyChanged("filter");
                }
            }
        }

    }//class

}//namespace
