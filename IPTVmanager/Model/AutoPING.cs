using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Threading;
using IPTVman.Helpers;
using IPTVman.Model;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace IPTVman.ViewModel
{
    class AUTOPING: ViewModelBase 
    {
        public static event Delegate_Print Event_Print;
        List<ParamCanal> LST;

        public AUTOPING()
        {
            
            
        }

        public Task<string> AsyncTaskSTART(string url)
        {

            return Task.Run(() =>
            {
                //----------------

                return ping_all(LST);

                //----------------
            });
        }

        async void RUN(string x)
        {

           // data.start_ping = true;
            string ss = await AsyncTaskSTART(x);

        }

        public void start()
        {
            LST = new List<ParamCanal>();//ПОСЛЕ ФИЛЬТРА
            try
            {

                foreach (var i in ViewModelMain.myLISTbase)
                {
                    LST.Add(i);
                }

                RUN("");


             

            }
            catch (Exception ex)
            {

                MessageBoxResult result = MessageBox.Show(ex.ToString(), " ",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }



        string  ping_all(List<ParamCanal> LST)
        {
            int iu = 0;

            foreach (var i in LST)
            {
                iu++;
                if (iu >= LST.Count) { if (Event_Print != null) Event_Print("end");  return ""; }

      

                _ping.result77 = "";
                if (i.http == null || i.http == "") continue;
                if (Event_Print != null) Event_Print("ping "+i.name);

                _pingPREPARE.GET(i.http);


                byte ct = 0;
                ct = 0;
                while (_ping.result77 == "") { Thread.Sleep(200); ct++; if (ct > 5* 10) break; }

                var item = ViewModelMain.myLISTfull.Find(x => x.Compare() == i.Compare());
                item.ping =  _ping.result77;

                //if (Event_Print != null) Event_Print(_ping.result77);

               // MessageBox.Show(i.name+"\n" +_ping.result77);

                    // item = ViewModelMain.myLISTbase.Find(x => x == i);
                    //item.ping = _ping.result77;

                    //RaisePropertyChanged("mycol");


                    // UPDATE_FILTER("");

            }


            return "";
        }

    }



}
