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
    class AUTOPING : ViewModelBase
    {
        public static event Delegate_Print Event_Print;
        List<ParamCanal> myLIST;

        public static Task task1;

        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken cancellationToken;
        CancellationTokenSource cts2 = new CancellationTokenSource();
        CancellationToken cancellationToken2;

        //примеры отмены задачи
        // cancellationToken.ThrowIfCancellationRequested();
        //throw new OperationCanceledException(cancellationToken);
        //throw new OperationCanceledException();
        //throw new OperationCanceledException(CancellationToken.None);
        //throw new OperationCanceledException(new CancellationToken(true));
        //throw new OperationCanceledException(new CancellationToken(false));

        public AUTOPING()
        {
            cancellationToken = cts.Token;//для task1
            cancellationToken2 = cts2.Token;
        }

        bool _iswork = false;
        public bool iswork
        {
            get
            {
                return _iswork;
            }
            set
            {
                _iswork = true;
            }
        }

        public void stop()
        {
            _iswork = false; 
        }

        public async Task<string> AsyncTaskSTART(CancellationToken cancellationToken, string url)
        {
            string rez="";
            _iswork = true;
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            task1 = Task.Run(() =>
            {
       
                rez = ping_all(cancellationToken, myLIST);
                return rez;

            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

            try
            {
                await task1;
                if (task1.Status == TaskStatus.Canceled) { MessageBox.Show("AUTOtask1  Cancelled befor start"); }

                //MessageBox.Show("AUTOtask1  end Success");

            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("AUTOtask1 Cancelled");
            }
            catch (Exception e)
            {
                MessageBox.Show("AUTOtask1 Error: {0}", e.Message);

            }


            return rez;
            
        }

     
        public async void start()
        {
            myLIST = new List<ParamCanal>();//ПОСЛЕ ФИЛЬТРА
            try
            {

                foreach (var i in ViewModelMain.myLISTbase)
                {
                    myLIST.Add(i);
                }

                string ss = await AsyncTaskSTART(cancellationToken, "");

            }
            catch (Exception ex)
            {

                MessageBoxResult result = MessageBox.Show(ex.ToString(), " ",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

       
        string  ping_all(CancellationToken cancellationToken, List<ParamCanal> myLIST)
        {
            int ct_channel = 0;

            data.ping_all = myLIST.Count;

            foreach (var i in myLIST)
            {
                
                ct_channel++;
                data.ct_ping = ct_channel;

                if (i.http == null || i.http == "") continue;
                if (Event_Print != null) Event_Print("ping "+i.name);
                if (!iswork) {  break; };


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
                if (item == null) return "??";
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
