using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using IPTVman.Model;
using IPTVman.Helpers;
using IPTVman.View;
using IPTVman.Data;

namespace IPTVman.ViewModel
{
    class ViewModelWindow2 : DependencyObject
    {
        //just type propdp in VisualStudio, below this line, then press Tab to get the DependencyProperty snippet

        public ParamCanal SelectedParamCanal
        {
            get { return (ParamCanal)GetValue(SelectedParamCanalProperty); }
            set { SetValue(SelectedParamCanalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedParamCanal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedParamCanalProperty =
            DependencyProperty.Register("SelectedParamCanal", typeof(ParamCanal), typeof(ViewModelWindow2), new UIPropertyMetadata(null));

        public ObservableCollection<ParamCanal> Canal
        {
            get { return (ObservableCollection<ParamCanal>)GetValue(CanalProperty); }
            set { SetValue(CanalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Canal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanalProperty =
            DependencyProperty.Register("Canal", typeof(ObservableCollection<ParamCanal>), typeof(ViewModelWindow2), new UIPropertyMetadata(null));

        public bool? CloseWindowFlag
        {
            get { return (bool?)GetValue(CloseWindowFlagProperty); }
            set { SetValue(CloseWindowFlagProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseWindowFlag.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseWindowFlagProperty = 
            DependencyProperty.Register("CloseWindowFlag", typeof(bool?), typeof(ViewModelWindow2), new UIPropertyMetadata(null));

        public RelayCommand NextExampleCommand { get; set; }

        public ViewModelWindow2()
        {
            Canal = FakeDatabaseLayer.GetCanalFromDatabase();
            NextExampleCommand = new RelayCommand(NextExample, NextExample_CanExecute);
        }

        bool NextExample_CanExecute(object parameter)
        {
            return SelectedParamCanal != null;
        }

        void NextExample(object parameter)
        {
            var win = new Window3(SelectedParamCanal);
            win.Show();
            CloseWindowFlag = true;
        }
    }
}
