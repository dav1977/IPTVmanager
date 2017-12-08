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
using System.IO;
using IPTVman.Model;
using System.Timers;
using System.Text.RegularExpressions;

namespace ListViewDragDropManager
{
    /// <summary>
    /// Demonstrates how to use the ListViewDragManager class.
    /// </summary>
    public partial class WindowRadio : Window
    {
        public Window header;
        System.Threading.Tasks.Task taskRADIO;
        ListViewDragDropManager<Task> dragMgr;
        ListViewDragDropManager<Task> dragMgr2;
        System.Timers.Timer Timer1;

        public WindowRadio()
        {
            InitializeComponent();
            this.Loaded += WindowMOVE_Loaded;
            CreateTimer1(300);
            IPTVman.ViewModel.ScannerRadio.event_done += scan_done;
            //listView.SelectionMode = SelectionMode.Multiple;

            kill_process(data.NAME_SCANER_SERVER);             
            if (num_open_process()==0)  init_scan_process();

            Thread.Sleep(200);
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

        byte ct_bascn=0;
        private void Timer1Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (lock_scan)
                {
                    string mes = "";
                    if (ct_bascn > 4) ct_bascn = 0;
                    ct_bascn++;
                    if (ct_bascn == 1) mes = "СТОП ";
                    if (ct_bascn == 2) mes = "СТОП . .";
                    if (ct_bascn == 3) mes = "СТОП . . .";
                    if (ct_bascn == 4) mes = "СТОП . . . .";


                    bSCAN.Dispatcher.Invoke(new Action(() =>
                    {
                        bSCAN.Content = mes;
                    }));

                }
                else
                {
                    bSCAN.Dispatcher.Invoke(new Action(() =>
                    {
                        bSCAN.Content = "Сканировать";
                    }));
                }
            }
            catch { }

        }
        #region WindowMOVE_Loaded

