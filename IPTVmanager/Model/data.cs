using System;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Windows;

namespace IPTVman.Model
{
    public enum typefilter { last, normal, best, dublicate };

    /// <summary>
    /// данные локирования
    /// </summary>
    public static class loc
    {
        public static bool autoping = false;
        public static bool collection = false;
        public static bool openfile = false;
        public static bool finddublic = false;
        public static bool lokUP, lokDN = false;
        public static bool keyadd = false;//дребезг кнопки
        public static bool updateMDB = false;
        public static bool edit = false;
        public static bool block_dialog_window = false;
        public static bool start_one = false;
        public static bool timer_tmr = false;
        public static bool asyncOPEN = false;
        public static bool MODE_RELEASE_SCRIPT = false;
    }
    
    public static class bd_data
    {
        public static string path = "";
        public static string mask = "acestream: ,  udp: ,    http://127.0.0.1:6689 , rtmp: , mmsh: , ";
        public static string filter1 = "best, music, kino, ";
        public static string filter2 = "";
    }
    public static class play
    {
        public static Process playerNVLC;
        public static string path = "";
        public static Process playerV;
        public static string URLPLAY = "";
        public static string name = "";
    }

    public static class data
    {
        public static string temppath = ""; 
        public static string NAME_SCANER_SERVER = "СКАННЕР РАДИО ТРЭКОВ";
        public static bool mode_radio_from_select = false;
        public static string[] arguments_startup;
        public static bool arguments_start;
        public static byte type_player = 0;
        public static string NOT_URL = "НЕ СУЩЕСТВУЕТ";
        public static byte current_favorites =1;
        public static string favorite1_1 = "best";
        public static string favorite1_2 = "";

        public static string favorite2_1 = "music";
        public static string favorite2_2 = "";

        public static string favorite3_1 = "kino";
        public static string favorite3_2 = "";

        public static ParamCanal canal = new ParamCanal();
        public static ParamCanal edit = new ParamCanal();

        public static string best1,best2;
  
        public static string f1,f2,f3,f4;

        public static int ct_ping = 0;
        public static int ping_all = 0;
        public static int ping_waiting = 0;
        public static bool flag_adding_ok = false;
        public static string Title = "";

        public static  void set_best()
        {
            if (data.current_favorites == 1)
            {
                    best1 = favorite1_1;
                    best2 = favorite1_2;
            }
            if (data.current_favorites == 2)
            {
                    best1 = favorite2_1;
                    best2 = favorite2_2;
                }
            if (data.current_favorites == 3)
            {
                    best1 = favorite3_1;
                    best2 = favorite3_2;
                }
        }

        public static class Utils
        {

            public static void Find_and_Close_Window(string name)
            {
                foreach (Window win in Application.Current.Windows)
                {
                    if (win.Name == name)
                    {
                        win.Close();
                    }
                }
            }


        }
    }


}
