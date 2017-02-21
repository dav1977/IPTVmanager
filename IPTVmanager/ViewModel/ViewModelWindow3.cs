using System;
using System.Windows.Threading;
using System.Collections.Generic;
using MvvmExample.Model;
using MvvmExample.Helpers;
using MvvmExample.View;

namespace MvvmExample.ViewModel
{
    class ViewModelWindow3
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
                }
            }
        }

        public object SelectedPerson { get; set; }

        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand NextExampleCommand { get; set; }

        DispatcherTimer timer;

        public ViewModelWindow3(Person person)
        {
            People = new List<NEWperson>
            {
                new NEWperson{ FirstName=person.FirstName, LastName=person.LastName, Age=person.Age },
                new NEWperson{ FirstName="Grace", LastName="Jones", Age=21 },
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
            People.Add(new NEWperson { FirstName = parameter.ToString(), LastName = parameter.ToString(), Age = DateTime.Now.Second });
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
