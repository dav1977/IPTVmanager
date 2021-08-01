using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using IPTVman.Model;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Net;
using System.Windows.Shapes;

namespace IPTVman.ViewModel
{
    class FileWork
    {
        
        //public static string pathAppication = Environment.ExpandEnvironmentVariables(@"%APPDATA%\IPTVmanager");

       

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, IntPtr dwProcessId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool TerminateProcess(int hProcess, uint uExitCode);

        const uint PROCESS_TERMINATE = 0x1;

        public static string Get_ApplPath()
        {
            string path = "";
            path = System.Reflection.Assembly.GetExecutingAssembly().Location ;
            path = path.Replace(@"\", @"/");
            path = path.Replace(@"IPTVmanager.exe", @"");
            return path;
        }

        static string pathM3u="";
        public static string Get_m3uPath()
        {
            Debug.WriteLine("PATHm3u="+ pathM3u);
            if (pathM3u == "")
            {
                pathM3u = Get_ApplPath();
            }
            else
            {
                string[] words = pathM3u.Split(new char[] { '\\' });
                pathM3u = "";

                for (byte i = 0; i < words.Length-1; i++)
                {
                    
                    pathM3u += words[i]+"/";
                }
            }
            Debug.WriteLine("PATHm3u X=" + pathM3u);
            return pathM3u;
        }

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

        /// <summary>
        ///  SAVE
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="title"></param>
        public void SAVE(List<IPTVman.Model.ParamCanal> lst, string title)
        {

            SaveFileDialog openFileDialog = new SaveFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamWriter sr = new StreamWriter(openFileDialog.FileName))
                {
                    //TITLE
                    if (title!="") sr.Write("#EXTM3U "+title + '\n');

                    string n = "";
                    foreach (var obj in lst)
                    {
                        n = "";
                        if (title == @"$BorpasFileFormat=" + '"' + '1' + '"') n += "#EXTINF:-1";
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

        
        
        

        /// <summary>
        ///  LOAD
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lst"></param>
        /// <param name="_text_title"></param>
        /// <param name="_chek1"></param>
        /// <param name="_chek2"></param>
        public async void LOAD(string path, List<ParamCanal> lst, bool _chek1, bool _chek2)
        {
            Parse.chek_update = _chek1;
            Parse.chek_hoop = _chek2;

            string name = "";

            if (path == "")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    name = openFileDialog.FileName;
                }
                else { loc.openfile = false;  return; }
            }
            else name = path;

            pathM3u = name;
            await AsyncOPEN(lst, name);
          
        }

        public async void LOAD(List<IPTVman.Model.ParamCanal> lst, string namefile)
        {
            //while (loc.asyncOPEN) Thread.Sleep(100);
            await AsyncOPEN(lst, namefile);
        }


        /// <summary>
        /// событие после выполнения задачи
        /// </summary>
        public event Action Task_Completed;
        public async Task<string> AsyncOPEN(List<ParamCanal> lst, string name)
        {
            var tcs = new TaskCompletionSource<string>();
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            var taskload = Task.Run(() =>
            {               
                try
                {
                    loc.asyncOPEN = true;
                    Wait.Create("Анализ файла ...", true);

                    if (name != null)
                    {
                        //РЕЖИМ ЗАПУСКА ПАРСИНГА С ОБРАБОТКОЙ КОМАНД ИЛИ БЕЗ
                        bool mode = Parse.mode.with_command; string nm = " KEYOP";//по кнопке

                        if (Script.working) 
                             { mode = Parse.mode.not_command; nm = " MODESCRPT"; }//по скрипту

                        data.flag_adding_ok = false;
                        var p = new Parse(mode,nm);
                        p.PARSING_FILE(lst, name, false);
                    }
                    else loc.openfile = false;

                    Thread.Sleep(300);
          
                    Wait.Close();
                   
                    Script.ResetMODEApplication();

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
                loc.asyncOPEN = false;
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
                dialog.Show("ОШИБКА-ПАРСИНГА ФАйЛА " + ex.Message);
            }         
            return tcs.ToString();
        }


       

     


       // List<string> bufferstring = new List<string>();
        public void add_file_to_memory(string path)
        {
            //Debug.WriteLine("Start to MEMORY "+path);

            //try
            //{
            //    using (StreamReader sr = new StreamReader(path))
            //    {
            //        while (!sr.EndOfStream)
            //        {
            //            try
            //            {
            //                string s = sr.ReadLine();

            //                Debug.WriteLine(s);
            //                bufferstring.Add(s);
            //            }
            //            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            //        }
            //    }
            //}
            //catch { Debug.WriteLine("CRASH to MEMORY " + path);  }

            //Debug.WriteLine("END Start to MEMORY " + path);
        }

        void add_memory_to_file(string path)
        {
            //Debug.WriteLine("MEMORY TO FILE " + path);
            //try
            //{
            //    using (StreamWriter sr = new StreamWriter(path))
            //    {
            //        foreach (string s in bufferstring)
            //        {
            //            sr.Write(s);
            //            sr.WriteLine();
            //        }
            //    }
            //}
            //catch { Debug.WriteLine("CRASH MEMORY TO FILE " + path);  }
            //Debug.WriteLine("END MEMORY TO FILE " + path);
        }


        public void add_stringS_to_file(string pathfile, string[] str)
        {
            try
            {
                using (StreamWriter sr = new StreamWriter(pathfile))
                {
                    foreach (string s in str)
                    {
                        sr.Write(s);
                        sr.WriteLine();
                    }
                }
            }
            catch
            {
                Debug.WriteLine("CRASH stringS TO FILE ");
            }
        }


        //---------------------------------------------------------------------------------------
        //
        //---------------------------------------------------------------------------------------
        public event Action<string> Event_DownloadLinkCompleted;
        void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Debug.WriteLine("start download COMPLETED!!  size=" + data.temppath.Length.ToString());
            //копия
            add_file_to_memory(data.temppath);
            // wait_download = false;
            if (Event_DownloadLinkCompleted != null)
            {
                Event_DownloadLinkCompleted(data.temppath);
            }

        }

        /// <summary>
        /// Закачка ссылки в temp файл
        /// </summary>
        /// <param name="s"></param>
        public string DownloadLINK(string s0)
        {
            try
            {
                string[] wo = s0.Split(new char[] { ' ' } );
                string s = wo[0];


                Debug.WriteLine("start download to " + data.temppath + " from " + s);
                System.Net.WebClient webClient = new WebClient();
                Wait.Create("Загружается\n" + s, false);
                webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
                webClient.DownloadFile(new Uri(s), data.temppath);

                Debug.WriteLine("download END");
                return s;
            }
            catch
            {
                dialog.Show("Ошибка parsing " + s0);
                Wait.Close();

                loc.openfile = false;
            }
            return "";
        }


        public void WebClient_DownloadFileCompletedClipb(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            data.flag_adding_ok = false;
            Debug.WriteLine("АНАЛИЗ WEB загрузки  " );
            add_file_to_memory(data.temppath);
            parse_temp_file(Parse.wblst);///в Parse.wblst  копия ссылки lst списка
        }


        public void Get_from_web(string link)
        {
            try
            {
                WebClient webClient = new WebClient();
                Wait.Create("Загружается\n" + link, false);
                

                //STOP
                webClient.DownloadFileCompleted += WebClient_DownloadFileCompletedClipb;
                webClient.DownloadFileAsync(new Uri(link), data.temppath);
                return;
            }
            catch (Exception ex)
            {
                dialog.Show("Ошибка загрузки  " + ex.Message);
                Wait.Close();
            }
        }


        public void parse_temp_file(List<ParamCanal> lst)
        {
            var p = new Parse(Parse.mode.with_command," PArTEMPfile");

            if (lst==null) Debug.WriteLine("------PARSE TEMP FILE  CRASH lst-------");
            p.PARSING_FILE(lst, data.temppath, true);
            try
            {
                File.Delete(data.temppath);
            }
            catch (Exception ex) { dialog.Show("ошибка удаления " + ex.Message); }

           
        }
        //-------------------------------------------------------


    }//class
}
