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
using IPTVman.Model;

namespace IPTVman.ViewModel
{
    class ScannerRadio
    {
        List<string> lst= new List<string>();

        public static event Action event_done;
        public List<string> result = new List<string>();

        // MemoryFile  m = new MemoryFile();
        WCFCLIENT client = new WCFCLIENT("http://localhost:8000/IPTVmanagerSevice");

        public void clear()
        {
            lst.Clear();
        }

        public void add(string s)
        {
            lst.Add(s);
        }

        public int get_size()
        {
            return lst.Count;
        }

        public void CLOSE_SCANNER()
        {
            //Thread.Sleep(200);   
        }

        Task tasksc;
        public static CancellationTokenSource cts1 = new CancellationTokenSource();
        public static CancellationToken cancellationToken;

        public async void getPLAYING()
        {          
            cancellationToken = cts1.Token;//для task1
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            tasksc = Task.Run(() =>
            {
                var tcs = new TaskCompletionSource<string>();
                try
                {
                    result = client.GetPlaying(lst);
                    if (event_done != null) event_done();
                    tcs.SetResult("ok");
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
            catch { }// (Exception ex)
            {
                //System.Windows.MessageBox.Show("ОШИБКА  ScannerRadio  getPLAYING " + ex.Message.ToString());
            }
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
