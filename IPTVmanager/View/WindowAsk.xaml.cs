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
using System.Threading;

namespace IPTVman.ViewModel
{
    /// <summary>
    //
    /// </summary>
    public partial class WindowAsk : Window
    {
        public WindowAsk()
        {
            InitializeComponent();
            txtMessage.Text  = MessageAsk.message;
        }


        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void bn1_Click(object sender, RoutedEventArgs e)
        {
            MessageAsk.rez = 1;
            this.Close();
        }

        private void bn2_Click(object sender, RoutedEventArgs e)
        {
            MessageAsk.rez = 2;
            this.Close();
        }
    }

    public static class MessageAsk
    {
        public  static byte rez = 0;
        public static string message;
        static Window ask;
         public static MessageBoxResult Create(string s)
        {
            message = s;
            rez = 0;

            ask = new WindowAsk
            {
                Title = "",
                Topmost = true,
                //WindowStyle = WindowStyle.ThreeDBorderWindow,
                Name = "winask"
            };

            ask.ShowDialog();
            if (rez == 1) return MessageBoxResult.Yes;
            if (rez == 2) return MessageBoxResult.No;

            return MessageBoxResult.Cancel;

        }

       

    }

}
