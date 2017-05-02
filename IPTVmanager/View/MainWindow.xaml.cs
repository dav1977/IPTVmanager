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
        public static Vlc.DotNet.Player player = null;

        public MainWindow()
        {
            InitializeComponent();
            this.Title = "IPTV manager v1.0";

            ViewModelMain.Event_UpdateLIST += new Delegate_UpdateALL(updateLIST);

            ViewModelMain.Event_SelectITEM += new Delegate_SelectITEM(select);

            ViewModelMain.Event_WIN_WAIT += new Delegate_WIN_WAIT(WIN_WAIT);
            // use a timer to periodically update the memory usage
            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = new TimeSpan(0, 0,0,0 ,50);
            //timer.Tick += timer_Tick;
            //timer.Start();

            

            data.edit.name = "";
            best1.Text = "best";
            best2.Text = "best";
            data.best1 = best1.Text;
            data.best2 = best2.Text;



            //Binding bind = new Binding();
            //bind.Source = grid1;
            //bind.Path = new PropertyPath("grid1.ItemsSource.Count");  
            //bind.Mode = BindingMode.OneWay;
            //label_kanals.SetBinding(label_kanals.Content, bind);
        }

        void WIN_WAIT(byte num)
        {
           //if (num == 1) OPEN_WIN_LOADING();
           // if (num == 2) CLOSE_WIN_LOADING();
        }
        void OPEN_WIN_LOADING()
        {      
             new WindowLOADING
            {
                //DataContext = new V(tb1.Text),
                //Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                //Name = "winLOADINGiptv"
            }.Show(); ;

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

            MYLIST.SelectedIndex = sel;
           
            MYLIST.Focus();
        }

        void select (int a, ParamCanal b)
        {

            MYLIST.ScrollIntoView(b);

            sel = a;

          MYLIST.SelectedIndex = a;


           
            MYLIST.Focusable = true;
            //MYLIST.SelectedItem =b ;



            //  MessageBox.Show(MYLIST.SelectedItem.ToString());
         //MessageBox.Show(a.ToString());

            MYLIST.Focus();

        }

        //static uint i=0;
        //bool last = false;
        private void timer_Tick(object sender, EventArgs e)
        {
            //if (last == ViewModelMain.loading_file && !last) return;
            //last = ViewModelMain.loading_file;
            //if (ViewModelMain.loading_file)
            //{
            //    {
 
            //        if (i == 0) this.Title = "Opening ... ";
            //        if (i < 5) this.Title = "Opening .   ";
            //        if (i > 5 && i < 10) this.Title = "Opening . .   ";
            //        if (i > 10 && i < 15) this.Title = "Opening . . .   ";
            //        if (i > 15 && i < 20) this.Title = "Opening . . . . . . .   ";
            //        i++;
            //        if (i > 20) i = 1;
                    
            //    }
            //}
            //else this.Title= "IPTV manager v1.0";

          
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void tb1_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
  

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            

        }

        private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged_1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged_2(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
     
        }

        private void MYLIST_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
       
        }

        private void MYLIST_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }


        private void Button_Click_EDIT(object sender, RoutedEventArgs e)
        {
      
        }

        private void Button_Click_BEST(object sender, RoutedEventArgs e)
        {

            data.best1 = best1.Text;
            data.best2 = best2.Text;
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

            data.edit = p;

            new Window1
            {
                DataContext = new ViewModelWindow1(tb1.Text),
                Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                Name = "win1iptvMANAGER"
            }.Show(); ;


        }

        private void MYLIST_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            //правая кнопка мыши
        }

        private void MYLIST_MouseEnter_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //наведение на поле
        }

        private void MYLIST_TouchEnter(object sender, System.Windows.Input.TouchEventArgs e)
        {
            
        }

        private void MYLIST_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
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

            data.edit = p;
            data.best1 = best1.Text;
            data.best2 = best2.Text;
            if (data.edit.name == "") bDELETE.Content = "не выбрано"; else 
            bDELETE.Content = "УДАЛИТЬ " + data.edit.name;

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
        private void Ffilter4_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            data.best1 = best1.Text;
            data.best2 = best2.Text;
        }
    }
}
