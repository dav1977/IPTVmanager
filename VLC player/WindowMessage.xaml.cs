using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace IPTVman.ViewModel
{
    /// <summary>
    //
    /// </summary>
    public partial class WindowMessage : Window
    {
        
        public WindowMessage()
        {
            InitializeComponent();
            txtMessage.Text = dialog.get_current_message();

            if (dialog.get_current_message().Length > 120)
            {
                MessageBox.Show(dialog.get_current_message()); this.Close();
            }

            this.KeyDown += new System.Windows.Input.KeyEventHandler(Window1_KeyDown);

            //use a timer to periodically update the memory usage
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += Timer_Tick; ;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            txtMessage.Text = dialog.get_current_message();
        }

        void Window1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)this.Close();
        }
        private void exit_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }

    public static class dialog
    {
        static string message;
        public static bool dialog_enable=false;
        public static string get_current_message()
        {
            return message;
        }

        public static void Show(string s)
        {
            message = s;
            dialog_enable = true;
        }

    }
    
}
