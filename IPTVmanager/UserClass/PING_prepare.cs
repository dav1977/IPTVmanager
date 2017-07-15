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
    public class PING_prepare
    {
        public static Task task1;

        public static CancellationTokenSource cts;
        public static CancellationToken cancellationToken;
        public static CancellationTokenSource cts2;
        public static CancellationToken cancellationToken2;

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
            return "старт...";//ip + ViewModelBase._ping.result;
        }

        public async Task<string> AsyncTaskGet(CancellationToken cancellationToken, string url)
        {
            _ping.iswork = true;
            string rez = "";

            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            string except = "";
            task1 = Task.Run(() => 
            {
                try
                {
                    rez = _ping.GETnoas(cancellationToken, url);
                }
                catch (OperationCanceledException e)
                {
                    except += e.Message.ToString();
                }
                catch (Exception e)
                {
                    except += e.Message.ToString();
                }
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try
            {
                await  task1;
            }
            catch (OperationCanceledException e)
            {
                except += e.Message.ToString();
            }
            catch (Exception e)
            {
                except += e.Message.ToString();
            }
            //dialog.Show("Статус закрытя "+ task1.Status.ToString());
            if (except != "") dialog.Show("ОШИБКА " + except);
            return rez;
        }

         void GETasyn(string url)
        {
            data.start_ping = true;
            string rez =  AsyncTaskGet(cancellationToken, url).ToString();
        }


        public bool stateTASKisCanceled()
        {
            if (task1==null) return true;
            if (task1.Status == TaskStatus.RanToCompletion) return true;
            else
            { _ping.stop();   return false; }

        }
    }
}
