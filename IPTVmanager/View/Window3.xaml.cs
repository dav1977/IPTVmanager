using System.Windows;
using IPTVman.Model;
using IPTVman.ViewModel;

namespace IPTVman.View
{
    public partial class Window3 : Window
    {
        public Window3(ParamCanal ParamCanal)
        {
            InitializeComponent();
            var vm = new ViewModelWindow3(ParamCanal);
            DataContext = vm;
            vm.CloseWindowEvent += new System.EventHandler(vm_CloseWindowEvent);
        }

        void vm_CloseWindowEvent(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
