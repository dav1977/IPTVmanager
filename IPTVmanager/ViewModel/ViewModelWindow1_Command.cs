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


        public RelayCommand key_PLAY { get; set; }
        public RelayCommand key_PING { get; set; }
        public RelayCommand key_SAVE { get; set; }
        public RelayCommand key_ADDBEST { get; set; }


        System.Timers.Timer Timer2;

        public void CreateTimer2(int ms)
        {
            if (Timer2 == null)
            {
                Timer2 = new System.Timers.Timer();
                //Timer1.AutoReset = false; //
                Timer2.Interval = ms;
                Timer2.Elapsed += Timer2Tick;
                Timer2.Enabled = true;
                Timer2.Start();
            }



        }

        static bool newPING = false;
        private void Timer2Tick(object sender, EventArgs e)
        {

            if (ViewModelBase.result77 != "")
            {
                if (!newPING)
                {
                    analizPING(ViewModelBase.result77);
                    newPING = true;
                }
            }
            else newPING = false;

            //if (task_ping != null)
            //{
            //    if (task_ping.Status != System.Threading.Tasks.TaskStatus.Running)
            //    {
            //        if (newPING == true) ViewModelBase.result77 = "task  stopped";
            //    }

            //}
        }

        //============================== INIT ==================================
        public ViewModelWindow1(string lastText)
        {
            CreateTimer2(500);
            data.one_add = false;

            //p = new ParamCanal
            //{
            //    name = data.name,
            //    ExtFilter =data.extfilter,
            //    group_title =data.grouptitle,
            //    http =data.http,
            //    logo =data.logo,
            //    tvg_name = data.tvg,
            //    ping = data.ping
            //};
            p = data.edit;

            key_PLAY = new RelayCommand(PLAY);
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
            //data.name = p.name;
            //data.extfilter = p.ExtFilter;
            //data.grouptitle = p.group_title;
            //data.http = p.http;
            //data.ping = p.ping;
            data.edit = p;

            if (Event_UpdateEDIT != null) Event_UpdateEDIT();
            if (Event_CloseWin1 != null) Event_CloseWin1();
        }




        void PLAY(object selectedItem)
        {

            if (p.http == null) return;

            Regex regex1 = new Regex("http:");
            Regex regex2 = new Regex("https:");

            var match1 = regex1.Match(p.http);
            var match2 = regex2.Match(p.http);

            if (match1.Success || match2.Success)
            {
                p.ping = "";
                strPING = GET(p.http);
            }
        }

        void PING(object selectedItem)
        {
     
            if (p.http == null) return;
            p.ping = "";
            strPING = GET(p.http);
        }


        void analizPING(string strPING)
        { 
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
