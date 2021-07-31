using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.JoshSmith.ServiceProviders.UI;
using System.Windows.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using IPTVman.Model;
using System.Timers;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Net;
using System.Reflection;
using static System.Net.WebRequestMethods;


namespace IPTVman.ViewModel
{
    class Parse
    {
        public struct mode
        {
            public const bool with_command=true;
            public const bool not_command= false;
        }

        public List<string> LISTURL;

        public Parse(bool curr, string ex)
        {
            name = ex;
            Debug.WriteLine("CREATE CLASS " + name);
            current_mode =  curr;//работа с режимом поиска скриптов или без
        }


        string name = "no";
        bool current_mode = mode.with_command;
        uint ct_dublicat = 0;
        uint ct_update = 0;
        uint ct_ignore_update = 0;
        List<int> list_update_channels = new List<int>();

        int ctIFLE = 0;



        bool analiz_str(List<ParamCanal> lst, string str, bool OFFscript, int num  )
        {
            if (OFFscript)//АНАЛИЗ СТРОК БЕЗ СКРИПТОВ
            {
                Debug.WriteLine("БЕЗ СКРИП СТРОКА " + num + " - " + str);

                if (num < 3)
                    if (AnalizTYPElink(str))
                    {
                    }
            }
            else //С УЧЕТОМ СКРИПТОВ
            {
                Debug.WriteLine("СТРОКА " + num + " - " + str);

                if (num < 3)
                    if (AnalizTYPElink(str))
                    {
                        Debug.WriteLine("@@ FIND LINK ");

                        if (LISTURL == null) LISTURL = new List<string>();
                        //в файле находится ссылка на m3u
                        LISTURL.Add(str);

                        var web = new FileWork();
                        if (web.DownloadLINK(str) == "")
                        { Debug.WriteLine("error URL  "); };

                        Debug.WriteLine("DOWNL OK  " + data.temppath);

                        web.add_file_to_memory(data.temppath);

                        //
                        web.parse_temp_file(lst);

                        Debug.WriteLine("END parse_temp_file");

                       return true;
                    }
            }

            return false;
        }

