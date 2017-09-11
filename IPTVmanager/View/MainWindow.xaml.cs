using System.Windows;
using IPTVman.ViewModel;
using System.Windows.Data;
using System.Windows.Threading;
using IPTVman.Model;
using IPTVman.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace IPTVman.ViewModel
{
    public delegate void Delegate_UpdateEDIT( ParamCanal k);
    public delegate void Delegate_SelectITEM(int a, ParamCanal b);

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

            ViewModelMain.Event_UpdateLIST += new Action<int>(updateLIST);

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

            //this.KeyDown += new System.Windows.Input.KeyEventHandler(Window1_KeyDown);

        }
        //public void Window1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == System.Windows.Input.Key.Delete)
        //    {
                
        //    }
        //}
        private object threadLock = new object();
        void updateLIST(int type)
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

                            MYLIST.Dispatcher.Invoke( new Action(() =>
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


                if (WinPOP.need_to_close) WinPOP.Close();
                Wait.manager();
                dialog.manager();
                
            }
            catch (Exception ex) { MessageBox.Show("timer error "+ex.Message); }

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

            if (ViewModelMain.myLISTbase == null) return;
            if (ViewModelMain.myLISTbase.Count == 0) return;
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
            if (ViewModelMain.myLISTbase == null) return;
            if (ViewModelMain.myLISTbase.Count == 0) return;

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
                //WindowStyle = WindowStyle.ToolWindow,
                Name = "win2iptvMANAGER3"
            }.Show(); ;


        }
        private void Button_ClickRadio(object sender, RoutedEventArgs e)
        {
            if (ViewModelMain.myLISTbase == null) return;
            if (ViewModelMain.myLISTbase.Count == 0) return;

            foreach (Window win in Application.Current.Windows)
            {
                if (win.Name == "win2radio")
                {
                    return;
                }
            }
            IPTVman.Model.data.mode_radio_from_select = false;
            new ListViewDragDropManager.WindowRadio
            {
                //DataContext = new ViewModelWindow2(tb1.Text),
                Title = "Интернет РАДИО",
                Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                Name = "win2radio"
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
           SETTING.SaveInXmlFormat();
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

        //add new item
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            //MYLIST.Dispatcher.Invoke(new Action(() =>
            //{
            //    //MYLIST.Items.MoveCurrentToLast();
            //    // ScrollToLastItem();
            //}));
        }

        public void ScrollToLastItem()
        {
            if (MYLIST.Items.Count < 3) return;
            MYLIST.SelectedItem = MYLIST.Items.GetItemAt(MYLIST.Items.Count-2);

            MYLIST.Items.MoveCurrentToLast();

            ListViewItem item = MYLIST.ItemContainerGenerator.ContainerFromItem(MYLIST.SelectedItem) as ListViewItem;

            item.Focus();
        }

        private void text_title_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
