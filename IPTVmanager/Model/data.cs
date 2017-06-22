using System;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;


namespace IPTVman.Model
{

    public static class lok
    {
        public static bool edit = false;
        public static bool open = false;
        public static bool lokUP, lokDN = false;
        public static bool keyadd = false;//дребезг кнопки
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
        public static string favorite1_2 = "best";

        public static string favorite2_1 = "Music";
        public static string favorite2_2 = "best";

        public static string favorite3_1 = "Kino";
        public static string favorite3_2 = "best";

       

        public static ParamCanal canal = new ParamCanal();
        public static ParamCanal edit = new ParamCanal();

        public static string best1,best2;
  
        public static string f1,f2,f3,f4;

        public static bool filtr_best = false;

        public static bool start_ping = false;



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