        bool  find_max_str(string file)
        {
            int all_str = 0;
            byte null_str = 0;
            bool ns = false;

            try
            {

                //определение макс строк СКАНЕР ФАЙЛА
                Debug.WriteLine(name + "      ===ФАЙЛСКАН=== "+ ctIFLE.ToString() + "  " + file);
               
                Regex regex_link = new Regex("http://");
                Regex regex_link2 = new Regex("https://");
                Regex regex1 = new Regex("#EXTINF");
                Regex regex2 = new Regex("#EXTM3U");
                Match match = null;

                using (StreamReader sr = new StreamReader(file))
                {
                    while (!sr.EndOfStream)
                    {
                        string rez = sr.ReadLine();
                        if (sr.EndOfStream) { break; }
                        //string rez = ReadFind_script(str, true);
                        if (rez != "" && rez != null)
                            if (new Regex("#EXTINF").Match(rez).Success) { all_str++; null_str = 0; }
                            else null_str++; if (null_str > 100 || all_str > 500000) { ns = true; break; }
                        match = regex2.Match(rez);
                        if (match.Success) text_title = rez;
                    }
                }

                Debug.WriteLine(name + "        ===CLOSE СКАНФАЙЛ===");

            }
            catch (Exception ex) { MessageBox.Show("ошибка сканирования " + ex.Message);ns = true; }

           

            if (ns) return true;
            Debug.WriteLine("size=" + all_str + " null_str=" + null_str);
            Wait.set_ProgressBar(all_str);

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="file"></param>
        /// <param name="OFFscript"></param>
        public void PARSING_FILE(List<ParamCanal> lst, string file, bool OFFscript)
        {
          
            ctIFLE++;
            Debug.WriteLine("-START parsing "+name);
            if (lst== null) Debug.WriteLine("-START parsing CRASH lst  " + name);

            string line = null;
            bool skip_mes = false;

            try
            {
                Regex regex_link = new Regex("http://");
                Regex regex_link2 = new Regex("https://");
                Regex regex1 = new Regex("#EXTINF");
                Regex regex2 = new Regex("#EXTM3U");
                Match match = null;

                Wait.set_ProgressBar(100);

               if ( find_max_str(file) ) goto exit_open;

                
                //=========================================================
                //ПОИСК каналов
               

                Debug.WriteLine(name + "        ===ФАЙЛ===" );
                using (StreamReader sr = new StreamReader(file))
                {
                  
                    int num = 0;

                    while (!sr.EndOfStream)
                    {
                        string str = sr.ReadLine();
                        num++;

                        if (analiz_str(lst, str, OFFscript, num)) continue;
                       
                        if (sr.EndOfStream) { break; }
                        if (str == "") { continue; }


                        if (!OFFscript)
                        {
                            line = ReadFind_script(str);    
                        }
                        else line = str;

                        if (Script.CLOSE_ALL) { Debug.WriteLine("--EXIT-"); return; }

                        match = regex1.Match(line);  
                        
                        razbor_read_next(match.Success, line, match, sr, lst);


                        if (Script.CLOSE_ALL) { Debug.WriteLine("--EXIT-"); return; }

                    }///чтение файла


                }

                Debug.WriteLine(name + "        ===CLOSE ФАЙЛ===");
               
            }
            catch { }
            Debug.WriteLine("end parsing");

        exit_open:


            if (Event_UpdateLIST != null) Event_UpdateLIST(typefilter.normal);

            if (!OFFscript)
            {

                string addstr = "";

     

                if (!skip_mes)
                {
                    //обновление в списке
                    if (ct_update != 0) dialog.Show("ОБНОВЛЕНО " + ct_update.ToString() + " каналов" + addstr);
                    if (ct_dublicat != 0) dialog.Show("Пропущенно дублированных ссылок " + ct_dublicat.ToString());
                    if (ct_ignore_update != 0) addstr = "\nПропущено дублирование " + ct_ignore_update.ToString();
                }

                if (!data.flag_adding_ok && !Script.flag_add) dialog.Show("Каналы не обнаружены");

            }
            loc.openfile = false;
            loc.MODE_RELEASE_SCRIPT = false;


        }


        void razbor_read_next(bool exinf, string line, Match match, StreamReader sr, List<ParamCanal> lst)
        {

            string newname = "";
            string str_ex = "", str_name = "", str_http = "", str_gt = "", str_logo = "", str_tvg = "";
            ///========== разбор EXINF
            if (exinf)
            {
                Regex regex3 = new Regex("ExtFilter=", RegexOptions.IgnoreCase);
                Regex regex4 = new Regex("group-title=", RegexOptions.IgnoreCase);
                Regex regex5 = new Regex("logo=", RegexOptions.IgnoreCase);
                Regex regex6 = new Regex("tvg-name=", RegexOptions.IgnoreCase);
                Regex regex7 = new Regex("#EXTGRP", RegexOptions.IgnoreCase);

                string[] split = line.Split(new Char[] { '"' });


                str_ex = ""; str_name = ""; str_http = ""; str_gt = ""; str_logo = ""; str_tvg = "";

                if (split.Length <= 1)
                {
                    split = line.Split(new Char[] { ',' });
                    str_name = split[split.Length - 1];
                    newname = str_name;
                }
                else
                {
                    int i = 0;
                    for (i = 0; i < split.Length; i++)
                    {
                        string s = split[i];
                        //------------- разбор строки EXINF
                        if (str_ex == "!") str_ex = s;
                        if (str_gt == "!") str_gt = s;
                        if (str_logo == "!") str_logo = s;
                        if (str_tvg == "!") str_tvg = s;


                        if (i >= split.Length - 1) { str_name = s; }

                        match = regex3.Match(s);
                        if (match.Success)
                        {
                            str_ex = "!";
                        }

                        match = regex4.Match(s);
                        if (match.Success)
                        {
                            str_gt = "!";
                        }

                        match = regex5.Match(s);
                        if (match.Success)
                        {
                            str_logo = "!";
                        }

                        match = regex6.Match(s);
                        if (match.Success)
                        {
                            str_tvg = "!";
                        }
                    }//foreach

                    newname = "";
                    if (str_name != "") newname = str_name.Remove(0, 1);

                }



                //--------------------------------------------
                //чтение ссылки
                try
                {
                    str_http = sr.ReadLine();
                    Debug.WriteLine("//чтение ссылки RAZBOR "+ str_http);
                    ReadFind_script(str_http);

                }
                catch { Debug.WriteLine("//bad чтение ссылки"); }

                if (linkIsBad(str_http)) 
                {
                    match = regex7.Match(str_http);
                    if (match.Success)
                    {
                        str_http = str_http.Replace("#EXTGRP", "");
                        str_http = str_http.Replace(":", "");
                        str_gt = str_http;
                    }

                    str_http = sr.ReadLine();
                    Debug.WriteLine("//чтение ссылки ПОВТОР  RAZBOR " + str_http);

                } else
                { 
                    //URL OK
                    Debug.WriteLine("lk-"+str_http);  
                
                }

                if (linkIsBad(str_http))
                {
                    //URL BAD
                    Debug.WriteLine("BAD URL");
                    return;

                }
                
            }




            if (chek_hoop)//обрезание скобок
            {
                if (!Script.skip_obrez_skobki)
                {
                    //string[] words = newname.Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries);
                    var ind = newname.LastIndexOf('(');
                    if (ind > 0) newname = newname.Substring(0, ind);
                    newname = newname.Trim();
                }
            }


            if (!chek_update && !Script.enable_update)//Добавление
            {
                var item = lst.Find(x =>
                (x.http == str_http && x.ExtFilter == str_ex && x.group_title == str_gt));

                if (newname.Trim() != "" && str_http.Trim() != "")
                {

                    if (item == null)
                    {
                        lst.Add(new ParamCanal
                        {
                            name = newname.Trim(),
                            ExtFilter = str_ex.Trim(),
                            http = str_http.Trim(),
                            group_title = str_gt.Trim(),
                            logo = str_logo.Trim(),
                            tvg_name = str_tvg.Trim()

                        });
                        data.flag_adding_ok = true;
                    }
                    else ct_dublicat++;
                }
            }
            else//ОБНОВЛЕНИЕ
            {
                data.flag_adding_ok = true;
                newname = newname.Trim();

                if (newname != "")
                {

                    int index = 0;
                    bool replace_ok;
                    bool ingore = false;
                    replace_ok = false;

                    foreach (var k in ViewModelMain.myLISTbase)//обновление только отфильтрофанных
                    {
                        if (newname == k.name)
                        {
                            Debug.WriteLine("find " + newname + k.http);
                            int ind = 0;
                            foreach (var j in lst)
                            {
                                if (j.Compare() == k.Compare() && Comparehttp(lst[ind].http, str_http))//находим в полном списке
                                {
                                    lst[ind].http = str_http;

                                    if (ct_update != 0 && (list_update_channels.Find(x => x.Equals(ind)) == ind))
                                    {
                                        ingore = true;   //этот канал уже был обновлен             
                                    }
                                    else
                                    {
                                        list_update_channels.Add(ind);
                                        ct_update++;
                                    }

                                    replace_ok = true;
                                    break;

                                }
                                ind++;

                            }

                            if (replace_ok) { break; }
                        }

                        index++;

                    }
                    if (ingore) ct_ignore_update++;
                }


            }
        }





        bool Comparehttp(string s1, string s2)
        {
            string[] split1 = s1.Split(new Char[] { ':' });
            string[] split2 = s2.Split(new Char[] { ':' });
            if (split1[0] == split2[0]) return true;
            return false;
        }



        public static event Action<typefilter> Event_UpdateLIST;
       

        


        /// <summary>
        /// Чтение строки
        /// </summary>
        /// <param name="sr">указатель</param>
        /// <param name="noFINDscr">true - только чтение без анализа</param>
        /// <returns></returns>
        string ReadFind_script(string line)
        {
            if (line != "") { Wait.progressbar++; Wait.viewstring = line; }
            if (current_mode==mode.not_command || line == "" ) { return line; }

            Debug.WriteLine(name+" поиск наличия скрипта =" + line + ";    " + name);
            line = scripts.FIND_SCRIPT(line);
           

            int tmerr = 0;
            if (!loc.MODE_RELEASE_SCRIPT) return line;

            while (current_mode == mode.with_command)
            {   
                tmerr++; Thread.Sleep(3000);
                Debug.WriteLine("ожидание завершения скрипта " + name);
                if (!loc.MODE_RELEASE_SCRIPT) break;
            }

            return line;
        }



        public static bool linkIsBad(string line)
        {
            if (line == "" || line == " ") return true;
            if (new Regex("#EXTVLCOPT", RegexOptions.IgnoreCase).Match(line).Success) return true;
            if (new Regex("#EXTGRP", RegexOptions.IgnoreCase).Match(line).Success) return true;
            return false;

            //!(new Regex("#EXTSIZE:", RegexOptions.IgnoreCase).Match(line).Success) &&
            //                        !(new Regex("#EXTBG", RegexOptions.IgnoreCase).Match(line).Success) &&
            //                        !(new Regex("#EXTCTRL", RegexOptions.IgnoreCase).Match(line).Success) &&
            //                        !(new Regex("#EXTVLCOPT", RegexOptions.IgnoreCase).Match(line).Success) &&
            //                          regex_link.Match(line).Succe
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">return true==link;  false=file</param>
        /// <returns></returns>
        bool AnalizTYPElink(string line)
        {
            Regex regex_link = new Regex("http://");
            Regex regex_link2 = new Regex("https://");

            if (!(new Regex("EXTM3U", RegexOptions.IgnoreCase).Match(line).Success) &&
                      !(new Regex("url-tvg", RegexOptions.IgnoreCase).Match(line).Success) &&
                      !(new Regex("#EXTSIZE:", RegexOptions.IgnoreCase).Match(line).Success) &&
                      !(new Regex("#EXTBG", RegexOptions.IgnoreCase).Match(line).Success) &&
                      !(new Regex("#EXTCTRL", RegexOptions.IgnoreCase).Match(line).Success) &&
                      !(new Regex("#EXTVLCOPT", RegexOptions.IgnoreCase).Match(line).Success) &&
                      !(new Regex("#EXTGRP", RegexOptions.IgnoreCase).Match(line).Success) &&
                        (regex_link.Match(line).Success || regex_link2.Match(line).Success)
                     ) return true;
            return false;
        }

        public string ANALIZ_LINE(string line, out bool typelink)
        {
            string rezult = line;
            Regex regex_link = new Regex("http://");
            Regex regex_link2 = new Regex("https://");
            typelink = true;

            if (linkIsBad(line)) return FileWork.Get_m3uPath() + rezult;

            if (AnalizTYPElink(line))
            {
                Debug.WriteLine("find link >>>" + line);
                rezult = data.temppath;
            }
            else return FileWork.Get_m3uPath() + rezult;
            //--------------------------

            typelink = false;
            return rezult;
        }


        public static bool chek_update = false, chek_hoop = true;
        public static string text_title = "";
        public static List<ParamCanal> wblst;

      

        /// /// <summary>
        /// from clipboard
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="str"></param>
        public void OPEN_FROM_CLIPBOARD(List<ParamCanal> lst, string[] str)
        {

            Regex regex1 = new Regex("#EXTINF");
            Regex regex2 = new Regex("#EXTM3U");
            Match match = null;

            if (str == null) { dialog.Show("Буфер пустой"); return; }
            if (str.Length == 1)
            {
                Regex regex3 = new Regex("http:");
                Regex regex4 = new Regex(".m3u");
                match = regex3.Match(str[0]);

                if (match.Success)
                {
                    match = regex4.Match(str[0]);
                    if (match.Success)
                    {
                        var web = new FileWork();
                        wblst = lst;
                        web.Get_from_web(str[0]);
                        if (Event_UpdateLIST != null) Event_UpdateLIST(typefilter.last);
                        Wait.Close();
                    }
                }
            }


            if (Event_UpdateLIST != null) Event_UpdateLIST(typefilter.last);
            Wait.Close();
            loc.openfile = false;
        }

    }


    //=============================

}




