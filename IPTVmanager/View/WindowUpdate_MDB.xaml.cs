﻿using IPTVman.Model;
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
    /// Логика взаимодействия для Window
    /// </summary>
    public partial class WindowMDB : Window 
    {
        System.Timers.Timer Timer1;
        
        public WindowMDB()
        {
            InitializeComponent();
            //CreateTimer1(500);
            textBox.PreviewTextInput += new TextCompositionEventHandler(textBox_PreviewTextInput);
            textBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            textBox.Text = "";
            pa.Content = Model.bd_data.path;
            DataBaseWork.Event_Print += Access_Event_Print;
            TEXTmask.Text = Model.bd_data.mask;
            TEXT1.Text = Model.bd_data.filter1;
            TEXT2.Text = Model.bd_data.filter2;
        }

        private void Access_Event_Print(string obj)
        {
            update_block(obj);
        }

        static UInt16 ct=0;

        private void clear()
        {
            textBox.Dispatcher.Invoke(new Action(() =>
            {
                textBox.Text = "";
            }));
        }
        private void update_block(string text)
        {
            if (text == "") clear();
            ct++; //if (ct > 500) clear();
            textBox.Dispatcher.Invoke( new Action(() =>
            {
                textBox.Text += text;
                textBox.ScrollToEnd();
            }));

            pa.Dispatcher.Invoke( new Action(() =>
            {
                pa.Content = Model.bd_data.path;
            }));
   
        }

        public void CreateTimer1(int ms)
        {
            if (Timer1 == null)
            {
                Timer1 = new System.Timers.Timer();
                //Timer1.AutoReset = false; //
                Timer1.Interval = ms;
                Timer1.Elapsed += Timer1Tick;
                Timer1.Enabled = true;
                Timer1.Start();
            }
        }

        private void Timer1Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            
        }
        void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true; //if (!Char.IsDigit(e.Text, 0)) e.Handled = true;
        }
        //closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Model.loc.updateMDB = false;
            if (ViewModelWindowMDB._bd != null)
            {
                ViewModelWindowMDB._bd.Stop(); ;
                ViewModelWindowMDB._bd = null;
            }
            Model.bd_data.mask = TEXTmask.Text;
            Model.bd_data.filter1 = TEXT1.Text;
            Model.bd_data.filter2 = TEXT2.Text;
        }

        //key ЗАКРЫТЬ
        private void exit_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ButtonUPDATE(object sender, RoutedEventArgs e)
        {
            Model.bd_data.filter1 = ViewModelWindowMDB.sel1;
            Model.bd_data.filter2 = ViewModelWindowMDB.sel2;
            Model.bd_data.mask = ViewModelWindowMDB._mask;

            ser_data dt = new ser_data();
            dt.Prepare_to_save();
            Model.SETTING.SaveInXmlFormat(dt);
        }
    }
}
