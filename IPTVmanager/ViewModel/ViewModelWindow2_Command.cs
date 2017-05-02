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

            //p = new ParamCanal
            //{
            //    name = data.edit.name,
            //    ExtFilter = data.edit.ExtFilter,
            //    group_title = data.edit.group_title,
            //    http = data.edit.http,
            //    logo = data.edit.logo,
            //    tvg_name = data.edit.tvg_name,
            //    ping = data.edit.ping
            //};
            p = data.edit;

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

                if (obj.name == data.edit.name && obj.http == data.edit.http
                    && obj.ExtFilter == data.edit.ExtFilter && obj.group_title==data.edit.group_title)
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


                if (obj.name == data.edit.name && obj.http == data.edit.http
                    && obj.ExtFilter == data.edit.ExtFilter && obj.group_title == data.edit.group_title)
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
