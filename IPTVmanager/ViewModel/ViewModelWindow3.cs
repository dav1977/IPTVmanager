using System;
using System.Windows.Threading;
using System.Collections.Generic;
using IPTVman.Model;
using IPTVman.Helpers;
using IPTVman.View;

namespace IPTVman.ViewModel
{
    class ViewModelWindow3
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
                }
            }
        }

        public object SelectedParamCanal { get; set; }

        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand NextExampleCommand { get; set; }

        DispatcherTimer timer;

        public ViewModelWindow3(ParamCanal ParamCanal)
        {
            Canal = new List<NEWParamCanal>
            {
                new NEWParamCanal{ Title=ParamCanal.Title, ExtFilter=ParamCanal.ExtFilter, group_title=ParamCanal.group_title },
                new NEWParamCanal{ Title="Grace", ExtFilter="Jones", group_title=21 },
            };
            TextProperty1 = "Only this TextBox's changes are reflected in bindings";
            NextExampleCommand = new RelayCommand(NextExample);
            key_ADDCommand = new RelayCommand(key_ADD);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //This simulates something happening in the background
            //These changes are NOT reflected in the UI
            //For these changes to show, you need INotifyPropertyChanged or DependencyProperty
            TextProperty1 = DateTime.Now.ToString();
        }

        void key_ADD(object parameter)
        {
            if (parameter == null) return;
            Canal.Add(new NEWParamCanal { Title = parameter.ToString(), ExtFilter = parameter.ToString(), group_title = DateTime.Now.Second });
        }

        void NextExample(object parameter)
        {
            var win = new Window4 { DataContext = new ViewModelWindow4() };
            win.Show();

            if (CloseWindowEvent != null)
                CloseWindowEvent(this, null);
        }

    }
}
