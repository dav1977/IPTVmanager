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

        public List<NEWperson> People { get; set; }

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

        public object SelectedPerson { get; set; }

        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand NextExampleCommand { get; set; }

        DispatcherTimer timer;

        public ViewModelWindow4()
        {
            People = new List<NEWperson>
            {
                new NEWperson { FirstName="Tom", LastName="Jones", Age=80 },
                new NEWperson { FirstName="Dick", LastName="Tracey", Age=40 },
                new NEWperson { FirstName="Harry", LastName="Hill", Age=60 },
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
            People.Add(new NEWperson { FirstName = parameter.ToString(), LastName = parameter.ToString(), Age = DateTime.Now.Second });
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
