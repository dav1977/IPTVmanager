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
using System.Diagnostics;


namespace IPTVman.ViewModel
{
    partial class ViewModelWindow1 : ViewModelMain
    {
        public static Vlc.DotNet.Player player = null;

        public static event Delegate_UpdateEDIT Event_UpdateEDIT;
        public static event Delegate_ADDBEST Event_ADDBEST;
       
        public RelayCommand key_PLAY { get; set; }
        public RelayCommand key_PING { get; set; }
        public RelayCommand key_SAVE { get; set; }
        public RelayCommand key_ADDBEST { get; set; }

        public static PING _ping;
        public static PING_prepare _pingPREPARE;

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

  
        private void Timer2Tick(object sender, EventArgs e)
        {
            if (_ping == null) return;
            try
            {
                if (_ping.done)
                {
                    Thread.Sleep(1000);
                    if (_ping == null) return;
                    strPING = _ping.result;
                    convPING(_ping.result);
                    _ping.done = false;

                }
            }
            catch { }
           
        }

        //============================== INIT ==================================
        public ViewModelWindow1(string lastText)
        {
            _ping = null;
            _pingPREPARE = null;
            strPING = "";
            CreateTimer2(500);
            loc.keyadd = false;
            edit =(ParamCanal) data.canal.Clone();
            key_PLAY = new RelayCommand(PLAY);
            key_PING = new RelayCommand(PING);
            key_SAVE = new RelayCommand(SAVE);
            key_ADDBEST = new RelayCommand(BEST);
        }

        //=============================================================================

        void BEST(object selectedItem)
        {   
            if (Event_ADDBEST != null) Event_ADDBEST();
            Thread.Sleep(300);
        }


        void SAVE(object selectedItem)
        {
            if (loc.collection) return;
            GUI.edit = false;
            if (Event_UpdateEDIT != null) Event_UpdateEDIT(edit); 
        }

        void PLAY(object selectedItem)
        {
            if (play.URLPLAY == "") return;

            //сначала пробуем играть через библиотеку
            try
            {
                if (player == null)
                {
                    player = new Vlc.DotNet.Player { DataContext = new ViewModelWindow1("") };
                    player.Show();
                }
                else
                {
                    if (!player.window_enable)
                    {
                        player.window_enable = true;
                        player = new Vlc.DotNet.Player { DataContext = new ViewModelWindow1("") };
                        player.Show();
                    }

                }
            }
            catch
            {
                play.playerUPDATE = false;
            }

            if (play.playerUPDATE == false)
            {

                REG_FIND reg = new REG_FIND();
                string rez = reg.FIND("ace_player.exe");


                string[] words = rez.Split(new char[] { '"' });
                if (words.Length < 2) rez = "";

                if (rez == "")
                {
                    dialog.Show("Не найден ACE_PLAYER в реестре");
                    return;
                }
                else
                {
                    play.path = words[1];
                    play.path = play.path.Replace(@"\", @"/");
                }


                try
                {
                    if (play.playerV != null)
                    {
                        play.playerV.CloseMainWindow();
                        play.playerV.Close();
                        play.playerV.WaitForExit(10000);
                    }
                }
                catch { }


                if (File.Exists(play.path))
                {
                    //   Process.Start(player_path);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = false;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = play.path;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.Arguments = play.URLPLAY;

                    play.playerV = Process.Start(startInfo);
                    // Process.Start(startInfo);
                   

                }
                else dialog.Show("Не найден файл ACE_PLAYER.exe по пути\n" + play.path);

            }
            IPTVman.Model.play.playerUPDATE = false;
        }

        void PING(object selectedItem)
        {
            _ping = new ViewModel.PING();
            _pingPREPARE = new PING_prepare(_ping);
            edit.ping = "";
            if (edit.http == null) return;
            strPING = _pingPREPARE.GET(edit.http);
        }


       public  void convPING(string strPING)
        {
            edit.ping = "";
            if (strPING.Length > 5000) { edit.ping = "большой размер данных "+ strPING.Length.ToString();return; }

            string[] n3 = strPING.Split(new char[] { '\n' });
            foreach (string s in n3)
            {
                edit.ping += s;
            } 
        }


 
    }
}
