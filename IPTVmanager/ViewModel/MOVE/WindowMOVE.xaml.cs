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

namespace ListViewDragDropManager
{
    /// <summary>
    /// Demonstrates how to use the ListViewDragManager class.
    /// </summary>
    public partial class WindowMOVE : System.Windows.Window
    {
        ListViewDragDropManager<Task> dragMgr;
        ListViewDragDropManager<Task> dragMgr2;


        public static event IPTVman.ViewModel.Delegate_UpdateMOVE Event_UpdateAFTERmove;


        public WindowMOVE()
        {
            InitializeComponent();
            this.Loaded += WindowMOVE_Loaded;
        }

        #region WindowMOVE_Loaded

        void WindowMOVE_Loaded(object sender, RoutedEventArgs e)
        {
            // Give the ListView an ObservableCollection of Task 
            // as a data source.  Note, the ListViewDragManager MUST
            // be bound to an ObservableCollection, where the collection's
            // type parameter matches the ListViewDragManager's type
            // parameter (in this case, both have a type parameter of Task).
            ObservableCollection<Task> tasks = Task.CreateTasks();
            this.listView.ItemsSource = tasks;

            this.listView2.ItemsSource = new ObservableCollection<Task>();

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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            foreach (var s in ListViewDragDropManager.Task.list)
            {
                IPTVman.ViewModel.ViewModelMain.myLISTbase[i].name = s.Name;
                IPTVman.ViewModel.ViewModelMain.myLISTbase[i].ExtFilter = s.ExtFilter;
                IPTVman.ViewModel.ViewModelMain.myLISTbase[i].group_title = s.Group_title;
                IPTVman.ViewModel.ViewModelMain.myLISTbase[i].http = s.Http;
                IPTVman.ViewModel.ViewModelMain.myLISTbase[i].ping = s.Ping;
                IPTVman.ViewModel.ViewModelMain.myLISTbase[i].logo = s.Logo;
                IPTVman.ViewModel.ViewModelMain.myLISTbase[i].tvg_name = s.Tvg;
               
                i++;
            }

            foreach (var s in ListViewDragDropManager.Task.list)
            {
                if (!s.Finished) continue;
                
                var item = IPTVman.ViewModel.ViewModelMain.myLISTfull.Find(x =>
                ( x.name == s.Name && x.ExtFilter == s.ExtFilter && x.group_title == s.Group_title));

                if (item!=null) IPTVman.ViewModel.ViewModelMain.myLISTfull.Remove(item);

            }


            if (Event_UpdateAFTERmove != null) Event_UpdateAFTERmove(new IPTVman.Model.ParamCanal());
            this.Close();
        }


        private void button_ClickCANCEL(object sender, RoutedEventArgs e)
        {

            if (Event_UpdateAFTERmove != null) Event_UpdateAFTERmove(new IPTVman.Model.ParamCanal());
            this.Close();

        }

    }
}