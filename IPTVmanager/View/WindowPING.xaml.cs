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


namespace IPTVman.ViewModel
{
    public delegate void Delegate_Print(string s);
    /// <summary>
    /// Логика взаимодействия для Window
    /// </summary>
    public partial class WindowPING : Window 
    {
        public static event Delegate_UpdateEDIT Event_updateFILTER;
        public static event Delegate_UpdateALL Event_Refresh;
        public static event Delegate_CLOSEPING Event_Close_ping;

        int size = 0;
        static AUTOPING ap;
        bool update_ok=false;
        System.Timers.Timer Timer1;
        
        public WindowPING()
        {
            InitializeComponent();
            button.Visibility = Visibility.Hidden;
            ap = new AUTOPING();
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

            if (IPTVman.Model.data.ping_waiting > 20)
            {
                   textct.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                   {
                       textct.Text = String.Format("{0} из {1}   ожидание {2} ", IPTVman.Model.data.ct_ping,
                                          IPTVman.Model.data.ping_all, IPTVman.Model.data.ping_waiting-20);
                   }));

            }
        }

        
        void add(string s)
        {
            string e = s;
            textBox.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                size++;
                if (size > 500) { textBox.Clear(); size = 0; }

                textct.Text = String.Format("{0} из {1} ", IPTVman.Model.data.ct_ping, IPTVman.Model.data.ping_all);

                if (s == "end")
                {
                    _writingProgressBar.Visibility = Visibility.Hidden;
                    s = "== АВТО ПИНГ ЗАКОНЧЕН==";
                    tb1.Text = "ВЫПОЛНЕНО";
                    button.Visibility = Visibility.Visible;
                }
                textBox.AppendText(s + Environment.NewLine);
                textBox.ScrollToEnd();
           
           
            }));


            if (e == "end")
            {
                if (Event_updateFILTER != null) Event_updateFILTER(new Model.ParamCanal { });
                if (Event_Refresh != null) Event_Refresh(1);
                update_ok = true;
            }

        }


        //key ЗАКРЫТЬ
        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (Event_Close_ping != null) Event_Close_ping("");
            if  (ViewModelBase._ping!=null) ViewModelBase._ping.stop();
            if (this!=null) this.Close();
            
        }



        //close
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!update_ok)
            {
                if (Event_updateFILTER != null) Event_updateFILTER(new Model.ParamCanal { });
                if (Event_Refresh != null) Event_Refresh(1);
                update_ok = true;
            }

            if (ViewModelBase._ping != null) ViewModelBase._ping.stop();
            if (ap!=null) ap.stop();
            ap = null;
        }
    }
}
