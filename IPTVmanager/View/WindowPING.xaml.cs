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

  
    public delegate void Delegate_Print(string s);
    /// <summary>
    /// Логика взаимодействия для WindowWAIT.xaml
    /// </summary>
    public partial class WindowPING : Window 
    {
        public static event Delegate_UpdateEDIT Event_updateFILTER;
        public static event Delegate_UpdateALL Event_Refresh;
        static AUTOPING ap;
        public WindowPING()
        {
  
            ap = new AUTOPING();
            AUTOPING.Event_Print += new Delegate_Print(add);
            InitializeComponent();
            button.Visibility = Visibility.Hidden;
            ap.start();
        }

        int size = 0;
        void add(string s)
        {
            string e = s;
        textBox.Dispatcher.Invoke(DispatcherPriority.Background, new
        Action(() =>
        {
            size++;
            if (size > 100) { textBox.Clear(); size = 0; }

            if (s == "end")
            {
                _writingProgressBar.Visibility = Visibility.Hidden;
                s = "== АВТО ПИНГ ЗАКОНЧЕН==";
                tb1.Text = "ВЫПОЛНЕНО";
                button.Visibility = Visibility.Visible;
            }
            textBox.AppendText(s + Environment.NewLine);
            textBox.ScrollToEnd();
           
           
        }));


            if (e == "end")
            {
                if (Event_updateFILTER != null) Event_updateFILTER(new Model.ParamCanal { });
                if (Event_Refresh != null) Event_Refresh(1);

            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
