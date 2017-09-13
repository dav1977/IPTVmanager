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
        public void asyncGET(string u)
        {
           var rez = AsyncTaskGet(cancellationToken, u);    
        }


        /// <summary>
        /// событие после выполнения задачи
        /// </summary>
        public event Action<string> Task_Completed;
        async Task<string> AsyncTaskGet(CancellationToken cancellationToken, string url)
        {
            _ping.iswork = true;
            string result = "";
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            task1 = Task.Run(() => 
            {
                var tcs = new TaskCompletionSource<string>();
                _ping.result = "";
                               
                try
                {
                    result = _ping.GETnoas(cancellationToken, url);
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
                await task1;
                _ping.iswork = false;
                if (Task_Completed != null) Task_Completed(result);
                Wait.Close();
                task1.Dispose();
                task1 = null;
            }
            catch (Exception e)
            {
                _ping.stop();
                _ping.exit("");
                dialog.Show("ОШИБКА pingPrepare " + e.Message.ToString());
                _ping.stop();//без ошибки должна выполниться шататно
            }

            return "";
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
