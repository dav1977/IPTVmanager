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
using System.Diagnostics;

namespace IPTVman.ViewModel
{
    class AUTOPING : ViewModelBase
    {
        PING _ping;
        PING_prepare _pingPREPARE;
        public static event Action<string> Event_Print;
        List<ParamCanal> myLIST;
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
            data.ping_waiting = 0;
            taskAP = null;
            Wait.Close();
            loc.autoping = false;
        }


        /// <summary>
        /// событие после выполнения задачи
        /// </summary>
        Task<string> taskAP;
        public event Action Task_Completed;
        private object threadLock = new object();
        public async Task<string> start()
        {
            var tcs = new TaskCompletionSource<string>();
            if (loc.autoping) return "";
            cancellationToken = cts1.Token;
            if (myLIST == null) myLIST = new List<ParamCanal>();
            myLIST.Clear();
            foreach (var i in ViewModelMain.myLISTbase)
            {
                myLIST.Add(i);
            }

            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            string result = "";
            taskAP = Task.Run(() =>
            {
                try
                {
                    Trace.WriteLine("task ping all");
                    result = ping_all(cancellationToken, myLIST);
                    tcs.SetResult("ok");
                }
                catch (OperationCanceledException e)
                {
                    tcs.SetException(e);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                
                return tcs.Task;
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try
            {
                await taskAP;
                if (Task_Completed != null) Task_Completed();
                Dispose();
            }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА-АВТОПИНГ " + e.Message.ToString());
                Dispose();
            }
            return result;
        }


        /// <summary>
        /// PING всех с цикле
        /// </summary>
        string result_task = "";
        string  ping_all(CancellationToken token, List<ParamCanal> myLIST)
        {
            if (loc.autoping) return "";
            loc.autoping = true;
            int ct_channel = 0;
            data.ping_all = myLIST.Count;
            //Trace.WriteLine("----Старт ...");     
            if (Event_Print != null) Event_Print("Старт ...");
            foreach (var i in myLIST)
            {
                //Trace.WriteLine("p="+i.name.ToString()+"  size="+ myLIST.Count);
                if (token.IsCancellationRequested) { break; };
                ct_channel++;
                data.ct_ping = ct_channel;
                if (Event_Print != null) Event_Print(" ping " + i.name.ToString());
               //Trace.WriteLine(data.ct_ping.ToString() + " ping " + i.name.ToString());

                if (i.http == null || i.http == "") continue;
               
                _ping.done = false;
                _pingPREPARE.Task_Completed += _pingPREPARE_Task_Completed;
                result_task = "";

                Regex regex1 = new Regex("http:");
                Regex regex2 = new Regex("https:");
                Regex regex3 = new Regex("udp:");
                Regex regex4 = new Regex("rtmp:");
                Regex regex5 = new Regex("mms:");

                var match1 = regex1.Match(i.http);
                var match2 = regex2.Match(i.http);
                var match3 = regex3.Match(i.http);
                var match4 = regex4.Match(i.http);
                var match5 = regex5.Match(i.http);

                if (match1.Success || match2.Success || match3.Success || match4.Success || match5.Success)
                {
                    int ct = 0;
                    _pingPREPARE.asyncGET(i.http);
                    while (result_task == "")
                    {
                        Thread.Sleep(200);
                        ct++;
                        data.ping_waiting = ct;
                        if (ct > 100)
                        { result_task = "тайм аут"; break;
                        }
                        if (token.IsCancellationRequested) { break; };
                    }
                }
                else result_task = "НЕ ПОДДЕРЖИВАЕТСЯ";

                var item = ViewModelMain.myLISTfull.Find(x => x.Compare() == i.Compare());

                data.ping_waiting = 0;
                if (item != null)
                {
                    if (_ping.result.Length < 2000)
                    {
                        string tmp = _ping.result.Replace('\r', ';');
                        tmp = tmp.Replace("#EXTM3U;", "");
                        tmp = tmp.Replace("#EXTM3U", "");

                        item.ping = tmp.Replace('\n', ';');

                        if (result_task == "НЕ ПОДДЕРЖИВАЕТСЯ" || result_task == "тайм аут")
                        {
                            item.ping = result_task;
                            if (Event_Print != null) Event_Print(result_task+" " + i.name);
                        }
                    }
                    else item.ping = "большой размер ответа " + _ping.result.Length.ToString();
                }

            }//for
            if (Event_Print != null) Event_Print("end");
            return "";
        }

        private void _pingPREPARE_Task_Completed(string rez)
        {
            result_task = rez;
        }
    }



}
