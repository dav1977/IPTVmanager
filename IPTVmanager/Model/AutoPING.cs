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
        List<ParamCanal> myLIST;

        public AUTOPING()
        {
        }

        public Task<string> AsyncTaskSTART(string url)
        {

            return Task.Run(() =>
            {
                //----------------

                return ping_all(myLIST);

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
            myLIST = new List<ParamCanal>();//ПОСЛЕ ФИЛЬТРА
            try
            {

                foreach (var i in ViewModelMain.myLISTbase)
                {
                    myLIST.Add(i);
                }

                RUN("");

            }
            catch (Exception ex)
            {

                MessageBoxResult result = MessageBox.Show(ex.ToString(), " ",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

       
        string  ping_all(List<ParamCanal> myLIST)
        {
            int ct_channel = 0;

            data.ping_all = myLIST.Count;

            foreach (var i in myLIST)
            {
                ct_channel++;
                data.ct_ping = ct_channel;

                if (i.http == null || i.http == "") continue;
                if (Event_Print != null) Event_Print("ping "+i.name);

                _ping.done = false;
                string rez= _pingPREPARE.GET(i.http);
                if (rez != "НЕ ПОДДЕРЖИВАЕТСЯ")
                {
                    int ct = 0;
                    ct = 0;
                    while (!_ping.done)
                    {
                        Thread.Sleep(200);
                        ct++; data.ping_waiting = ct;
                        if (ct > 5 * 120) { if (Event_Print != null) Event_Print("timeout " + i.name); break; }
                    }
                }
                var item = ViewModelMain.myLISTfull.Find(x => x.Compare() == i.Compare());
           
                if (data.ping_waiting >20) if (Event_Print != null) Event_Print("        time " + String.Format("{0}", data.ping_waiting-20));

                data.ping_waiting = 0;
                if (_ping.result.Length < 2000)
                {
                    string tmp= _ping.result.Replace('\r', ';');
                    tmp = tmp.Replace("#EXTM3U;", "");
                    tmp = tmp.Replace("#EXTM3U", "");

                    item.ping = tmp.Replace('\n', ';');
                    if (rez == "НЕ ПОДДЕРЖИВАЕТСЯ") { item.ping = "НЕ ПОДДЕРЖИВАЕТСЯ"; if (Event_Print != null) Event_Print("НЕ ПОДДЕРЖИВАЕТСЯ " + i.name); }
                 
                }
                else item.ping = "большой размер ответа " + _ping.result.Length.ToString();


            }

            if (Event_Print != null) Event_Print("end");
            return "";
        }

    }



}