        void WindowMOVE_Loaded(object sender, RoutedEventArgs e)
        {
            header = this;
            INIT();

            this.listView.ItemsSource = dataDD.tasks;//
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

        void INIT()
        {
            // Give the ListView an ObservableCollection of Task 
            // as a data source.  Note, the ListViewDragManager MUST
            // be bound to an ObservableCollection, where the collection's
            // type parameter matches the ListViewDragManager's type
            // parameter (in this case, both have a type parameter of Task).
            dataDD.tasks = Task.CreateTasks();  
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

        //save
        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_ClickCANCEL(object sender, RoutedEventArgs e)
        {
            //if (Event_UpdateCollection != null) Event_UpdateCollection(new IPTVman.Model.ParamCanal());
            this.Close();
        }

        private void inc_Click(object sender, RoutedEventArgs e)
        {
            listView.FontSize += 1;
            if (listView.FontSize > 56) listView.FontSize = 56;
        }

        private void dec_Click(object sender, RoutedEventArgs e)
        {
            listView.FontSize -= 1;
            if (listView.FontSize < 8) listView.FontSize = 8;
        }

        
        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListViewDragDropManager.Task p = (ListViewDragDropManager.Task)listView.SelectedItem;

            if ( p==null) { return; }
            
            play.path = System.Reflection.Assembly.GetExecutingAssembly().Location + "Player/nvlcp.exe";
            play.path = play.path.Replace(@"\", @"/");
            play.path = play.path.Replace(@"IPTVmanager.exe", @"");

            if (File.Exists(play.path))
            {
                taskRADIO = System.Threading.Tasks.Task.Run(() => 
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = false;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = play.path;
                    //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.Arguments = p.Http + " " + p.Name + " --radio";
                    //startInfo.WindowStyle = ProcessWindowStyle.Maximized;

                    play.playerV = Process.Start(startInfo);
 
                });
            }
            else IPTVman.ViewModel.dialog.Show("Не найден файл nVLC player по пути\n" + play.path);

        }

        void zap()
        {
            string prof = "  --- ";
            int ct = 0;
            foreach (var line in dataDD.tasks)
            {
                if (!prefix[ct])
                {
                    prefix[ct] = true;
                    if (line.Playing != "") dataDD.tasks[ct].Playing = prof + line.Playing;
                }
                ct++;
            }
            listView.Dispatcher.Invoke(new Action(() =>
            {
                listView.Items.Refresh();
            }));

        }


        bool scanner_is_null()
        {
            if (scanner == null)
            {
                scanner = new IPTVman.ViewModel.ScannerRadio();
                if (prefix == null) prefix = new bool[listView.Items.Count + 1];
                return true;
            }
            else return false;
        }

        bool waiting_result=false;
        bool need_stop_scan = false;
        IPTVman.ViewModel.ScannerRadio scanner;
        int[] key;
        bool[] prefix;
        ListViewDragDropManager.Task it;
        bool lock_scan = false;
        /// <summary>
        /// SCANER
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void scan_Click(object sender, RoutedEventArgs e)
        {
            if (waiting_result) { need_stop_scan = true; return; }
            waiting_result = true;
            Thread.Sleep(200);
            int num = listView.SelectedIndex;
            bool cycen = (bool)cyc.IsChecked;
           
            key = new int[listView.Items.Count + 1];
            if (scanner_is_null()) { }
            else
            { //повторный скан
                if (lock_scan) return;
                zap();
            }

            if (lock_scan) return;
            lock_scan = true;
            IPTVman.ViewModel.WinPOP.Create("Старт сканирования...", 1, header);
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            var task1 = System.Threading.Tasks.Task.Run(() =>
            {
                var tcs = new TaskCompletionSource<string>();
                try
                {
                    cycscan(num, cycen);
                    lock_scan = false;
                }
                catch (OperationCanceledException ex)
                {
                    tcs.SetException(ex);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                return tcs.Task;
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try { await task1; }
            catch (Exception ex)
            {
               IPTVman.ViewModel.dialog.Show("ОШИБКА-СКАНЕРА " + ex.Message.ToString());
               waiting_result = false;
            }         
        }
  
        int current = 1;
        /// <summary>
        /// Cтарт сканирования num - с какой позиции  enabled_cyc - циклически
        /// </summary>
        /// <param name="num"></param>
        void cycscan(int num, bool enabled_cyc)
        {
            if (num_open_process() == 0) init_scan_process();
            if (!enabled_cyc) current = 1;
           while (true)
           {
                if (need_stop_scan) { break; }
                byte max = 0;
                scanner.clear();
                int j = 0;
                foreach (var item in listView.Items)
                {
                    if (j < num) { j++; continue; }
                    it = (ListViewDragDropManager.Task)item;

                    if (key[j] < current)
                    {
                        key[j] += 1;
                        scanner.add(it.Http);
                        max++;
                        if (max > 1) break;
                    }

                    j++;
                }

                if (max == 0) { if (!enabled_cyc) break; else { Thread.Sleep(3000); zap();  current++; } }//циклически

                waiting_result = true;
                scanner.getPLAYING();
               
                while (waiting_result)
                {
                    if (need_stop_scan) { break; }
                    Thread.Sleep(100);
                }
                if (need_stop_scan) { break; }
                waiting_result = true;
            }

            waiting_result = false; need_stop_scan = false;
        }


        string NAMEPLAYER = "nvlcp";
        string NAMEPLAYERexe = "nvlcp.exe";
        Process[] myProcesses;


        void kill_process(string titleNAME)
        {
            myProcesses = Process.GetProcessesByName(NAMEPLAYER);

            bool killok = false;
            if (myProcesses.Length > 1)
            {
                killok = IPTVman.ViewModel.FileWork.KILL_PROCESS(titleNAME);
            }

            if (killok) return;

            myProcesses = Process.GetProcessesByName(NAMEPLAYER);

            if (myProcesses.Length > 1)
            {
                foreach (var proc in myProcesses)
                {
                    if (titleNAME == "") proc.Kill();
                    else
                    if (proc.MainWindowTitle == titleNAME) proc.Kill();
                    proc.WaitForExit(7000);
                }
            }
        }


        int num_open_process()
        {
            byte size = 0;
            myProcesses = Process.GetProcessesByName(NAMEPLAYER);

            if (myProcesses.Length > 1)
            {
                foreach (var proc in myProcesses)
                {
                    if (proc.MainWindowTitle == data.NAME_SCANER_SERVER) size++;
                }
            }
            return size;

        }

        void init_scan_process()
        {
           
            myProcesses = Process.GetProcessesByName(NAMEPLAYER);
          
                play.path = System.Reflection.Assembly.GetExecutingAssembly().Location + "Player/" + NAMEPLAYERexe;
                play.path = play.path.Replace(@"\", @"/");
                play.path = play.path.Replace(@"IPTVmanager.exe", @"");

                if (File.Exists(play.path))
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.CreateNoWindow = false;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = play.path;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.Arguments = "null" + " " + "null" + " --scan";
                   
                    play.playerV = Process.Start(startInfo);
                    play.playerV.Exited += exitPROCESS;

                }
                else IPTVman.ViewModel.dialog.Show("Не найден " + NAMEPLAYERexe + " по пути\n" + play.path);


                while (myProcesses.Length == 0)
                {
                    myProcesses = Process.GetProcessesByName(NAMEPLAYER);
                    if (myProcesses.Length != 0) break;
                    Thread.Sleep(100);
                }
      
        }

        private void exitPROCESS(object sender, EventArgs e)
        {
            play.playerV = null;
        }

        void scan_done()
        {
            if (scanner.result == null) goto exit;
            if (scanner.result.Count == 0) goto exit;
            int ct = 0;
            foreach (var line in dataDD.tasks)
            {
                for (int i = 0; i < scanner.result.Count-1; i++)
                {
                    if (line.Http == scanner.result[i])
                    {
                        prefix[ct] = false;
                        string btr= "   [" + scanner.result[i + 2].ToString() + " kbs]";
                        if (scanner.result[i + 2].ToString() == "") btr = "";
                        if (scanner.result[i + 2].ToString() == "0") btr = "";
                        if (scanner.result[i + 2].ToString() == "?") btr = "";
                        dataDD.tasks[ct].Playing = scanner.result[i + 1] + btr; 
                    } 
                }
                ct++;
    
            }
            
            listView.Dispatcher.Invoke(new Action(() =>
            {
                listView.Items.Refresh();
            }));
            exit:
            waiting_result = false;
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            exit();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            exit();
        }

        void exit()
        {
            //if (scanner == null)
            //{
            //    scanner = new IPTVman.ViewModel.ScannerRadio();
            //    if (prefix == null) prefix = new bool[listView.Items.Count + 1];
            //}


            try
            {
                scanner_is_null();//создание если null
                scanner.CLOSE_SCANNER();//команда на закрытие окна сканнера
                need_stop_scan = true;
               while (waiting_result) Thread.Sleep(100);

               //waiting_result = true;
               //while (waiting_result) Thread.Sleep(100);

                //try
                //{
                //    if (play.playerV != null) play.playerV.Kill();
                //}
                //catch { }

            //kill_process(data.NAME_SCANER_SERVER);

            }
            catch(Exception ex)
            {
                MessageBox.Show("error close radio "+ex);
            }

        }



    }
}