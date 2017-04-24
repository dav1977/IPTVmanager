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
        }



        private void timer_Tick(object sender, EventArgs e)
        {



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
