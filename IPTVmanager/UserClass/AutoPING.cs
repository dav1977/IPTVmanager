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
        PING _ping;
        PING_prepare _pingPREPARE;
        public static event Delegate_Print Event_Print;
        List<ParamCanal> myLIST;
        public static Task task1;
        public static CancellationTokenSource cts1;
        public static CancellationToken cancellationToken;


        //примеры отмены задачи
        // cancellationToken.ThrowIfCancellationRequested();
        //throw new OperationCanceledException(cancellationToken);
        //throw new OperationCanceledException();
        //throw new OperationCanceledException(CancellationToken.None);
        //throw new OperationCanceledException(new CancellationToken(true));
        //throw new OperationCanceledException(new CancellationToken(false));



        public AUTOPING(PING p, PING_prepare b)
        {
            _ping = p;
            _pingPREPARE = b;
            cts1 = new CancellationTokenSource();       
            cancellationToken = cts1.Token;//для task1
        }

        public void stop()
        {
           if(cts1!=null) cts1.Cancel();
        }

        public void Dispose()
        {
            //int ct = 0;
            //while (true)
            //{
            //    ct++;
            //    if (ct > 10) { task1 = null; return result; }
            //    if (task1.IsCompleted) break;
            //    if (task1.IsFaulted) break;
            //    if (task1.IsCanceled) break;
            //    Thread.Sleep(500);

            //}
            //if (task1 != null) task1.Dispose();
            task1 = null;
            loc.collection = false;
            data.ping_waiting = 0;
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

                 await AsyncTaskSTART(cts1.Token);

            }
            catch (Exception ex)
            {
                dialog.Show("Ошибка autoping "+ex.ToString());
            }
        }
         private object threadLock = new object();
        public async Task<string> AsyncTaskSTART(CancellationToken cancellationToken)
        {

            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            string result = "";
            task1 = Task.Run(() =>
            {
                var tcs = new TaskCompletionSource<string>();
                try
                {
                    lock (threadLock)
                    {
                        result = ping_all(cancellationToken, myLIST);
                    }
                }
                catch (OperationCanceledException e)
                {
                    tcs.SetException(e);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                loc.collection = false;
                return tcs.Task;
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try { await task1; }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА-АВТОПИНГ " + e.Message.ToString());
            }
            if (cts1!=null)cts1.Cancel();
            Wait.Close();
           
            return result;
        }

       
        string  ping_all(CancellationToken token, List<ParamCanal> myLIST)
        {
            int ct_channel = 0;

            data.ping_all = myLIST.Count;

            loc.collection = true;
            foreach (var i in myLIST)
            {
                
                ct_channel++;
                data.ct_ping = ct_channel;

                if (i.http == null || i.http == "") continue;
                if (Event_Print != null) Event_Print("ping "+i.name);
                if (token.IsCancellationRequested) {  break; };


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

            loc.collection = false;
            if (Event_Print != null) Event_Print("end");
            return "";
        }

    }



}
