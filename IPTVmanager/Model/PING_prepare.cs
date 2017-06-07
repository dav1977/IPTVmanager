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



        public string GET(string u)
        {
            ViewModelBase._ping.result77 = "";
            Regex regex1 = new Regex("http:");
            Regex regex2 = new Regex("https:");

            var match1 = regex1.Match(u);
            var match2 = regex2.Match(u);

            if (match1.Success || match2.Success)
            {
                //if (task_ping != null)
                //    if (task_ping.Status == TaskStatus.Running) return "task is running";


                //ViewModelBase._ping.GETnoas(u);
          

                 GETasyn(u);//асинхр
                //         test(u);


            }
            else
            {

                //string ss= UDPtest("");
                // result77 = NEW_UDP();


                //if (result77 == "") result77 = "НЕТУ UDP";
                //result77=GETnoas(u);//синхр

                return "error";

            }
            return "--";//ip + ViewModelBase._ping.result77;
        }




        public Task<string> AsyncTaskGet(string url)
        {

            return Task.Run(() =>
            {
                //----------------

                return ViewModelBase._ping.GETnoas(url);

                //----------------
            });
        }



        async void GETasyn(string url)
        {

            data.start_ping = true;
            string ss = await AsyncTaskGet(url);

        }



        public Task task_ping;

        async void test(string url)
        {

            task_ping = Task.Run(async delegate
            {
                ViewModelBase._ping.GETnoas(url);
                await Task.Delay(TimeSpan.FromSeconds(3));

            });

            await Task.WhenAll(task_ping);


            // using (var cts = new CancellationTokenSource())
            //{
            //    var ct = cts.Token;
            //    var tasks =  Enumerable.Range(0, 30).Select(i => Task.Run(() => GETnoas(url), ct));

            //    cts.CancelAfter(TimeSpan.FromSeconds(3));



            //    try
            //    {
            //        //await Task.WhenAll(tasks);
            //        await Task.WhenAll(tasks);

            //        // await Task.Delay(5);
            //    }
            //    catch (AggregateException)
            //    {
            //        result77 = "errror";
            //    }
            //}

        }



    }
}
