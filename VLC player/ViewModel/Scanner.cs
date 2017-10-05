using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPTVman.ViewModel
{

    partial class Player
    {
        /// <summary>
        /// initWCF
        /// </summary>
        /// <returns></returns>
        bool initWCF()
        {
            try
            {
                data._server = new WCFSERVER("http://localhost:8000/IPTVmanagerSevice");
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// scan
        /// </summary>
        /// <param name="cts"></param>
        void scan(CancellationToken cts)
        {
            //List<string> datals = new List<string>();
            //List<string> data = m.ReadObjectFromMemory("iptvlinks") as List<string>;

            //if (data == null) return;
            //List<string> savedata = new List<string>();

            data.scanURL = "ready to scan...";
            int ct = 0;
            while (true)
            {
                if (cts.IsCancellationRequested) return;
                ct++;
                if (ct > 10 * 30) break; //даем 30 сек на всё
                if (Result.data_ok) break;

                Thread.Sleep(100);  //ждем приема списка и выполнение сканирования
            }
        }

    }


    public static class Result
    {
        public static bool data_ok = false;

        public static List<string> listresult = new List<string>();

        public static void Clear()
        {
            data_ok = false;
            listresult.Clear();
        }


        /// <summary>
        /// этот метод вызвыает  WCF служба 
        /// </summary>
        /// <param name="data"></param>
        public static void RUN_SCAN(List<string> datalist)
        {
            try
            {
                var bass = new AudioBass();
                bass.init();

                Result.Clear();
                int i = 1;
                foreach (var s in datalist)
                {
                    bass.init();
                    Thread.Sleep(50);
                    bass.create_stream(s, false, null);
                    Thread.Sleep(50);

                    data.scanURL = "[" + i.ToString() + " из " + datalist.Count + "]" + s;

                    string bitr = "";
                    string play = bass.get_tags(s, ref bitr);
                    i++;

                    Result.listresult.Add(s);
                    Result.listresult.Add(play);
                    Result.listresult.Add(bitr.ToString());
                }

                Result.data_ok = true;
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("err scan " + ex.Message); }
        }

    }
}
