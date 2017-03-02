using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using IPTVman.Helpers;
using IPTVman.Model;
using IPTVman.View;

namespace IPTVman.ViewModel
{
    class ViewModelWindow4 : ViewModelBase, IClosableViewModel
    {
        public event EventHandler CloseWindowEvent;

        public List<NEWParamCanal> Canal { get; set; }

        string _TextProperty1;
        public string TextProperty1
        {
            get
            {
                return _TextProperty1;
            }
            set
            {
                if (_TextProperty1 != value)
                {
                    _TextProperty1 = value;
                    RaisePropertyChanged("TextProperty1"); //The fix
                }
            }
        }

        public object SelectedParamCanal { get; set; }

        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand NextExampleCommand { get; set; }

        DispatcherTimer timer;

        public ViewModelWindow4()
        {
            Canal = new List<NEWParamCanal>
            {
                new NEWParamCanal { Title="Tom", ExtFilter="Jones", group_title=80 },
                new NEWParamCanal { Title="Dick", ExtFilter="Tracey", group_title=40 },
                new NEWParamCanal { Title="Harry", ExtFilter="Hill", group_title=60 },
            };
            TextProperty1 = "This will now update";
            NextExampleCommand = new RelayCommand(NextExample);
            key_ADDCommand = new RelayCommand(key_ADD);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void key_ADD(object parameter)
        {
            if (parameter == null) return;
            Canal.Add(new NEWParamCanal { Title = parameter.ToString(), ExtFilter = parameter.ToString(), group_title = DateTime.Now.Second });
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //This simulates something happening in the background
            //These changes are NOT reflected in the UI
            //For these changes to show, you need INotifyPropertyChanged or DependencyProperty
            TextProperty1 = DateTime.Now.ToString();
        }

        void NextExample(object parameter)
        {
            var win = new Window5 { DataContext = new ViewModelWindow5() };
            win.Show();

            if (CloseWindowEvent != null)
                CloseWindowEvent(this, null);
        }    
    }
}
