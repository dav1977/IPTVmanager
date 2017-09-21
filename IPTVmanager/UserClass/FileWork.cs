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

namespace IPTVman.ViewModel
{
    class FileWork
    {
        public static event Action<typefilter> Event_UpdateLIST;

        readonly string tempname = "temp_m3u_IPTVmanager";

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, IntPtr dwProcessId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool TerminateProcess(int hProcess, uint uExitCode);

        const uint PROCESS_TERMINATE = 0x1;

        private static void TerminateProcess(IntPtr PID)
        {
            IntPtr hProcess = OpenProcess(PROCESS_TERMINATE, false, PID);

            if (hProcess == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception(
                    System.Runtime.InteropServices.Marshal.GetLastWin32Error());

            if (!TerminateProcess((int)hProcess, 0))
                throw new System.ComponentModel.Win32Exception(
                    System.Runtime.InteropServices.Marshal.GetLastWin32Error());

            CloseHandle(hProcess);
        }

        /// <summary>
        /// Удаление по TITLE
        /// </summary>
        /// <param name="nameTITLE"></param>
        /// <returns></returns>
        public static bool KILL_PROCESS(string nameTITLE)
        {
            //Проходимся по всем процессам локального компьютера.
            foreach (System.Diagnostics.Process clsProcess in
                               System.Diagnostics.Process.GetProcesses())
            {
                //Определяем, совпадает ли начало имени процесса с указанным.
                //Если да, то метод "StartsWith" возвращает значение true
                //и вызывается метод удаления процесса,
                //в противном случае — значение false и происходит
                //переход к следующему процессу.
                if (/*clsProcess.ProcessName.StartsWith(name) &&*/ clsProcess.MainWindowTitle == nameTITLE)
                {
                    TerminateProcess((IntPtr)clsProcess.Id);

                    if (clsProcess.HasExited) { return true; }//"Процесс успешно завершён!"

                    return false;

                }
            }
            return true;
        }


        public void SAVE(List<IPTVman.Model.ParamCanal> lst, string title)
        {

            SaveFileDialog openFileDialog = new SaveFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamWriter sr = new StreamWriter(openFileDialog.FileName))
                {
                    sr.Write(title + '\n');

                    string n = "";
                    foreach (var obj in lst)
                    {
                        n = "";
                        if (title == @"#EXTM3U $BorpasFileFormat=" + '"' + '1' + '"') n += "#EXTINF:-1";
                        else n += "#EXTINF:0";

                        if (obj.ExtFilter != "") n += " $ExtFilter=" + '"' + obj.ExtFilter + '"';
                        if (obj.group_title != "") n += " group-title=" + '"' + obj.group_title + '"';
                        if (obj.logo != "") n += " tvg-logo=" + '"' + obj.logo + '"';
                        if (obj.tvg_name != "") n += " tvg-name=" + '"' + obj.tvg_name + '"';

                        n += "," + obj.name + '\n';
                        sr.Write(n + obj.http + '\n');
                    }
                }

            }

        }

        bool chek1=false, chek2=false;
        public string text_title = "";
        /// <summary>
        /// куда, заголовок,  флаг только обновлять, флаг обрезать скобки
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="_text_title"></param>
        /// <param name="_chek1"></param>
        /// <param name="_chek2"></param>
        public async void LOAD(List<IPTVman.Model.ParamCanal> lst, string _text_title, bool _chek1, bool _chek2)
        {
            text_title = _text_title;
            chek1 = _chek1;
            chek2 = _chek2;

            string name = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                name = openFileDialog.FileName;
            }
            else { loc.openfile = false; return; }

