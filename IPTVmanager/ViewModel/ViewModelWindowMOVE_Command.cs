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

       public static event Delegate_UpdateMOVE Event_UpdateCollection;
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
            if (loc.lokUP || data.canal.name=="") return;
           
            ParamCanal pred = new ParamCanal();
            ParamCanal curr = new ParamCanal();

            pred.name = "";
            //находим предыдущий в фильтрованном списке 
            foreach (var obj in myLISTbase)
            {

                if (obj.Compare() == data.canal.Compare())
                {
                    curr = obj;
                    // curr=(ParamCanal)obj.Clone(); 
                    break;
                }
                else
                {
                    pred = obj;
                    //pred = (ParamCanal)obj.Clone();
                }
            }


            if (pred.name != "")
            {
                int i1 = myLISTfull.IndexOf(pred);
                int i2 = myLISTfull.IndexOf(curr);

                if (i1 >= 0 && i1 <= myLISTfull.Count())
                    if (i2 >= 0 && i2 <= myLISTfull.Count())
                    {
                        myLISTfull[i1] = curr; //(ParamCanal)curr.Clone();
                        myLISTfull[i2] = pred; //(ParamCanal)pred.Clone();
                    }
            }


            data.canal.name = "";
            if (Event_UpdateCollection != null) Event_UpdateCollection(curr);
            if (Event_SELECT != null) { Event_SELECT(1, curr);  }

            loc.lokUP = false;
            loc.lokDN = false;

        }

        /// <summary>
        /// DN
        /// </summary>
        void dn(object selectedItem)
        {
            if (loc.lokDN || data.canal.name == "") return;

            ParamCanal nxt = new ParamCanal();
            ParamCanal curr = new ParamCanal();

            nxt.name = "";
            bool find_ok = false;
            //находим следующий в фильтрованном списке 
            foreach (var obj in myLISTbase)
            {
                if (find_ok)
                {
                    nxt = obj;
                   // nxt = (ParamCanal)obj.Clone();
                    break;
                }


                if (obj.Compare()==data.canal.Compare())     
                {
                   curr = obj;// (ParamCanal)obj.Clone();
                   find_ok = true;
                }
               
            }


            if (nxt.name != "")
            {
                int i1 = myLISTfull.IndexOf(nxt);
                int i2 = myLISTfull.IndexOf(curr);

                if (i1 >= 0 && i1 <= myLISTfull.Count())
                    if (i2 >= 0 && i2 <= myLISTfull.Count())
                    {
                        myLISTfull[i1] = curr; //(ParamCanal)curr.Clone();
                        myLISTfull[i2] = nxt; //(ParamCanal)nxt.Clone();
                    }
            }


            data.canal.name = "";
            if (Event_UpdateCollection != null) Event_UpdateCollection(curr);
            if (Event_SELECT != null) { Event_SELECT(1, curr); }

        }
    }
}
