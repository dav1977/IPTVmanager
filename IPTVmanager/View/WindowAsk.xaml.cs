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
    //
    /// </summary>
    public partial class WindowAsk : Window
    {
        System.Timers.Timer Timer1;
        byte poz = 0;

        public WindowAsk()
        {
            InitializeComponent();
            txtMessage.Text  = MessageAsk.message;
            CreateTimer1(679);
            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window1_KeyDown);
        }

        void Window1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
           // if (e.Key == System.Windows.Input.Key.Enter) { DialogResult = true; this.Close(); }
            if (e.Key == System.Windows.Input.Key.Escape) { DialogResult = false; this.Close(); }
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
            try
            {  
                r1.Dispatcher.Invoke(new Action(() =>
                {
                    r1.Visibility = Visibility.Hidden;
                }));
                r2.Dispatcher.Invoke(new Action(() =>
                {
                    r2.Visibility = Visibility.Hidden;
                }));
                r3.Dispatcher.Invoke(new Action(() =>
                {
                    r3.Visibility = Visibility.Hidden;
                }));
                r4.Dispatcher.Invoke(new Action(() =>
                {
                    r4.Visibility = Visibility.Hidden;
                }));

                poz++; if (poz > 4) poz = 1;

                if (poz == 1)
                {
                    r1.Dispatcher.Invoke(new Action(() =>
                    {
                        r1.Visibility = Visibility.Visible;
                    }));
                }
                if (poz == 2)
                {
                    r2.Dispatcher.Invoke(new Action(() =>
                    {
                        r2.Visibility = Visibility.Visible;
                    }));
                }
                if (poz == 3)
                {
                    r3.Dispatcher.Invoke(new Action(() =>
                    {
                        r3.Visibility = Visibility.Visible;
                    }));
                }
                if (poz == 4)
                {
                    r4.Dispatcher.Invoke(new Action(() =>
                    {
                        r4.Visibility = Visibility.Visible;
                    }));
                }

            }
            catch { }

        }





        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void bn1_Click(object sender, RoutedEventArgs e)
        {
            MessageAsk.rez = 1;
            this.Close();
        }

        private void bn2_Click(object sender, RoutedEventArgs e)
        {
            MessageAsk.rez = 2;
            this.Close();
        }
    }

    public static class MessageAsk
    {
        public  static byte rez = 0;
        public static string message;
        static Window ask;
         public static MessageBoxResult Create(string s)
        {
            message = s;
            rez = 0;

            ask = new WindowAsk
            {
                Title = "",
                Topmost = true,
                //WindowStyle = WindowStyle.ThreeDBorderWindow,
                Name = "winask"
            };

            ask.ShowDialog();
            if (rez == 1) return MessageBoxResult.Yes;
            if (rez == 2) return MessageBoxResult.No;

            return MessageBoxResult.Cancel;

        }

       

    }

}
