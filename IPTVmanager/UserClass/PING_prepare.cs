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
    class PING_prepare
    {
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

        public PING_prepare()
        {
             cancellationToken = cts.Token;//для task1
             cancellationToken2 = cts2.Token;
        }

        /// <summary>
        /// ВОЗВРАЩАЕТ ОТВЕТ СЕРВЕРА ПО url
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public string GET(string u)
        {
            ViewModelBase._ping.result = "";
            Regex regex1 = new Regex("http:");
            Regex regex2 = new Regex("https:");
            Regex regex3 = new Regex("udp:");
            Regex regex4 = new Regex("rtmp:"); 

            var match1 = regex1.Match(u);
            var match2 = regex2.Match(u);
            var match3 = regex3.Match(u);
            var match4 = regex4.Match(u);

            if (match1.Success || match2.Success || match3.Success || match4.Success)
            {

                //ViewModelBase._ping.GETnoas(u);
         
                 GETasyn(u);//асинхр
                //         test(u);

            }
            else
            {
                //string ss= UDPtest("");
                // result = NEW_UDP();


                //if (result == "") result = "НЕТУ UDP";
                //result=GETnoas(u);//синхр
                 
            
                return "НЕ ПОДДЕРЖИВАЕТСЯ";

            }
            return "--";//ip + ViewModelBase._ping.result;
        }




        public async Task<string> AsyncTaskGet(CancellationToken cancellationToken, string url)
        {
            ViewModelBase._ping.iswork = true;
            string rez = "";

            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            task1 = Task.Run(() => 
            {  
                rez = ViewModelBase._ping.GETnoas( cancellationToken, url);    
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&



            try
            {
                await  task1;
               // if (task1.Status == TaskStatus.Canceled) { dialog.Show("task1  Cancelled befor start"); }

            }
            catch (OperationCanceledException)
            {
                dialog.Show("task1 Cancelled");
            }
            catch (Exception e)
            {
                dialog.Show($"task1 Error: {e.Message}");

            }

            return rez;
        }



         void GETasyn(string url)
        {
      
            data.start_ping = true;
            string rez =  AsyncTaskGet(cancellationToken, url).ToString();

        }




    }
}
