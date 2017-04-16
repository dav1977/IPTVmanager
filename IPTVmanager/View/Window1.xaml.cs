using System.Windows;

namespace IPTVman.ViewModel
{
    public partial class Window1 : Window
    {


        public Window1()
        {
            ViewModelWindow1.Event_CloseWin1 += new Delegate_Window1(CloseW);
            InitializeComponent();
           
        }

        void CloseW()
        {

            this.Close();
        }
    }
}
