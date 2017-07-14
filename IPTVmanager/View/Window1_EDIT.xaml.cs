using System.Windows;
using System.Windows.Threading;
using System;


namespace IPTVman.ViewModel
{
    public partial class Window1 : Window
    {
        PING _ping;
        PING_prepare _pingPREPARE;

        public Window1()
        {

            InitializeComponent();
            //Configure the ProgressBar
            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = 255;
            ProgressBar1.Value = 0;
            textBoxPING.Text="";
            textBoxPING2.Text = "";


            _ping = new PING();
            _ping.result = "";
            _pingPREPARE = new PING_prepare(_ping);

            //use a timer to periodically update the memory usage
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();
        }


        private void timer_Tick(object sender, EventArgs e)
        {

            if (_ping == null) return;

            if (ViewModelWindow1.edit.ping!="")
            {
                textBoxPING.Text = _ping.result;
                ProgressBar1.Value = 0;
            }
            else ProgressBar1.Value += 3;
           

            //УСТАНОВКА ССЫЛКИ В ПОДЧИНЕННОМ ЭКЗЕМПЛЯРЕ
            if (ViewModelWindow1._ping == null)
            {
                ViewModelWindow1._ping = _ping;
                ViewModelWindow1._pingPREPARE = _pingPREPARE;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _ping.stop();
            _ping = null;
        }

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            IPTVman.Model.play.URLPLAY = urlTEXT.Text;
            IPTVman.Model.play.playerUPDATE = true;
            this.Close();
        }

        private void ButtonBEST_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exit_Copy_Click(object sender, RoutedEventArgs e)
        {
          
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
