using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPTVman.ViewModel
{
    static class scripts
    {
        public static string FIND_SCRIPT(string line)
        {
            int ct = 0;
            //наличие скрипта
            if (new Regex("%").Match(line).Success)
            {
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
                        Debug.WriteLine("---hooks");
                        IPTVman.Model.ModeWork.skip_obrez_skobki = true;
                    }
                    if (new Regex("SKIPMESSAGE").Match(str).Success)
                    {
                        IPTVman.Model.ModeWork.skip_message_skiplinks = true;
                    }
                    if (new Regex("OPENWINUPDATE").Match(str).Success)
                    {
                        IPTVman.Model.ModeWork.OpenWindow_db_update = true;
                    }
                    if (new Regex("OPENWINRADIO").Match(str).Success)
                    {
                        IPTVman.Model.ModeWork.OpenWindow_radio = true;
                    }

                    if (new Regex("CLOSEIPTVMANAGER").Match(str).Success)
                    {
                        IPTVman.Model.ModeWork.CLOSE_ALL = true;
                    }

                    if (new Regex("ADDFILE").Match(str).Success)
                    {
                        Debug.WriteLine("find script addfile "+line);
                        if (split[ct] != null)
                        {
                            IPTVman.Model.ModeWork.add = true;
                            IPTVman.Model.ModeWork.addpath = split[ct];
                            IPTVman.Model.ModeWork.process_adding = true;
                        }
                    }
                }


            }

            return line;
        }




    }
}
