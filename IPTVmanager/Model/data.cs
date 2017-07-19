﻿using System;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;


namespace IPTVman.Model
{
    /// <summary>
    /// данные локирования
    /// </summary>
    public static class loc
    {
        public static bool collection = false;
        public static bool openfile = false;
        public static bool lokUP, lokDN = false;
        public static bool keyadd = false;//дребезг кнопки
    }
    
    /// <summary>
    /// данные для формы
    /// </summary>
    public static class GUI
    {
      public static  double progressbar=0;
      public static double progressbar_max = 0;
      public static bool dynamic_progressbar = false;
      public static bool edit = false;
      public static string longtaskSTRING;

        public static void set_ProgressBar(double max, bool en )
        {
            progressbar_max = max;
            if (en) dynamic_progressbar = false; else dynamic_progressbar = true;
        }
    }

    public static class bd_data
    {
        public static string s1 = "ace";
        public static string s2 = "best";
        public static string s3 = "best";
    }
    public static class play
    {
        public static string path = "";
        public static Process playerV;
        public static bool playerUPDATE = false;
        public static string URLPLAY = "";
    }

    public static class data
    {
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

        public static bool filtr_best = false;

        public static int ct_ping = 0;
        public static int ping_all = 0;
        public static int ping_waiting = 0;

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


    }

   
}
