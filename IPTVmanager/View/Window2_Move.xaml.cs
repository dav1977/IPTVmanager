using System.Windows;
using System.Windows.Threading;
using System;


namespace IPTVman.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для Window2_Move.xaml
    /// </summary>
    public partial class Window2 : Window
    {

        public static bool window_enabled = false;

        public Window2()
        {
          
            InitializeComponent();


            //use a timer to periodically update the memory usage
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();

        }



        private void timer_Tick(object sender, EventArgs e)
        {

            //this.button1.Content = "ВВЕРХ ";

        }


    

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            window_enabled = false;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
   
}
