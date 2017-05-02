using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPTVman.Helpers;
using IPTVman.Model;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Threading;


namespace IPTVman.ViewModel
{
    partial class ViewModelWindow2 : ViewModelMain
    { 
        public RelayCommand key_UPCommand { get; set; }
        public RelayCommand key_DNCommand { get; set; }


        public static event Delegate_Update2 Event_Update2;

        //============================== INIT ==================================
        public ViewModelWindow2(string lastText)
        {

            key_UPCommand = new RelayCommand(up);

            key_DNCommand = new RelayCommand(dn);


            //data.one_add = false;

            p = new ParamCanal
            {
                name = data.name,
                ExtFilter = data.extfilter,
                group_title = data.grouptitle,
                http = data.http,
                logo = data.logo,
                tvg_name = data.tvg,
                ping = data.ping
            };

            data.temp = p; 
        }
        //=============================================================================



        /// <summary>
        /// UP
        /// </summary>
        
        void up(object selectedItem)
        {
            if (data.lokUP || data.edit.name=="") return;
           
           
          
            int j = 0;
            ParamCanal pred = new ParamCanal();
            ParamCanal curr = new ParamCanal();

            pred.name = "";
            //находим предыдущий в фильтрованном списке 
            foreach (var obj in myLISTbase)
            {

                if (obj.name == data.name && obj.http == data.http)
                {
                    curr = obj;   j--; break;
                }
                else
                {
                    pred = obj;

                    pred.name = obj.name; pred.http = obj.http;
                    j++;
                }
            }


            if (pred.name != "")
            {
                int i1 = myLISTfull.IndexOf(pred);
                int i2 = myLISTfull.IndexOf(curr);

                 myLISTfull[i1] = curr;
                 myLISTfull[i2] = pred;
            }


            data.edit.name = "";
            if (Event_Update2 != null) Event_Update2(curr);

            data.lokUP = false;
            data.lokDN = false;

        }

        /// <summary>
        /// DN
        /// </summary>
        void dn(object selectedItem)
        {
            if (data.lokDN || data.edit.name == "") return;



            int j = 0;
            ParamCanal nxt = new ParamCanal();
            ParamCanal curr = new ParamCanal();

            nxt.name = "";
            bool find_ok = false;
            //находим следующий в фильтрованном списке 
            foreach (var obj in myLISTbase)
            {
                if (find_ok)
                {

                    nxt = obj; break;
                }

                if (obj.name == data.name && obj.http == data.http)
                {
                    curr = obj; find_ok = true;
                }
               
            }


            if (nxt.name != "")
            {
                int i1 = myLISTfull.IndexOf(nxt);
                int i2 = myLISTfull.IndexOf(curr);

                myLISTfull[i1] = curr;
                myLISTfull[i2] = nxt;
            }


            data.edit.name = "";
            if (Event_Update2 != null) Event_Update2(curr);


          
        }
    }
}
