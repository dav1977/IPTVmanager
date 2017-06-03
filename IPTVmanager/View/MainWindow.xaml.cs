using System.Windows;
using IPTVman.ViewModel;
using System.Windows.Data;
using System.Windows.Threading;
using IPTVman.Model;
using IPTVman.Helpers;
using System;
using System.Collections.Generic;


namespace IPTVman.ViewModel
{
   
    public delegate void Delegate_UpdateALL(int size);
    public delegate void Delegate_UpdateEDIT();
    public delegate void Delegate_Window1();
    public delegate void Delegate_ADDBEST();
    public delegate void Delegate_SelectITEM(int a, ParamCanal b);
    public delegate void Delegate_WIN_WAIT(byte n);

    public partial class MainWindow : Window
    {
  
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "IPTV manager v1.0";

            ViewModelMain.Event_UpdateLIST += new Delegate_UpdateALL(updateLIST);
            ViewModelMain.Refresh += new Delegate_UpdateALL(updateLIST);
            // ViewModelMain.Event_SelectITEM += new Delegate_SelectITEM(select);

            ViewModelMain.Event_WIN_WAIT += new Delegate_WIN_WAIT(WIN_WAIT);


            ViewModelWindow2.Event_SelectITEM += new Delegate_SelectITEM(select);
           // use a timer to periodically update the memory usage
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);
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


            //Binding bind = new Binding();
            //bind.Source = grid1;
            //bind.Path = new PropertyPath("grid1.ItemsSource.Count");  
            //bind.Mode = BindingMode.OneWay;
            //label_kanals.SetBinding(label_kanals.Content, bind);
        }

        void WIN_WAIT(byte num)
        {
            new WindowWAIT
            {
                //DataContext = new V(tb1.Text),
                //Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                //Name = "winLOADINGiptv"
            }.Show();
        }
     

        void CLOSE_WIN_LOADING()
        {
            foreach (Window win in Application.Current.Windows)
            {
                if  (win.Name == "winLOADINGiptv")
                {
                    win.Close();
                }
            }

        }


        int sel = 0;
       
        void updateLIST(int size)
        {
            bDELETE.Content = "";
            //MYLIST.Items.Refresh();
            bDELETE.Content = "";

            MYLIST.Items.Refresh();

         //   MYLIST.SelectedIndex = sel;
           
         //   MYLIST.Focus();
        }

        void select (int a, ParamCanal b)
        {

            

        //    MYLIST.ScrollIntoView(b);

        //    sel = a;

         MYLIST.SelectedIndex = 10;


           
            MYLIST.Focusable = true;
           // MYLIST.SelectedItem =b ;



            //  MessageBox.Show(MYLIST.SelectedItem.ToString());
         //MessageBox.Show(a.ToString());

            MYLIST.Focus();

        }



        bool win_open = false;
        private void timer_Tick(object sender, EventArgs e)
        {

            if (data.waiting)
            {
                if (win_open) return;
                new WindowWAIT
                {
                    //DataContext = new V(tb1.Text),
                    Topmost = true,
                    //WindowStyle = WindowStyle.ToolWindow,
                    Name = "winwait"
                }.Show();
                win_open = true;

            }
            else
            {
                if (!win_open) return;
                else
                {
                    foreach (Window win in Application.Current.Windows)
                    {
                        if (win.Name == "winwait")
                        {
                            win.Close();
                            win_open = false;
                        }
                    }
                }
            }

        }

        private void tb1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    
        private void MYLIST_MouseDoubleClick_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            foreach (Window win in Application.Current.Windows)
            {
                if ((win.IsLoaded == true) && (win.Name == "win1iptvMANAGER"))
                {
                    return;
                }
            }

            int si = MYLIST.SelectedIndex;
            if (si < 0) {  return; }
            var p = MYLIST.SelectedItem as ParamCanal;
            if (p == null) return;

            data.canal = p;

            new Window1
            {
                DataContext = new ViewModelWindow1(tb1.Text),
                Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                Name = "win1iptvMANAGER"
            }.Show();

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


        private void Button_ClickMOVE(object sender, RoutedEventArgs e)
        {

            if (IPTVman.ViewModel.ViewModelMain.myLISTbase == null) return;
            if (IPTVman.ViewModel.ViewModelMain.myLISTbase.Count == 0) return;

            foreach (Window win in Application.Current.Windows)
            {
                if ((win.IsLoaded == true) && (win.Name == "win2iptvMANAGER"))
                {
                    return;
                }
            }

            new Window2
            {
                DataContext = new ViewModelWindow2(tb1.Text),
                Topmost = true,
                WindowStyle = WindowStyle.ToolWindow,
                Name ="win2iptvMANAGER"
            }.Show(); ;
           
         
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

     

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //    MYLIST.ScrollIntoView(b);

            //    sel = a;

            //   MYLIST.SelectedIndex = 3;


            var a = MYLIST.Items[2];
            //MYLIST.Focusable = true;
            MYLIST.SelectedItem = a;

           MYLIST.Items.Refresh();

            //  MessageBox.Show(MYLIST.SelectedItem.ToString());
            //MessageBox.Show(a.ToString());

            MYLIST.Focus();
        }
    }
}
