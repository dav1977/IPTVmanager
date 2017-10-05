using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IPTVman.ViewModel
{
        static class data
        {
            public static WCFSERVER _server;
            public static string url = "";
            public static string name = "";
            public static bool mode_radio = false;
            public static bool mode_scan = false;
            public static string title = "";
            public static string buff = "";
            public static string scanURL = "init";

            public static bool exit_programm = false;
        }

}