            await AsyncOPEN(lst, name);
        }

        public async void LOAD(List<IPTVman.Model.ParamCanal> lst, string namefile)
        {
            await AsyncOPEN(lst, namefile);
        }



        /// <summary>
        /// событие после выполнения задачи
        /// </summary>
        public event Action Task_Completed;
        public async Task<string> AsyncOPEN(List<IPTVman.Model.ParamCanal> lst, string name)
        {
            var tcs = new TaskCompletionSource<string>();
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            var taskload = Task.Run(() =>
            {               
                try
                {
                    Wait.Create("Идет анализ файла", true);
                    mode_work_with_links = true;
                    bufferstring.Clear();
                    if (name != "")  PARSING_FILE(lst, name);
                    else loc.openfile = false;
                  
                    Thread.Sleep(300);
                    Wait.Close();
                    if (Event_UpdateLIST != null) Event_UpdateLIST(typefilter.normal);
                    Wait.Close();
                    bufferstring.Clear();
                    tcs.SetResult("ok");                 
                }
                catch (OperationCanceledException e)
                {
                    tcs.SetException(e);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                return tcs.Task;
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try
            {
                var result = await taskload;      
                if (Task_Completed != null) Task_Completed();            
            }
            catch (Exception ex)
            {
                dialog.Show("ОШИБКА-ПАРСИНГА ФАйЛА " + ex.Message.ToString());
            }         
            return tcs.ToString();
        }




        void Parsing_link(string s)
        {
            string ss = s;
            //try { ss = HttpUtility.UrlEncode(s); } catch { ss = s; } 
            try
            {
                string path = System.IO.Path.GetTempPath() + tempname;
                System.Net.WebClient webClient = new WebClient();
                Wait.Create("Загружается\n" + ss, false);
                webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                webClient.DownloadFileAsync(new Uri(ss), path);
                return;
            }
            catch
            {
                dialog.Show("Ошибка parsing " + s);
                Wait.Close();
                wait_download = false;
                loc.openfile = false;
            }
        }


        bool wait_download = false;
        bool mode_work_with_links = false;
        /// <summary>
        /// ПАРСИНГ
        /// </summary>
        /// <param name="name"></param>
        void PARSING_FILE(List<IPTVman.Model.ParamCanal> lst, string name)
        {
            bool flag_adding_ok = false;
            uint ct_dublicat = 0;
            uint ct_update = 0;
            uint ct_ignore_update = 0;
            string line = null;
            string newname = "";
            List<int> list_update_channels = new List<int>();

            try
            {              
                Regex regex_link = new Regex("http://");
                Regex regex_link2 = new Regex("https://");
                Regex regex1 = new Regex("#EXTINF");
                Regex regex2 = new Regex("#EXTM3U");
                Match match = null;

                int all_str = 0;
                byte null_str = 0;

                try
                {
                    //определение макс строк СКАНЕР ФАЙЛА
                    using (StreamReader sr = new StreamReader(name))
                    {
                        while (!sr.EndOfStream)
                        {
                            string rez = sr.ReadLine();
                            if (rez != "" && rez != null)
                                if (new Regex("#EXTINF").Match(rez).Success) { all_str++; null_str = 0; }
                                else null_str++; if (null_str > 100) goto exit_open;
                            match = regex2.Match(rez);
                            if (match.Success) text_title = rez;
                        }
                    }

                }
                catch (Exception ex) { MessageBox.Show("ошибка сканирования " + ex.Message.ToString()); goto exit_open; }

                Wait.set_ProgressBar(all_str);
                //=========================================================
                //ПОИСК каналов
                using (StreamReader sr = new StreamReader(name))
                {
                    string str_ex = "", str_name = "", str_http = "", str_gt = "", str_logo = "", str_tvg = "";
                    bool yes = false;

                    while (!sr.EndOfStream)
                    {
                        try
                        {
                            line = sr.ReadLine();
                            Wait.progressbar++;
                        }
                        catch { }

                        if (linkIsBad(line)) continue;
                        //MessageBox.Show(line);

                        //*******************************************************//-------------------- закачка Links
                        if (mode_work_with_links)
                        {
                   
                            while (wait_download) Thread.Sleep(100);

                            if     (!(new Regex("EXTM3U", RegexOptions.IgnoreCase).Match(line).Success) &&
                                    !(new Regex("url-tvg", RegexOptions.IgnoreCase).Match(line).Success) &&
                                    !(new Regex("#EXTSIZE:", RegexOptions.IgnoreCase).Match(line).Success) &&
                                    !(new Regex("#EXTBG", RegexOptions.IgnoreCase).Match(line).Success) &&
                                    !(new Regex("#EXTCTRL", RegexOptions.IgnoreCase).Match(line).Success) &&
                                    !(new Regex("#EXTVLCOPT", RegexOptions.IgnoreCase).Match(line).Success) &&
                                      (regex_link.Match(line).Success|| regex_link2.Match(line).Success)
                                   )
                            {
                                mode_work_with_links = true;                              
                                Parsing_link(line);
                                wait_download = true;
                                flag_adding_ok = true;
                                continue;
                            }
                            //--------------------------

                            if (mode_work_with_links)
                            {
                                mode_work_with_links = false;
                                if (flag_adding_ok)
                                {
                                    parse_temp_file(lst); return;
                                }
                            }//рекурсия

                        }
                        //***********************************************************

                        match = regex1.Match(line);
                        if (match.Success) yes = true; else yes = false;

                        ///========== разбор EXINF
                        if (yes)
                        {
                            Regex regex3 = new Regex("ExtFilter=", RegexOptions.IgnoreCase);
                            Regex regex4 = new Regex("group-title=", RegexOptions.IgnoreCase);
                            Regex regex5 = new Regex("logo=", RegexOptions.IgnoreCase);
                            Regex regex6 = new Regex("tvg-name=", RegexOptions.IgnoreCase);

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
                            //чтение ссылки

                            while (!sr.EndOfStream)
                            {
                                try
                                {
                                    str_http = sr.ReadLine();
                                }
                                catch { }

                                if (!linkIsBad(str_http)) break;
                            }

                            //MessageBox.Show(str_http);
                        }

                        if (chek2)//обрезание скобок
                        {
                            //string[] words = newname.Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries);
                            var ind = newname.LastIndexOf('(');
                            if (ind > 0) newname = newname.Substring(0, ind);
                            newname = newname.Trim();
                        }


                        if (!chek1)//Добавление
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
                                    flag_adding_ok = true;
                                }
                                else ct_dublicat++;
                            }
                        }
                        else//ОБНОВЛЕНИЕ
                        {
                            flag_adding_ok = true;
                            bool fl = false;
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
                                        int ind = 0;
                                        foreach (var j in lst)
                                        {
                                            if (j.Compare() == k.Compare() && Comparehttp(lst[ind].http, str_http))//находим в полном списке
                                            {
                                                lst[ind].http = str_http;
                                                //if (list_update_channels.Exist(x => x.Equals(ind))) 
                                                if (ct_update != 0 && (list_update_channels.Find(x => x.Equals(ind)) == ind))
                                                {
                                                    ingore = true;   //этот канал уже был обновлен             
                                                }
                                                else
                                                {
                                                    list_update_channels.Add(ind);
                                                    ct_update++;
                                                }

                                                fl = true;
                                                replace_ok = true;
                                                break;

                                            }
                                            ind++;

                                        }

                                        if (replace_ok) { break; }
                                        break;
                                    }
                                    else
                                    {

                                    }
                                    index++;

                                }
                                if (ingore) ct_ignore_update++;
                                if (!fl) { }// MessageBox.Show("не обновленно " + newname);
                            }


                        }

                    }///чтение фала


                }

            }
            catch { }

            if (mode_work_with_links)
            {
                while (wait_download) Thread.Sleep(100);
                mode_work_with_links = false;
                if (flag_adding_ok)
                {
                    parse_temp_file(lst); return;
                }
            }//рекурсия


        exit_open:
            string addstr = "";
            if (!flag_adding_ok) dialog.Show("Каналы не обнаружены");
            if (ct_dublicat != 0) dialog.Show("Пропущенно дублированных ссылок " + ct_dublicat.ToString());
            if (ct_ignore_update != 0) addstr = "\nПропущено дублирование " + ct_ignore_update.ToString();
            if (ct_update != 0) dialog.Show("ОБНОВЛЕННО " + ct_update.ToString() + " каналов" + addstr);
            loc.openfile = false;
        }



        bool linkIsBad(string line)
        {
            if (line == "" || line == " ") return true;
            if (new Regex("#EXTVLCOPT", RegexOptions.IgnoreCase).Match(line).Success) return true;
            return false;

            //!(new Regex("#EXTSIZE:", RegexOptions.IgnoreCase).Match(line).Success) &&
            //                        !(new Regex("#EXTBG", RegexOptions.IgnoreCase).Match(line).Success) &&
            //                        !(new Regex("#EXTCTRL", RegexOptions.IgnoreCase).Match(line).Success) &&
            //                        !(new Regex("#EXTVLCOPT", RegexOptions.IgnoreCase).Match(line).Success) &&
            //                          regex_link.Match(line).Succe
        }

        bool Comparehttp(string s1, string s2)
        {
            string[] split1 = s1.Split(new Char[] { ':' });
            string[] split2 = s2.Split(new Char[] { ':' });
            if (split1[0] == split2[0]) return true;
            return false;
        }


        List<string> bufferstring = new List<string>();
        void add_file_to_memory(string name)
        {
            using (StreamReader sr = new StreamReader(name))
            {
                while (!sr.EndOfStream)
                {
                    try
                    {
                        bufferstring.Add(sr.ReadLine());
                    }
                    catch (Exception ex) { MessageBox.Show(ex.ToString()); }
                }
            }
        }

        void add_memoty_to_file(string name)
        {
            using (StreamWriter sr = new StreamWriter(name))
            {
                foreach (string s in bufferstring)
                {
                    sr.Write(s);
                    sr.WriteLine();
                }
            }
        }

        private void parse_temp_file(List<IPTVman.Model.ParamCanal> lst)
        {
            string path = System.IO.Path.GetTempPath() + tempname;
            add_memoty_to_file(path);
            PARSING_FILE(lst, path);
            try
            {
                System.IO.File.Delete(path);
            }
            catch (Exception ex) { dialog.Show("ошибка удаления " + ex.Message.ToString()); }
            if (Event_UpdateLIST!=null) Event_UpdateLIST(typefilter.last);
            Wait.Close();
        }

        private void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //копия
            string path = System.IO.Path.GetTempPath() + tempname;
            add_file_to_memory(path);
            wait_download = false;
        }






        /// <summary>
        /// from clipboard
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="str"></param>
        public void OPEN_FROM_CLIPBOARD(List<IPTVman.Model.ParamCanal> lst, string[] str)
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
                        try
                        {
                            string path = System.IO.Path.GetTempPath() + tempname;
                            WebClient webClient = new WebClient();
                            Wait.Create("Загружается\n" + (str[0].ToString()), false);
                            wblst = lst;
                            webClient.DownloadFileCompleted += WebClient_DownloadFileCompletedClipb;
                            webClient.DownloadFileAsync(new Uri(str[0].ToString()), path);
                            return;
                        }
                        catch (Exception ex)
                        {
                            dialog.Show("Ошибка parsclb " + ex.Message.ToString());
                            Wait.Close();
                        }
                    }
                }
            }

            if (chek1) { dialog.Show("ОБНОВЛЕНИЕ ТОЛЬКО ЧЕРЕЗ ФАЙЛ"); return; }
            //ПОИСК заголовка
            foreach (var s in str)
            {
                match = regex2.Match(s);
                if (match.Success) { text_title = s; break; }
            }

            bool need_link = false;
            // dialog.Show(str.Length.ToString());
            int ct_dublicat = 0;
            int index = 0;
            bool flag_adding = false;

            string newname = "";
            string str_ex = "", str_name="", str_http = "", str_gt = "", str_logo = "", str_tvg = "";
            //ПОИСК каналов
            foreach (var line in str)
            {
                index++;

                //поиск ссылки
                if (need_link)
                {
                    try
                    {
                        str_http = line.Trim();// str[index].Substring(0, str[index].Length - 1);//remove перевода строки
                        if (str_http == "") { continue; }
                    }
                    catch { }
                }
                else { str_name = ""; newname = ""; }

                if (!need_link)
                {

                    if (line == null || line == "" || line == "\r" || line == "\n") continue;
                    string lineNEW = line.Replace("\r", "");
                    lineNEW = lineNEW.Replace("\n", "");


                    match = regex1.Match(lineNEW);
                    ///========== разбор EXINF
                    if (match.Success)
                    {
                        Regex regex3 = new Regex("ExtFilter=", RegexOptions.IgnoreCase);
                        Regex regex4 = new Regex("group-title=", RegexOptions.IgnoreCase);
                        Regex regex5 = new Regex("logo=", RegexOptions.IgnoreCase);
                        Regex regex6 = new Regex("tvg-name=", RegexOptions.IgnoreCase);

                        string[] split = lineNEW.Split(new Char[] { '"' });

                        str_name = "";
                        str_ex = ""; str_name = ""; str_http = ""; str_gt = ""; str_logo = ""; str_tvg = "";

                        if (split.Length == 0)
                        {
                            newname = lineNEW;
                        }
                        else
                        {

                            split = lineNEW.Split(new Char[] { ',' });

                            if (split.Length < 1) { dialog.Show("Буфер пустой"); return; }

                            if (split.Length <= 2) str_name = split[split.Length - 1];


                            if (str_name != "") newname = str_name;
                            need_link = true; continue;


                            //int i = 0;
                            //for (i = 0; i < split.Length; i++)
                            //{
                            //    string s = split[i];
                            //    //------------- разбор строки EXINF
                            //    if (str_ex == "!") str_ex = s;
                            //    if (str_gt == "!") str_gt = s;
                            //    if (str_logo == "!") str_logo = s;
                            //    if (str_tvg == "!") str_tvg = s;


                            //    if (i >= split.Length - 1) { str_name = s; }

                            //    match = regex3.Match(s);
                            //    if (match.Success)
                            //    {
                            //        str_ex = "!";
                            //    }

                            //    match = regex4.Match(s);
                            //    if (match.Success)
                            //    {
                            //        str_gt = "!";
                            //    }

                            //    match = regex5.Match(s);
                            //    if (match.Success)
                            //    {
                            //        str_logo = "!";
                            //    }

                            //    match = regex6.Match(s);
                            //    if (match.Success)
                            //    {
                            //        str_tvg = "!";
                            //    }


                            //}//for
                        }


                    }
                    else continue;
                }
                need_link = false;
  
                ParamCanal item = null;
                if (newname == "") continue;

                item = lst.Find(x =>
                 (x.http == str_http && x.ExtFilter == str_ex && x.group_title == str_gt));

                if (newname.Trim() != "" && str_http.Trim() != "")
                {
                    if (item == null)
                    {
                        flag_adding = true;
                        lst.Add(new ParamCanal
                        {
                            name = newname.Trim(),
                            ExtFilter = str_ex.Trim(),
                            http = str_http.Trim(),
                            group_title = str_gt.Trim(),
                            logo = str_logo.Trim(),
                            tvg_name = str_tvg.Trim()

                        });
                    }
                    else ct_dublicat++;
                }
            }

            if (flag_adding)
            {
                if (ct_dublicat != 0) dialog.Show("ПРОПУЩЕНО ДУБЛИРОВАННЫХ ССЫЛОК " + ct_dublicat.ToString());
                else dialog.Show("Каналы добавлены успешно!");
            }
            else dialog.Show("Ссылки не распознаны");

            if (Event_UpdateLIST != null) Event_UpdateLIST(typefilter.last);
            Wait.Close();
            loc.openfile = false;
        }




        List<ParamCanal> wblst;
        private void WebClient_DownloadFileCompletedClipb(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string path = System.IO.Path.GetTempPath() + tempname;
            add_file_to_memory(path);
            parse_temp_file(wblst);
        }



    }//class
}
