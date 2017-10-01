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
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

namespace IPTVman.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для Window
    /// </summary>
    public partial class WindowPING : Window 
    {
        int size = 0;
        System.Timers.Timer Timer1;
        string id = "";
        public WindowPING()
        {
            InitializeComponent();
            button.Visibility = Visibility.Hidden;
            this.MaxHeight = 270;
            this.MaxWidth = 332;
            CreateTimer1(200);
            AUTOPING.Event_Print += new Action<string>(add);
            id = this.Name;
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
                if (Model.data.ping_waiting > 3)
                {
                    textct.Dispatcher.Invoke( new Action(() =>
                    {
                        textct.Text = String.Format("{0} из {1}   ожидание {2} ", IPTVman.Model.data.ct_ping,
                                             Model.data.ping_all, Model.data.ping_waiting);
                    }));

                }
            //----------------------------------------------
                if (STR.Count!=0)
                {
                    string mes = STR.Dequeue();


                    textct.Dispatcher.Invoke(new Action(() =>
                    {
                        textct.Text = String.Format("{0} из {1} ", Model.data.ct_ping, Model.data.ping_all);
                    }));


                    textBox.Dispatcher.Invoke(new Action(() =>
                    {
                        size++;
                        if (size > 500) { textBox.Clear(); size = 0; }

                        if (mes == "end")
                        {
                            _writingProgressBar.Visibility = Visibility.Hidden;
                            mes = "== АВТО ПИНГ ЗАКОНЧЕН==";
                            tb1.Text = "  ВЫПОЛНЕНО";
                            button.Visibility = Visibility.Visible;
                        }
                        textBox.AppendText(mes + Environment.NewLine);
                        textBox.ScrollToEnd();


                    }));
                }


            }
            catch { }
        }
        // private object threadLock = new object();

        Queue<string> STR = new Queue<string>();
        /// <summary>
        /// ДОБАВЛЕНИЕ сообщения. для запуска из другого потока
        /// </summary>
        void add(string s)
        {
            STR.Enqueue(s);
            Trace.WriteLine(" ping WINDS  " + s+ "  id="+ id);
        }

        //key ЗАКРЫТЬ
        private void button_Click(object sender, RoutedEventArgs e)
        {  
            this.Close();  
        }

        //close
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Model.data.ping_waiting = 0;
        }

    }
}
