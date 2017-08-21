using System.Windows;
using IPTVman.ViewModel;
using System.Windows.Data;
using System.Windows.Threading;
using IPTVman.Model;
using IPTVman.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace IPTVman.ViewModel
{

    public delegate void Delegate_UpdateFILTER();
    public delegate void Delegate_UpdateALL(int size);
    public delegate void Delegate_UpdateEDIT( ParamCanal k);
    public delegate void Delegate_Window1();
    public delegate void Delegate_ADDBEST();
    public delegate void Delegate_SelectITEM(int a, ParamCanal b);
    public delegate void Delegate_WIN_WAIT(byte n);
    public delegate void Message(string mess);
  
    public partial class MainWindow : Window
    {
        public static Window header;
        bool start_update = false;

        public MainWindow()
        {
            header = this;
            InitializeComponent();
            this.Title = "IPTV manager v1.0";

            SETTING.ReadFromXML();

            ViewModelMain.Event_UpdateLIST += new Delegate_UpdateALL(updateLIST);
  
           // use a timer to periodically update the memory usage
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;
            timer.Start();

            changefav = true;
            data.canal.name = "";
            best1.Text = data.favorite1_1;
            best2.Text = data.favorite1_2;
            data.current_favorites = 1;
            data.best1 = best1.Text;
            data.best2 = best2.Text;
            changefav = false;

            //MYLIST.Loaded += (sender, args) => { IPTVman.ViewModel.MainWindow.u = MYLIST.Dispatcher; };

            //Binding bind = new Binding();
            //bind.Source = grid1;
            //bind.Path = new PropertyPath("grid1.ItemsSource.Count");  
            //bind.Mode = BindingMode.OneWay;
            //label_kanals.SetBinding(label_kanals.Content, bind);
        }


        private object threadLock = new object();
        void updateLIST(int size)
        {
            start_update = true;    
        }

       // void select (int a, ParamCanal b)
       // {
       //     lock (threadLock)
       //     {
       //         MYLIST.SelectedIndex = 10;
       //         MYLIST.Focusable = true;
       //         MYLIST.Focus();
       //     }
       //}

       
        static Window message;
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {

                if (start_update)
                {

                    lock (threadLock)
                    {
                        //object lockObj = new object();
                        //BindingOperations.EnableCollectionSynchronization(, lockObj);
                        try
                        {

                            MYLIST.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                           {
                               bDELETE.Content = "";
                               MYLIST.Items.Refresh();

                           }));
                        }
                        catch
                        {

                        }
                    }

                    start_update = false;
                }



                if (dialog.message_open)
                {
                    dialog.message_open = false;

                    if (message != null)
                    {
                        message.Close();
                        message = null;
                    }
                    message = new WindowMessage()
                    {
                        Title = "Сообщение",
                        Topmost = true,
                        WindowStyle = WindowStyle.ToolWindow,
                        Name = "message",
                        SizeToContent = SizeToContent.WidthAndHeight,
                        ResizeMode = ResizeMode.NoResize
                        // WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };

                    message.Closing += message_Closing;
                    message.Show();
                    message.Owner = MainWindow.header;
                }

                Wait.manager();

            }
            catch { }

        }
        private void message_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            message = null;
        }


        private void tb1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }



        private void MYLIST_LostTouchCapture(object sender, System.Windows.Input.TouchEventArgs e)
        {


        }

        private void MYLIST_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int si = MYLIST.SelectedIndex;
            if (si < 0) { return; }
            var p = MYLIST.SelectedItem as ParamCanal;
            if (p == null) return;

            data.canal = new ParamCanal()
            {
                name = p.name,
                ExtFilter = p.ExtFilter,
                group_title = p.group_title,
                tvg_name = p.tvg_name,
                http = p.http,
                logo = p.logo,
                ping = p.ping

            };

            data.best1 = best1.Text;
            data.best2 = best2.Text;
            if (data.canal.name == "") bDELETE.Content = "не выбрано"; else 
            bDELETE.Content = "УДАЛИТЬ " + data.canal.name;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            data.f1 = Ffilter1.Text;
            data.f2 = Ffilter2.Text;
            data.f3 = Ffilter3.Text;
            data.f4 = Ffilter4.Text;
            data.best1 = best1.Text;
            data.best2 = best2.Text;
        }


        Window win2;
        private void Button_ClickMOVE(object sender, RoutedEventArgs e)
        {

            if (IPTVman.ViewModel.ViewModelMain.myLISTbase == null) return;
            if (IPTVman.ViewModel.ViewModelMain.myLISTbase.Count == 0) return;
            if (win2 != null) return;

            win2 = new Window2
            {
                DataContext = new ViewModelWindow2(tb1.Text),
                Topmost = true,
                WindowStyle = WindowStyle.ToolWindow,
                Name = "win2iptvMANAGER"
            };

            win2.Closing += Win2_Closing;
            win2.Show();
        }

        private void Win2_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            win2 = null;
        }

        private void Button_ClickMOVEDrag(object sender, RoutedEventArgs e)
        {
            if (IPTVman.ViewModel.ViewModelMain.myLISTbase == null) return;
            if (IPTVman.ViewModel.ViewModelMain.myLISTbase.Count == 0) return;

            foreach (Window win in Application.Current.Windows)
            {
                if (win.Name == "win2iptvMANAGER3")
                {
                    return;
                }
            }
            new ListViewDragDropManager.WindowMOVE
            {
                //DataContext = new ViewModelWindow2(tb1.Text),
                Title = "ПЕРЕМЕЩЕНИЕ",
                Topmost = true,
                WindowStyle = WindowStyle.ToolWindow,
                Name = "win2iptvMANAGER3"
            }.Show(); ;


        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            data.best1 = best1.Text;
            data.best2 = best2.Text;
        }

        bool changefav = false;
        private void button_Click_5(object sender, RoutedEventArgs e)
        {
            changefav = true;
            best1.Text = data.favorite1_1;
            best2.Text = data.favorite1_2;
            data.current_favorites = 1;
            changefav = false;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            changefav = true;
            best1.Text = data.favorite2_1;
            best2.Text = data.favorite2_2;
            data.current_favorites = 2;
            changefav = false;
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            changefav = true;
            best1.Text = data.favorite3_1;
            best2.Text = data.favorite3_2;
            data.current_favorites = 3;
            changefav = false;
        }

        void update_favorites(string s1, string s2)
        {
            if (changefav) return;
            if (data.current_favorites == 1)
            {
                data.favorite1_1 = s1;
                data.favorite1_2 = s2;
            }
            if (data.current_favorites == 2)
            {
                data.favorite2_1 = s1;
                data.favorite2_2 = s2;
            }
            if (data.current_favorites == 3)
            {
                data.favorite3_1 = s1;
                data.favorite3_2 = s2;
            }
        }

        private void best2_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            update_favorites(best1.Text, best2.Text);
        }

        private void best1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            update_favorites(best1.Text, best2.Text);
        }

     

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    var a = MYLIST.Items[2];
        //    MYLIST.SelectedItem = a;
        //    MYLIST.Items.Refresh();
        //    MYLIST.Focus();
        //}


        Window win1;
        private void MYLIST_MouseDoubleClick_EDIT(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LongtaskPingCANCELING.isENABLE()) return;
            if (win1 != null) return;
            int si = MYLIST.SelectedIndex;
            if (si < 0) { return; }
            var p = MYLIST.SelectedItem as ParamCanal;
            if (p == null) return;
            data.canal = p;

            win1 = new Window1
            {
                Title = "РЕДАКТИРОВАНИЕ",
                DataContext = new ViewModelWindow1(tb1.Text),
                Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                Name = "win1iptvMANAGER"
            };

            win1.Closing += Win1_Closing;
            win1.Show();
        }

        private void Win1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            win1 = null;
        }

        //mdb
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Model.SETTING.SaveInXmlFormat();
        }

        Window about;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (LongtaskPingCANCELING.isENABLE()) return;
            if (about != null) return;
            about = new WindowAbout
            {
                Title = "",
                Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                Name = "winABOUT"
            };

            about.Closing += WinABOUT_Closing;
            about.Show();
        }
        private void WinABOUT_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            about = null;
        }

        private void sizeinc_Click(object sender, RoutedEventArgs e)
        {
            MYLIST.FontSize += 1;
            if (MYLIST.FontSize >56) MYLIST.FontSize = 56;
        }

        private void sizedec_Click(object sender, RoutedEventArgs e)
        {
            MYLIST.FontSize -= 1;
            if (MYLIST.FontSize < 8) MYLIST.FontSize = 8;
        }
    }
}
