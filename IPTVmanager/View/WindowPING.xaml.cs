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
    public delegate void Delegate_Print(string s);
    /// <summary>
    /// Логика взаимодействия для Window
    /// </summary>
    public partial class WindowPING : Window 
    {
        static PING _ping;
        static PING_prepare _pingPREPARE;

        public static event Delegate_UpdateCollection Event_Refresh;

        int size = 0;
        AUTOPING ap;
        bool update_ok=false;
        System.Timers.Timer Timer1;
        
        public WindowPING()
        {
            InitializeComponent();

            _ping = new PING();
            _pingPREPARE = new PING_prepare(_ping);

            button.Visibility = Visibility.Hidden;
            ap = new AUTOPING(_ping, _pingPREPARE);
            AUTOPING.Event_Print += new Delegate_Print(add);
            ap.start();
            
            CreateTimer1(500);
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
                if (IPTVman.Model.data.ping_waiting > 20)
                {
                    textct.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        textct.Text = String.Format("{0} из {1}   ожидание {2} ", IPTVman.Model.data.ct_ping,
                                           IPTVman.Model.data.ping_all, IPTVman.Model.data.ping_waiting - 20);
                    }));

                }
            //----------------------------------------------
                if (start_add)
                {
                    string z = message_add;
                    textBox.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        size++;
                        if (size > 500) { textBox.Clear(); size = 0; }

                        textct.Text = String.Format("{0} из {1} ", Model.data.ct_ping,  Model.data.ping_all);

                        if (message_add == "end")
                        {
                            _writingProgressBar.Visibility = Visibility.Hidden;
                            message_add = "== АВТО ПИНГ ЗАКОНЧЕН==";
                            tb1.Text = "ВЫПОЛНЕНО";
                            button.Visibility = Visibility.Visible;
                        }
                        textBox.AppendText(message_add + Environment.NewLine);
                        textBox.ScrollToEnd();


                    }));

                    if (z == "end") update();

                    start_add = false;
                }


            }
            catch { }
        }

        bool start_add = false;
        string message_add;
        void add(string s)
        {
            message_add = s;
            start_add = true;
        }


        //key ЗАКРЫТЬ
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if  (_ping!=null) _ping.stop();
            this.Close();  
        }

        void update()
        {
            if (Event_Refresh != null) Event_Refresh(new Model.ParamCanal { });
            update_ok = true;
        }

        //close
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Trace.WriteLine("----------------- клосинг");
            if (ap != null) ap.stop();
            ap = null;
            LongtaskCANCELING.analiz_closing_thread(_ping, _pingPREPARE);
          
            if (!update_ok) update();

        }
    }
}
