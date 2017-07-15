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
        public WindowWAIT()
        {
            InitializeComponent();
            label.Content = IPTVman.Model.loc.longtaskSTRING;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

    }

    public static class Wait
    {
        static Window wait = null;

        public static void Create(string meesage)
        {
            if (WindowIsOpen()) Close();

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

        public static bool WindowIsOpen()
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
