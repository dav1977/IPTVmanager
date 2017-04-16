using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPTVman.Helpers;
using IPTVman.Model;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Threading;



namespace IPTVman.ViewModel
{

    partial class ViewModelWindow1 : ViewModelMain
    {

        public static event Delegate_Window1 Event_CloseWin1;
        public static event Delegate_UpdateEDIT Event_UpdateEDIT;
        public static event Delegate_ADDBEST Event_ADDBEST;


        public RelayCommand key_PING { get; set; }
        public RelayCommand key_SAVE { get; set; }
        public RelayCommand key_ADDBEST { get; set; }



        //============================== INIT ==================================
        public ViewModelWindow1(string lastText)
        {

            data.one_add = false;

            p = new ParamCanal
            {
                name = data.name,
                ExtFilter =data.extfilter,
                group_title =data.grouptitle,
                http =data.http,
                logo =data.logo,
                tvg_name = data.tvg,
                ping = data.ping
            };

            key_PING = new RelayCommand(PING);
            key_SAVE = new RelayCommand(SAVE);
            key_ADDBEST = new RelayCommand(BEST);
        }

        //=============================================================================








        void BEST(object selectedItem)
        {
            
            if (Event_ADDBEST != null) Event_ADDBEST();
            if (Event_CloseWin1 != null) Event_CloseWin1();

            Thread.Sleep(300);
            //MessageBox.Show("УСПЕШНО ДОБАВЛЕНО В ГРУППУ BEST",""
            //  MessageBoxButton.OK);
        }








        void SAVE(object selectedItem)
        {
            data.name = p.name;
            data.extfilter = p.ExtFilter;
            data.grouptitle = p.group_title;
            data.http = p.http;
            data.ping = p.ping;

            if (Event_UpdateEDIT != null) Event_UpdateEDIT(data.edit_index);
            if (Event_CloseWin1 != null) Event_CloseWin1();
        }



        public string GET()
        {
            String url = p.http.Trim();
       
            try
            {
                WebClient client = new WebClient();

              // MessageBox.Show("IP="+ ">>" + result.ToString() + "<<", "non",
              //   MessageBoxButton.OK);

                string rez="";
                Stream stream = client.OpenRead(url);
                StreamReader sr = new StreamReader(stream);
                string newLine;
                while ((newLine = sr.ReadLine()) != null)
                {
                    string[] words;
                    words = newLine.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);

                    foreach (var s in words)
                    rez += s.ToString() + '\n';

                }
                stream.Close();
                return rez;
            }
            catch (Exception ex) {
                //MessageBox.Show("НЕ СУЩЕСТВУЕТ "+ ex.Message.ToString(), "",    
                //                    MessageBoxButton.OK);
                return ("НЕ СУЩЕСТВУЕТ "+ex.Message);
            }
        }


        void PING(object selectedItem)
        {
            if (p.http == null) return;
            strPING = GET();

            string n0 = strPING.ToString();
            string n1 = n0.Replace("#EXTM3U", "");
            string n2 = n1.Replace("#EXT-X-VERSION:3", "");
            n2 = n2.Replace("#EXT-X-STREAM-", "");


            //  n2 = n2.Trim(new char[] { '\n', '\r' });


            string[] n3 = n2.Split(new char[] { '\n' });

            foreach (string s in n3)
            {
                p.ping += s;
            }


           
        }


   


    }
}
