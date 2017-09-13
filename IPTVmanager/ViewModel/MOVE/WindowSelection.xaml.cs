using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.JoshSmith.ServiceProviders.UI;
using System.Windows.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using IPTVman.Model;
using IPTVman.ViewModel;

namespace ListViewDragDropManager
{
    /// <summary>
    /// Demonstrates how to use the ListViewDragManager class.
    /// </summary>
    public partial class WindowSELECT : Window
    {
        ListViewDragDropManager<Task> dragMgr;
        ListViewDragDropManager<Task> dragMgr2;
        public static event Delegate_UpdateCollection Event_UpdateCollection;
        ObservableCollection<Task> tasks2;


        public WindowSELECT()
        {
            InitializeComponent();
            this.Loaded += WindowMOVE_Loaded;
        }

        #region WindowMOVE_Loaded

        void WindowMOVE_Loaded(object sender, RoutedEventArgs e)
        {

            INIT();
        }


        void INIT()
        {
            // Give the ListView an ObservableCollection of Task 
            // as a data source.  Note, the ListViewDragManager MUST
            // be bound to an ObservableCollection, where the collection's
            // type parameter matches the ListViewDragManager's type
            // parameter (in this case, both have a type parameter of Task).
            ObservableCollection<Task> tasks = Task.CreateTasks();
            this.listView.ItemsSource = tasks;//


            tasks2 = Task.CreateTasks2(null);
            this.listView2.ItemsSource = tasks2;

            // This is all that you need to do, in order to use the ListViewDragManager.
            this.dragMgr = new ListViewDragDropManager<Task>(this.listView);
            this.dragMgr2 = new ListViewDragDropManager<Task>(this.listView2);

            // Turn the ListViewDragManager on and off. 

            // Hook up events on both ListViews to that we can drag-drop
            // items between them.
            this.listView.DragEnter += OnListViewDragEnter;
            this.listView2.DragEnter += OnListViewDragEnter;
            this.listView.Drop += OnListViewDrop;
            this.listView2.Drop += OnListViewDrop;


        }
        #endregion // WindowMOVE_Loaded

        #region dragMgr_ProcessDrop

        // Performs custom drop logic for the top ListView.
        void dragMgr_ProcessDrop(object sender, ProcessDropEventArgs<Task> e)
        {
            // This shows how to customize the behavior of a drop.
            // Here we perform a swap, instead of just moving the dropped item.

            int higherIdx = Math.Max(e.OldIndex, e.NewIndex);
            int lowerIdx = Math.Min(e.OldIndex, e.NewIndex);

            if (lowerIdx < 0)
            {
                // The item came from the lower ListView
                // so just insert it.
                e.ItemsSource.Insert(higherIdx, e.DataItem);
            }
            else
            {
                // null values will cause an error when calling Move.
                // It looks like a bug in ObservableCollection to me.
                if (e.ItemsSource[lowerIdx] == null ||
                    e.ItemsSource[higherIdx] == null)
                    return;

                // The item came from the ListView into which
                // it was dropped, so swap it with the item
                // at the target index.
                e.ItemsSource.Move(lowerIdx, higherIdx);
                e.ItemsSource.Move(higherIdx - 1, lowerIdx);
            }

            // Set this to 'Move' so that the OnListViewDrop knows to 
            // remove the item from the other ListView.
            e.Effects = DragDropEffects.Move;
        }

        #endregion // dragMgr_ProcessDrop

        #region OnListViewDragEnter

        // Handles the DragEnter event for both ListViews.
        void OnListViewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        #endregion // OnListViewDragEnter

        #region OnListViewDrop

        // Handles the Drop event for both ListViews.
        void OnListViewDrop(object sender, DragEventArgs e)
        {
            if (e.Effects == DragDropEffects.None)
                return;

            Task task = e.Data.GetData(typeof(Task)) as Task;
            if (sender == this.listView)
            {
                if (this.dragMgr.IsDragInProgress)
                    return;

                // An item was dragged from the bottom ListView into the top ListView
                // so remove that item from the bottom ListView.
                (this.listView2.ItemsSource as ObservableCollection<Task>).Remove(task);
            }
            else
            {
                if (this.dragMgr2.IsDragInProgress)
                    return;

                // An item was dragged from the top ListView into the bottom ListView
                // so remove that item from the top ListView.
                (this.listView.ItemsSource as ObservableCollection<Task>).Remove(task);
            }
        }

