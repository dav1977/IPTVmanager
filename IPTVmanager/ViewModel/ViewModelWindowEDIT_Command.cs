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

            if (ViewModelBase._ping.result77 != "")
            {
                if (!newPING)
                {
                    analizPING(ViewModelBase._ping.result77);
                    newPING = true;
                }
            }
            else newPING = false;
        }

        //============================== INIT ==================================
        public ViewModelWindow1(string lastText)
        {
            CreateTimer2(500);
            lok.keyadd = false;
            ViewModelBase._ping.result77 = "";
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
            lok.edit = false;
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
                IPTVman.Model.play.playerUPDATE = false;
            }

            if (IPTVman.Model.play.playerUPDATE == false)
            {

                REG_FIND reg = new REG_FIND();
                string rez = reg.FIND("ace_player.exe");


                string[] words = rez.Split(new char[] { '"' });
                if (words.Length < 2) rez = "";

                if (rez == "")
                {
                    MessageBox.Show("Не найден ACE_PLAYER в реестре", "", MessageBoxButton.OK);
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
                else MessageBox.Show("Не найден файл ACE_PLAYER.exe по пути " + play.path, "", MessageBoxButton.OK);

            }
            IPTVman.Model.play.playerUPDATE = false;
        }

        void PING(object selectedItem)
        {
     
            if (edit.http == null) return;
            edit.ping = "";
            strPING = _pingPREPARE.GET(edit.http);
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
                edit.ping += s;
            } 
        }


 
    }
}
