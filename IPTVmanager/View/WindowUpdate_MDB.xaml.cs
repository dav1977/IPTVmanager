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
   
    /// <summary>
    /// Логика взаимодействия для Window
    /// </summary>
    public partial class WindowMDB : Window 
    {
   
       
        System.Timers.Timer Timer1;
        
        public WindowMDB()
        {
            InitializeComponent();
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

        }

      


        //closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ViewModelWindowMDB._bd = null;

        }

        //key ЗАКРЫТЬ
        private void exit_Copy_Click(object sender, RoutedEventArgs e)
        {
            if (this != null) this.Close();
        }
    }
}