        #endregion // OnListViewDrop

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        string title = "selected";
        //save
        private void button_Click(object sender, RoutedEventArgs e)
        {
            FileWork _file = new FileWork();
            CONVERT();
            _file.SAVE(ViewModelMain.myLISTselect, "selected");
            _file = null;
        }

        private void button_ClickCANCEL(object sender, RoutedEventArgs e)
        {
            if (Event_UpdateCollection != null) Event_UpdateCollection(new IPTVman.Model.ParamCanal());
            this.Close();
        }

        private void inc_Click(object sender, RoutedEventArgs e)
        {
            listView.FontSize += 1;
            if (listView.FontSize > 56) listView.FontSize = 56;

            listView2.FontSize += 1;
            if (listView2.FontSize > 56) listView2.FontSize = 56;
        }

        private void dec_Click(object sender, RoutedEventArgs e)
        {
            listView.FontSize -= 1;
            if (listView.FontSize < 8) listView.FontSize = 8;
            listView2.FontSize -= 1;
            if (listView2.FontSize < 8) listView2.FontSize = 8;
        }


        FileWork _file;
        private void button_ADD(object sender, RoutedEventArgs e)
        {
            if (Wait.IsOpen) return;
            if (LongtaskPingCANCELING.isENABLE()) return;
            if (IPTVman.Model.loc.openfile) return;
            loc.openfile = true;
            if (ViewModelMain.myLISTselect == null)
                ViewModelMain.myLISTselect = new List<ParamCanal>();

            _file = new FileWork();
            _file.Task_Completed += _file_Task_Completed;
            _file.LOAD(ViewModelMain.myLISTselect, title, false, false);           
        }

        private void _file_Task_Completed()
        {
            BACK();
            title = _file.text_title;
            _file = null;
            loc.openfile = false;
        }

        /// <summary>
        /// заполнение list данными из listView
        /// </summary>
        void CONVERT()
        {
            if (ViewModelMain.myLISTselect == null)
                ViewModelMain.myLISTselect = new List<IPTVman.Model.ParamCanal>();
            ViewModelMain.myLISTselect.Clear();

            int size = listView2.Items.Count;
            for (int i = 0; i < size; i++)
            {
                ListViewDragDropManager.Task p = (ListViewDragDropManager.Task)listView2.Items[i];
                IPTVman.Model.ParamCanal canal = new IPTVman.Model.ParamCanal
                {
                    name = p.Name,
                    ExtFilter = "",
                    group_title = "",
                    logo = "",
                    http = p.Http,
                    tvg_name = "",
                    ping = "",
                };

                ViewModelMain.myLISTselect.Add(canal);
            }
        }

        /// <summary>
        /// заполнение listView из list
        /// </summary>
        void BACK()
        {      
            if (ViewModelMain.myLISTselect == null) return;

                tasks2= Task.CreateTasks2(ViewModelMain.myLISTselect);
                listView2.ItemsSource = tasks2;
              
                listView2.Dispatcher.Invoke(new Action(() =>
                {
                    
                    listView2.Items.Refresh();
                }));
        }


        private void Button_ClickRadio(object sender, RoutedEventArgs e)
        {
            if (ViewModelMain.myLISTbase == null) return;
            if (ViewModelMain.myLISTbase.Count == 0) return;
            int size = listView2.Items.Count;
            if (size == 0) return;

            foreach (Window win in Application.Current.Windows)
            {
                if (win.Name == "win7radio")
                {
                    return;
                }
            }
          
            CONVERT();

            IPTVman.Model.data.mode_radio_from_select = true;
            new ListViewDragDropManager.WindowRadio
            {
                //DataContext = new ViewModelWindow2(tb1.Text),
                Title = "Интернет РАДИО",
                Topmost = true,
                //WindowStyle = WindowStyle.ToolWindow,
                Name = "win7radio"
            }.Show(); ;
        }
    }
}