using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace IPTVman.ViewModel
{
    /// <summary>
    /// Всплывающее окно
    /// </summary>
    public partial class WindowPOP : Window
    {
        System.Timers.Timer Timer1;
        byte ct = 0;
        Window handle;
       

        public WindowPOP()
        {
            handle = this;
            InitializeComponent();
            ct = 0;
            if (WinPOP.sec!=0) CreateTimer1(WinPOP.sec);
            label1.Content = WinPOP.message1_win_pop;
            label2.Content = WinPOP.message2_win_pop;
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window1_KeyDown);
        }

        public void CreateTimer1(int ms)
        {
            if (Timer1 == null)
            {
                Timer1 = new System.Timers.Timer();
                //Timer1.AutoReset = false; //
                Timer1.Interval = ms;
                Timer1.Elapsed += Timer1Tick;
                Timer1.Enabled = true;
                Timer1.Start();
            }
        }

        private void Timer1Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            ct++; if (ct > 3) WinPOP.need_to_close = true;
        }

        void Window1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //close any key
            //if (e.Key == System.Windows.Input.Key.Escape)
                WinPOP.loc = false;
                this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {  

        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }

    public static class WinPOP
    {
        public static bool init_ok = false;
        static Window p;
        public static bool need_to_close = false;
        public static string message1_win_pop = "";
        public static string message2_win_pop = "";
        public static byte sec;
        public static bool loc = false;


        public static void Create(string mes, byte s, Window ow)
        {
            if (loc) return;
            loc = true;
            sec = s;

            try
            {
                string bitr = "?";
                message2_win_pop = data._bass.scan_get_tags(data.url, ref bitr);
                if (bitr == "?" || bitr == "0") message1_win_pop = mes;
                else message1_win_pop = mes + "   [" + bitr + " кбит/с ]";

            }
            catch { }

            p = new WindowPOP()
            {
                Title = "",
                //Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                //WindowStyle = WindowStyle.ThreeDBorderWindow,
                Name = "winpop",
 
            };
            p.Owner = ow;
            p.Show();
        }

        public static void Close()
        {
            need_to_close = false;
            p.Close();
            loc = false;
        }
     }

    

}
