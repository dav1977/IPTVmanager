using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace IPTVman.ViewModel
{
    class ViewModelBase : INotifyPropertyChanged
    {
        System.Timers.Timer Timer1;

        public void CreateTimer1(int ms)
        {
            if (Timer1 == null)
            {
                Timer1 = new System.Timers.Timer();
                Timer1.AutoReset = false; // Чтобы операции удаления не перекрывались
                Timer1.Interval = ms;
                Timer1.Elapsed += Timer1Tick;
                Timer1.Enabled = true;
                Timer1.Start();
            }
        }

        private void Timer1Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("numberCANALS"));
                PropertyChanged(this, new PropertyChangedEventArgs("memory"));
            }
        }


        //basic ViewModelBase
        internal void RaisePropertyChanged(string prop)
        {
           if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
               
            }
         

        }
       public  event PropertyChangedEventHandler PropertyChanged; //событие выбора канала





        //Extra Stuff, shows why a base ViewModel is useful
        bool? _CloseWindowFlag;
        public bool? CloseWindowFlag
        {
            get { return _CloseWindowFlag; }
            set
            {
                _CloseWindowFlag = value;
                RaisePropertyChanged("CloseWindowFlag");
            }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null 
                    ? true 
                    : !CloseWindowFlag;
            }));
        }
    }
}
