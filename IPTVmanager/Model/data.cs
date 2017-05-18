using System;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;


namespace IPTVman.Model
{

    


    public static class data
    {

        public static Process playerV;
        public static byte current_favorites =1;
        public static string favorite1_1 = "best";
        public static string favorite1_2 = "best";

        public static string favorite2_1 = "music";
        public static string favorite2_2 = "music";

        public static string favorite3_1 = "kino";
        public static string favorite3_2 = "kino";

        public static bool lokUP,lokDN = false;

        public static ParamCanal canal = new ParamCanal();
        public static ParamCanal edit;

        public static string best1,best2;
  
        public static string f1,f2,f3,f4;

        public static bool one_add = false;//дребезг кнопки

        public static bool start_ping = false;

        public static bool playerUPDATE = false;
        public static string URLPLAY = "";
    }

    
}
