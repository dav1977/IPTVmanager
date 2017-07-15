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

namespace IPTVman.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для WindowWAIT.xaml
    /// </summary>
    public partial class WindowWAIT : Window
    {
        System.Timers.Timer Timer1;
        string message;

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
            if (IPTVman.Model.loc.enable_ostatok)
            {
                //label.Content = message + IPTVman.Model.loc.ostatok.ToString();
            }
        }


        public WindowWAIT()
        {
            InitializeComponent();
            CreateTimer1(500);
            label.Content = IPTVman.Model.loc.longtaskSTRING;
            message = IPTVman.Model.loc.longtaskSTRING;
        }

       

    }


    public static class Wait
    {
        public static Window wait = null;

        public static void Create(string meesage)
        {
            if (WaitIsOpen()) Close();


            IPTVman.Model.loc.enable_ostatok = false;
            IPTVman.Model.loc.longtaskSTRING = meesage;

            wait = new WindowWAIT()
            {
                Title = "",
                Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                Name = "winwait"
            };

            wait.Show();
        }

        public static bool WaitIsOpen()
        {
            if (wait != null) return true;
            else return false;
        }

        public static void Close()
        {
            if (wait == null) return;
            wait.Close();
            wait = null;
        }
    }



    public static class LongtaskCANCELING
    {
        static PING _ping;
        static PING_prepare _ping_prepare;

        static bool en;
        public static bool isENABLE()
        {

            if (_ping_prepare == null) return en;
            else
            {
                if (_ping_prepare.stateTASKisCanceled()) { stop(); return false; }
                else return true;
            }
        }

        public static void enable(PING p, PING_prepare pp)
        {
            _ping = p;
            _ping_prepare = pp;
            en = true;
        }

        public static void enable()
        {
            en = true;
        }

        public static void stop()
        {
            _ping = null;
            _ping_prepare = null;
            en = false;
        }


        public static void analiz_closing_thread(PING p, PING_prepare prep)
        {

            if (p != null) p.stop();
            if (prep != null)
            {
                prep.stop();

                byte ct = 0;
                while (!prep.stateTASKisCanceled())
                {
                    ct++;
                    if (ct == 5)
                    {
                        LongtaskCANCELING.enable();
                        Wait.Create("Ждите идет прерывание пинга");

                        LongtaskCANCELING.enable(p, prep);
                        break;
                    }
                    Thread.Sleep(100);
                }

            }

            prep = null;
            p = null;

        }


    }

}
