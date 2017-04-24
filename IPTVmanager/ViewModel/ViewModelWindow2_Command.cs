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

        bool lokUP = false;
        void up(object selectedItem)
        {
            if (lokUP) return;
            int index = 0;
           
            //находим выделенный в полном списке
            foreach (var obj in myLISTfull)
            {
                //var item = ViewModelMain.myLISTfull.Find(x =>
                // (x.http == obj.http && x.ExtFilter == obj.ExtFilter && x.group_title == obj.group_title));

                //if (item != null) { myLISTfull.Remove(item); }

                if (obj.name == data.name && obj.http == data.http)
                {
                    break;
                }
                index++;

            }

            int j = 0;
            ParamCanal pred = new ParamCanal();
            pred.name = "";
            //находим предыдущий в фильтрованном списке 
            foreach (var obj in myLISTbase)
            {

                if (obj.name == data.name && obj.http == data.http)
                {
                    j--; break;
                }
                else
                {
                    pred.name = obj.name; pred.http = obj.http;
                    j++;
                }
            }

            if (pred.name == "") return;

            int indexpred = 0;
            //находим предыдущий в полном списке
            foreach (var obj in myLISTfull)
            {

                if (obj.name == pred.name &&  obj.http == pred.http)
                {
                    break;
                }
                else
                {
                    indexpred++;
                }
            }

           

    
            MessageBox.Show("full=" + index.ToString() + "  pred=" + indexpred.ToString() +
                
                "    "+myLISTfull[index].name+"  pred="+myLISTfull[indexpred].name 
                , " ",

               
                           MessageBoxButton.OK, MessageBoxImage.Information);

            // myLISTfull.RemoveAt(index);

            data.temp.name += "NEW!!";
            myLISTfull.Insert(indexpred, data.temp);



             if (Event_Update2 != null) Event_Update2();

            Thread.Sleep(500);
            lokUP = false;
        }

        void dn(object selectedItem)
        {



        }
    }
}
