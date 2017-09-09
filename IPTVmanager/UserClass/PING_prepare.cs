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
    public class PING_prepare
    {
        public static Task task1;

        public static CancellationTokenSource cts;
        public static CancellationToken cancellationToken;
        public static CancellationTokenSource cts2;
        public static CancellationToken cancellationToken2;

        /// <summary>
        /// вторая копия ссылки первая в ViewModelWindow1._ping 
        /// </summary>
        PING _ping;
        //примеры отмены задачи
        // cancellationToken.ThrowIfCancellationRequested();
        //throw new OperationCanceledException(cancellationToken);
        //throw new OperationCanceledException();
        //throw new OperationCanceledException(CancellationToken.None);
        //throw new OperationCanceledException(new CancellationToken(true));
        //throw new OperationCanceledException(new CancellationToken(false));

        public PING_prepare(PING p)
        {
            cts = new CancellationTokenSource();
            cts2 = new CancellationTokenSource();

            _ping = p;
             cancellationToken = cts.Token;//для task1
             cancellationToken2 = cts2.Token;
        }

        public void stop()
        {
            cts.Cancel();
       
        }
        /// <summary>
        /// ВОЗВРАЩАЕТ ОТВЕТ СЕРВЕРА ПО url
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public string GET(string u)
        {
            _ping.result = "";
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
                 GETasyn(u);//асинхр
            }
            else
            {
                return "НЕ ПОДДЕРЖИВАЕТСЯ";
            }
            return "  Cтарт...";//ip + ViewModelBase._ping.result;
        }


        void GETasyn(string url)
        {
            Thread.Sleep(100);//чтобы старт успела появиться
            string rez = AsyncTaskGet(cancellationToken, url).ToString();
        }


        public async Task<string> AsyncTaskGet(CancellationToken cancellationToken, string url)
        {
            _ping.iswork = true;

            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            string result = "";
            task1 = Task.Run(() => 
            { 
                var tcs = new TaskCompletionSource<string>();
                try
                {
                    result = _ping.GETnoas(cancellationToken, url);
                }
                catch (OperationCanceledException e)
                {
                    _ping.stop();
                    _ping.exit("");
                    tcs.SetException(e);
                }
                catch (Exception e)
                {
                    _ping.stop();
                    _ping.exit("");
                    tcs.SetException(e);
                }
                return tcs.Task;
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try { await task1; }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА ping pingPrepare " + e.Message.ToString());
                _ping.stop();//без ошибки должна выполниться шататно
            }
            if (_ping.iswork) { _ping.iswork = false; }

            //  ViewModelWindow1._ping = null;//уничтожаем первую ссылку на экземпляр
            // _ping = null;//уничтожаем вторую ссылку на экземпляр
            Wait.Close();
            task1.Dispose();
            task1 = null;
            return result;
        }

     
        public bool stateTASKisCanceled()
        {
            if (task1==null) return true;
            if (task1.Status == TaskStatus.RanToCompletion) return true;
            else
            {
                if (_ping!=null)_ping.stop();
                return false;
            }

        }
    }
}
