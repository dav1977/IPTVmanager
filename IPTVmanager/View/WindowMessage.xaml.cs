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
            txtMessage.Text = dialog.message;
            if (dialog.message.Length > 120) txtMessage.FontSize = 8;
        }

        private void exit_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }

    public static class dialog
    {
       public static string message;
       public static bool message_open = false;
        public static void Show(string s)
        {
            message = s;
           message_open = true;
        }

    }
    
}
