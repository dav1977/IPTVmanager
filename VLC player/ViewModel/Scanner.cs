using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
    }


    public static class Result
    {
        //public static event Action<string> print;

        public static bool data_ok = false;

        public static List<string> listresult = new List<string>();

        public static void Clear()
        {
            data_ok = false;
            listresult.Clear();
        }

        public static AudioBass bass;
        /// <summary>
        /// этот метод вызвыает  WCF служба 
        /// </summary>
        /// <param name="data"></param>
        public static void RUN_SCAN(List<string> datalist)
        {
            try
            {
                Result.Clear();
                int i = 1;
                foreach (var s in datalist)
                {
                    bass.create_stream(s, false, null);
                    Thread.Sleep(500);

                    data.scanURL = "[" + i.ToString() + " из " + datalist.Count + "]" + s;
            

                    string bitr = "";
                    string play = bass.get_tags(s, ref bitr);
                    i++;

                    Result.listresult.Add(s);
                    Result.listresult.Add(play);
                    Result.listresult.Add(bitr.ToString());
                    //if (print != null) print(s+" "+play);
                }

                Result.data_ok = true;
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("err scan " + ex.Message); }
        }

    }
}
