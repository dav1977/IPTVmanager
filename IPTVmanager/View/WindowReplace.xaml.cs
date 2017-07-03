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
    ///
    /// </summary>
    public partial class WindowReplace : Window 
    {
        public static event Delegate_UpdateEDIT Event_updateFILTER;
        public static event Delegate_UpdateALL Event_Refresh;
   

        public WindowReplace()
        {
            InitializeComponent(); 
         
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
            if (Event_updateFILTER != null) Event_updateFILTER(new Model.ParamCanal { });
            if (Event_Refresh != null) Event_Refresh(1);
        }

        private void exit_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
