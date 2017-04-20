using System.Windows;
using System.Windows.Threading;
using System;


namespace IPTVman.ViewModel
{
    public partial class Window1 : Window
    {


        public Window1()
        {
            ViewModelWindow1.Event_CloseWin1 += new Delegate_Window1(CloseW);
            InitializeComponent();
            //Configure the ProgressBar
            ProgressBar1.Minimum = 0;
            ProgressBar1.Maximum = 255;
            ProgressBar1.Value = 0;
            textBoxPING.Text="";
            textBoxPING2.Text = "";


            ViewModelBase.result77 = "";

            //use a timer to periodically update the memory usage
           DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();
        }


        private void timer_Tick(object sender, EventArgs e)
        {

            if (ViewModelBase.result77 != "")
            {
                textBoxPING.Text = ViewModelBase.result77;

            }
            else ProgressBar1.Value++; 

           
        }


        void CloseW()
        {

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

       

        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {

            IPTVman.Model.data.URLPLAY = urlTEXT.Text;
            IPTVman.Model.data.playerUPDATE = true;

            if (MainWindow.player == null)
            {
                MainWindow.player = new Vlc.DotNet.Player { DataContext = new ViewModelWindow1("") };
                MainWindow.player.Show();
            }
        }
    }
}
