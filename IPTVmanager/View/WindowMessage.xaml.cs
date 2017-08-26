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
    //
    /// </summary>
    public partial class WindowMessage : Window
    {
        
        public WindowMessage()
        {
            InitializeComponent();
            txtMessage.Text = dialog.get_current_message();
            if (dialog.get_current_message().Length > 120)
            { MessageBox.Show(dialog.get_current_message()); this.Close(); }
        }

        private void exit_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }

    public static class dialog
    {
        static string message;
        public static bool dialog_enable;
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
