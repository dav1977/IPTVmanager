using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Globalization;
using System.Threading;
using System.Reflection;
using System.Diagnostics;

namespace runIPTVMANAGER
{
    class Program
    {
        [DllImportAttribute("shell32.dll")]
        public static extern int ShellExecuteA(int hwnd, string Operation, string File, string Parameters, string Directory, int ShowCmd);

        //[DllImportAttribute("winmm.dll")]
        // public static extern long PlaySound(String lpszName, long hModule, long dwFlags); 

        static void Main(string[] args)
        {

            string p = System.IO.Directory.GetCurrentDirectory();// Assembly.GetExecutingAssembly().Location;// Process.GetCurrentProcess();
            //System.IO.Directory.GetCurrentDirectory();
                                                                //  string s2 = s1.Remove(s1.LastIndexOf("\\"));
                                                                // string pathcatalog = s2.Remove(s2.LastIndexOf("\\")) + "\\";

          
            /// System.Diagnostics.Process.Start("cmd", @"cd..");

            try {

                Process myProcess = new Process();
                myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                myProcess.StartInfo.FileName = p+"\\IPTVmanager.exe";
                myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                myProcess.StartInfo.Arguments = @"radio.m3u";
                //myProcess.StartInfo.Arguments = @"script1.m3u";
                //myProcess.StartInfo.Arguments = @"script2.m3u";
                //myProcess.StartInfo.Arguments = @"script3.m3u";


                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.RedirectStandardError = true;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.CreateNoWindow = true;
                myProcess.Start();

 

            } catch(Exception ex)
            {
                Console.WriteLine("error "+ex.ToString());
                Console.ReadKey();
            }



            //Process myProcess = new Process();
            //myProcess.StartInfo.FileName = "cmd.exe";
            //myProcess.StartInfo.Arguments = @"/C cd " + Application.StartupPath + "/server/login & start.bat";
            //myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //myProcess.StartInfo.CreateNoWindow = true;
            //myProcess.Start();


            //ShellExecuteA(0, "Open", p+"run1.bat", "", p, 11);
            //ShellExecuteA(0, "Open", p+"run2.bat", "", p, 11);

            //ShellExecuteA(0, "Open", p+"run3.bat", "", p, 11);
            //ShellExecuteA(0, "Open", p+ "run4.bat", "", p, 11);

            //Console.ReadKey();
        }
    }
}
