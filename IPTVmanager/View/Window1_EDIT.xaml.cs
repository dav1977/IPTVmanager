using System.Windows;
using System.Windows.Threading;
using System;


namespace IPTVman.ViewModel
{
    public partial class Window1 : Window
    {


        public Window1()
        {

            InitializeComponent();
            //Configure the ProgressBar
            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = 255;
            ProgressBar1.Value = 0;
            textBoxPING.Text="";
            textBoxPING2.Text = "";

           if (ViewModelBase._ping==null) ViewModelBase._ping = new PING();

            ViewModelBase._ping.result77 = "";

            //use a timer to periodically update the memory usage
           DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();
        }


        private void timer_Tick(object sender, EventArgs e)
        {

            if (ViewModelBase._ping.result77 != "")
            {
                textBoxPING.Text = ViewModelBase._ping.result77;

            }
            else ProgressBar1.Value++; 

           
        }


    

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
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
