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
    public delegate void Delegate_UpdateEDIT(int size);
    public delegate void Delegate_Window1();
    public delegate void Delegate_ADDBEST();

    public partial class MainWindow : Window
    {
        public static Vlc.DotNet.Player player = null;

        public MainWindow()
        {
            InitializeComponent();

            ViewModelMain.Event_UpdateLIST += new Delegate_UpdateALL(updateLIST);
      


            // use a timer to periodically update the memory usage
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;
            timer.Start();

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


        void updateLIST(int size)
        {
            bDELETE.Content = "";
            MYLIST.Items.Refresh();
            bDELETE.Content = "";
            data.edit_index = -1;

        }


      

        private void timer_Tick(object sender, EventArgs e)
        {
   


            ////delete
            //ContactsListView.DeleteItem(ContactsListView.SelectedIndex);
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //var win = new Window1 { DataContext = new ViewModelWindow1(tb1.Text) };
           // win.Show();
            //this.Close();
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
            //label_kanals.Content = "none";
          

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //// create the demo items provider according to specified parameters
            //int numItems = 10000;// int.Parse(tbNumItems.Text);
            //int fetchDelay = 10;// int.Parse(tbFetchDelay.Text);
            //CollectionProvider myProvider = new CollectionProvider(numItems, fetchDelay);

            //// create the collection according to specified parameters
            //int pageSize = 100;// int.Parse(tbPageSize.Text);
            //int pageTimeout = 5;// int.Parse(tbPageTimeout.Text);

            //if (false)
            //{
            //    //DataContext = new List<Customer>(customerProvider.FetchRange(0, customerProvider.FetchCount()));
            //}
            //else if (false)
            //{
            //    //DataContext = new VirtualizingCollection<Customer>(customerProvider, pageSize);
            //}
            //else if (true)
            //{
            //    DataContext =  new AsyncVirtualizingCollection<ParamCanal>( myProvider, pageSize, pageTimeout * 100);
               
            //}

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
            int si = MYLIST.SelectedIndex;
            if (si < 0) {  return; }
            var p = MYLIST.SelectedItem as ParamCanal;
            if (p == null) return;

            data.edit_index = si;
 

            data.name = p.name;
            data.extfilter = p.ExtFilter;
            data.grouptitle = p.group_title;
            data.http = p.http;
            data.logo = p.logo;
            data.tvg = p.tvg_name;
            data.ping = p.ping;

  

            var win = new Window1 { DataContext = new ViewModelWindow1(tb1.Text) };
             win.Show();

           

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
            data.edit_index = si;
            data.name = p.name;
            data.extfilter = p.ExtFilter;
            data.grouptitle = p.group_title;
            data.http = p.http;
            data.logo = p.logo;
            data.tvg = p.tvg_name;
            data.ping = p.ping;
            bDELETE.Content = "УДАЛИТЬ " + data.name;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            data.f1 = Ffilter.Text;
            data.f2 = Ffilter2.Text;
            data.f3 = Ffilter3.Text;
        }
    }
}
