using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IPTVman.Model;

namespace IPTVman.ViewModel
{
    static class scripts
    {
        public static event Action Event_Update_GUI;
        public static string FIND_SCRIPT(string line)
        {
            int ct = 0;
         

            //наличие скрипта
            if (new Regex("%").Match(line).Success)
            {
                Debug.WriteLine("Найдена команда "+line);
                string[] split = line.Split(new Char[] { '%' });

                foreach (var str in split)
                {
                    ct++;
                    //Debug.WriteLine("--- "+str);
                    if (str == null || str == "") continue;
                    if (new Regex("http://").Match(str).Success) line = str.Trim();
                    if (new Regex("https://").Match(str).Success) line = str.Trim();

                    if (new Regex("ENABLEHOOKS").Match(str).Success)
                    {
                        Script.skip_obrez_skobki = true;
                    }
                    if (new Regex("SKIPMESSAGE").Match(str).Success)
                    {
                        Script.en_skip_message_skiplinks = true;
                    }
                    if (new Regex("OPENWINUPDATE").Match(str).Success)
                    {
                        Script.OpenWindow_db_update = true;
                        loc.MODE_RELEASE_SCRIPT = true;
                    }
                    if (new Regex("UPDATELISTENABLE").Match(str).Success)
                    {
                        Script.enable_update = true;
                        ViewModelMain.chek_upd = true;
                        if (Event_Update_GUI != null) Event_Update_GUI();
                    }
                    if (new Regex("UPDATELISTDISABLE").Match(str).Success)
                    {
                        Script.enable_update = false;
                        ViewModelMain.chek_upd = false;
                        if (Event_Update_GUI != null) Event_Update_GUI();

                    }
                    if (new Regex("OPENWINRADIO").Match(str).Success)
                    {
                        Script.OpenWindow_radio = true;
                        Script.working = true;
                        loc.MODE_RELEASE_SCRIPT = true;
                    }

                    if (new Regex("CLOSEIPTVMANAGER").Match(str).Success)
                    {
                        Script.CLOSE_ALL = true;
                    }

                    if (new Regex("ADDFILE").Match(str).Success)
                    {
                        Debug.WriteLine("find script addfile "+line);
                        if (split[ct] != null)
                        {
                            addFILE(split[ct]);
                        }
                    }
                }


            }

            return line;
        }


        public static void addFILE(string s)
        {
            Script.add = true;
            Script.addpath = s;
            Script.working = true;
            loc.MODE_RELEASE_SCRIPT = true;
        }

    }

   
}
