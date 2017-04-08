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

    
    


    public partial class MainWindow : Window
    {
   

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

            //Binding bind = new Binding();
            //bind.Source = grid1;
            //bind.Path = new PropertyPath("grid1.ItemsSource.Count");  
            //bind.Mode = BindingMode.OneWay;
            //label_kanals.SetBinding(label_kanals.Content, bind);
        }


        void updateLIST(int size)
        {

            MYLIST.Items.Refresh();
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
            data.d1 = select1.Text;
            data.d2 = select2.Text;
            data.d3 = select3.Text;
 

        }

        private void Button_Click_BEST(object sender, RoutedEventArgs e)
        {

            data.d1 = select1.Text;
            data.d2 = select2.Text;
            data.d3 = select3.Text;
      
            data.best1 = best1.Text;
            data.best2 = best2.Text;
        }

        private void MYLIST_MouseDoubleClick_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int si = MYLIST.SelectedIndex;
            if (si < 0) { select1.Text = ""; return; }
            var p = MYLIST.SelectedItem as ParamCanal;
            if (p == null) return;

            data.edit_index = si;
            select1.Text = p.name;
            select2.Text = p.ExtFilter;
            select3.Text = p.group_title;

            data.d4 = p.http;
            data.d5 = p.logo;
            data.d6 = p.tvg_name;

            data.delete = true;
        }
    }
}
