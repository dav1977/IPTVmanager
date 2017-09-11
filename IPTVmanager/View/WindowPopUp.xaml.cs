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
        int needsec;

        public WindowPOP()
        {
            handle = this;
            InitializeComponent();
            ct = 0;

            if (WinPOP.sec != 0) { needsec = WinPOP.sec; CreateTimer1(1000); }
            label.Content = WinPOP.message_win_pop;
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
            ct++; if (ct > needsec) WinPOP.need_to_close = true;
        }

        void Window1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape) this.Close();
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
        static Window p;
        public static bool need_to_close = false;
        public static string message_win_pop = "";
        public static byte sec;
        public static bool loc = false;


        public static void Create(string mes, byte s, Window ow)
        {
            if (loc) return;
            loc = true;
            sec = s;
            message_win_pop = mes;
            p = new WindowPOP()
            {
                Title = "",
                Owner = ow,
                Topmost = true,

                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                //WindowStyle = WindowStyle.ThreeDBorderWindow,
                Name = "winpop",
 
            };
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
