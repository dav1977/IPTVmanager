using System.Windows;
using IPTVman.ViewModel;

namespace IPTVman.View
{
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            DataContext = new ViewModelWindow2();
        }
    }
}
