using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows;
using System.Threading;
using System.Windows.Threading;


namespace IPTVman.ViewModel
{
    class ScannerRadio
    {
        List<string> lst= new List<string>();

        public static event Action<List<string>> event_done;
        public List<string> result = new List<string>();

        // MemoryFile  m = new MemoryFile();
        WCFCLIENT client = new WCFCLIENT("http://localhost:8000/IPTVmanagerSevice");

        public void add_to_save(string s)
        {
            lst.Add(s);
        }

        Task tasksc;
        public static CancellationTokenSource cts1 = new CancellationTokenSource();
        public static CancellationToken cancellationToken;

        public async void getPLAYING()
        {
            List<string> rez= new List<string>();
            
            cancellationToken = cts1.Token;//для task1
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            tasksc = Task.Run(() =>
            {
                var tcs = new TaskCompletionSource<string>();
                try
                {
                    result = client.GetPlaying(lst);
                    tcs.SetResult("ok");
                    if (event_done != null) event_done(result);
                }
                catch (OperationCanceledException ex)
                {
                    tcs.SetException(ex);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                return tcs.Task;
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try { await tasksc; }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("ОШИБКА  ScannerRadio  " + ex.Message.ToString());
            }
            if (cts1 != null) cts1.Cancel();

            return;
        }
        //public void save()
        //{
        //    //m.WriteObjectToMemory("iptvlinks", lst);
        //    //Thread.Sleep(300);
        //}

        //public void read()
        //{
        //    try
        //    {
        //        List<string> data = m.ReadObjectFromMemory("iptvplay") as List<string>;

        //        if (data == null) return;
        //        string rez = "";
        //        foreach (var s in data)
        //        {
        //            rez += s+"; ";
        //        }

        //        MessageBox.Show(rez); 
        //    }
        //    catch (Exception ex) { System.Windows.MessageBox.Show("memorymaps error=" + ex.Message); }
        //}


    }
}
