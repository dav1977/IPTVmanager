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



/// <summary>
///   ОКНО ПЕРЕМЕЩЕНИЯ
/// </summary>
namespace IPTVman.ViewModel
{
    partial class ViewModelWindow2 : ViewModelMain
    {

       public static event Delegate_UpdateMOVE Event_UpdateAFTERmove;
       public static event Delegate_SelectITEM Event_SELECT;

        public RelayCommand key_UPCommand { get; set; }
        public RelayCommand key_DNCommand { get; set; }

        //============================== INIT ==================================
        public ViewModelWindow2(string lastText)
        {

            key_UPCommand = new RelayCommand(up);
            key_DNCommand = new RelayCommand(dn);
            p = data.canal;

        }
        //=============================================================================

        /// <summary>
        /// UP
        /// </summary>
        
        void up(object selectedItem)
        {
            if (data.lokUP || data.canal.name=="") return;
           
            int j = 0;
            ParamCanal pred = new ParamCanal();
            ParamCanal curr = new ParamCanal();

            pred.name = "";
            //находим предыдущий в фильтрованном списке 
            foreach (var obj in myLISTbase)
            {

                if (obj.name == data.canal.name && obj.http == data.canal.http
                    && obj.ExtFilter == data.canal.ExtFilter && obj.group_title==data.canal.group_title)
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


            data.canal.name = "";
            if (Event_UpdateAFTERmove != null) Event_UpdateAFTERmove(curr);
            Thread.Sleep(1000);
            if (Event_SELECT != null) { Event_SELECT(1, curr);  }

            data.lokUP = false;
            data.lokDN = false;

        }

        /// <summary>
        /// DN
        /// </summary>
        void dn(object selectedItem)
        {
            if (data.lokDN || data.canal.name == "") return;

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


                if (obj.name == data.canal.name && obj.http == data.canal.http
                    && obj.ExtFilter == data.canal.ExtFilter && obj.group_title == data.canal.group_title)
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


            data.canal.name = "";
            if (Event_UpdateAFTERmove != null) Event_UpdateAFTERmove(curr);

            Thread.Sleep(1000);
            if (Event_SELECT != null) { Event_SELECT(1, curr); }

        }
    }
}
