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

namespace IPTVman.ViewModel
{
    class ViewModelBase : INotifyPropertyChanged
    {
        System.Timers.Timer Timer1;

        public void CreateTimer1(int ms)
        {
            if (Timer1 == null)
            {
                Timer1 = new System.Timers.Timer();
                //Timer1.AutoReset = false; //
                Timer1.Interval = ms;
                Timer1.Elapsed += Timer1Tick;
                Timer1.Enabled = true;
                Timer1.Start();
            }

            

        }

        private void Timer1Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                RaisePropertyChanged("memory");

            }
        }

         public Task<string> AsyncTaskGet(string url)
        {
         
            return Task.Run(() =>
            {
                //----------------

                return GETnoas(url);

                //----------------
            });
        }

        public static string result77="";

         async void GETasyn(string url)
        {

            data.start_ping = true;
            string ss = await AsyncTaskGet(url);

        }

        public string GET(string u)
        {
            result77 = "";

            if (task_ping !=null)
            if (task_ping.Status == TaskStatus.Running) return "task is running";
            // GETasyn(u);//асинхр

            test(u);
            //result77=GETnoas(u);//синхр
             
            return result77;
        }


        public Task task_ping;

        async void test(string url)
        {

            task_ping = Task.Run(async delegate
            {
                GETnoas(url);
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

        public string GETnoas(string url)
        {
            url = url.Trim();

            try
            {
                WebClient client = new WebClient();

                // MessageBox.Show("IP="+ ">>" + result.ToString() + "<<", "non",
                //   MessageBoxButton.OK);

                string rez = "";
                Stream stream = client.OpenRead(url);
                if (stream == null) { result77 = "НЕ СУЩЕСТВУЕТ."; data.start_ping = false; return "erroMNN"; }
                StreamReader sr = new StreamReader(stream);
                string newLine;
                while ((newLine = sr.ReadLine()) != null)
                {
                    string[] words;
                    words = newLine.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);

                    foreach (var s in words)
                        rez += s.ToString() + '\n';

                }
                if (stream!=null) stream.Close();

                if (stream == null) { result77 = "НЕ СУЩЕСТВУЕТ."; }
                result77 = rez;
                data.start_ping = false;
                return rez;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("НЕ СУЩЕСТВУЕТ "+ ex.Message.ToString(), "",    
                //                    MessageBoxButton.OK);
                result77 = "НЕ СУЩЕСТВУЕТ " + ex.Message;
                data.start_ping = false;
                return ("НЕ СУЩЕСТВУЕТ " + ex.Message);
            }
        }



        //basic ViewModelBase
        internal void RaisePropertyChanged(string prop)
        {
           if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
               
            }
         

        }
       public  event PropertyChangedEventHandler PropertyChanged; //событие выбора канала




       



        //Extra Stuff, shows why a base ViewModel is useful
        bool? _CloseWindowFlag;
        public bool? CloseWindowFlag
        {
            get { return _CloseWindowFlag; }
            set
            {
                _CloseWindowFlag = value;
                RaisePropertyChanged("CloseWindowFlag");
            }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null 
                    ? true 
                    : !CloseWindowFlag;
            }));
        }
    }
}
