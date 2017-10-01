using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using IPTVman.Model;
using IPTVman.Helpers;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Web;
//using System.Diagnostics;

namespace IPTVman.ViewModel
{
    partial class ViewModelMain : ViewModelBase
    {
        public RelayCommand key_UpdateMDBCommand { get; set; }
        public RelayCommand key_DelDUBLICATCommand { get; set; }
        public RelayCommand key_ReplaceCommand { get; set; }
        public RelayCommand key_OPENclipboarCommand { get; set; }
        public RelayCommand key_SetAllBestCommand { get; set; }
        public RelayCommand key_FILTERmoveDragCommand { get; set; }
        public RelayCommand key_FILTERmoveCommand { get; set; }
        public RelayCommand key_AUTOPINGCommand { get; set; }
        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand key_OPENCommand { get; set; }
        public RelayCommand key_SAVECommand { get; set; }
        public RelayCommand key_delCommand { get; set; }
        public RelayCommand key_DelFILTERCommand { get; set; }
        public RelayCommand key_radio { get; set; }
        public RelayCommand key_DelALLkromeBESTCommand{ get; set; }
        public RelayCommand key_FILTERCommand { get; set; }
        public RelayCommand key_FilterOnlyBESTCommand { get; set; }

        void ini_command()
        {
            key_UpdateMDBCommand = new RelayCommand(Update_MDB);
            key_DelDUBLICATCommand = new RelayCommand(DelDUBLICAT);
            key_ReplaceCommand =  new RelayCommand(key_Replace);
            key_OPENclipboarCommand = new RelayCommand(key_OPEN_clipboard);
            key_SetAllBestCommand = new RelayCommand(key_set_all_best);
            key_FILTERmoveDragCommand = new RelayCommand(key_dragdrop);
            key_radio = new RelayCommand(Key_radio);
            key_FILTERmoveCommand = new RelayCommand(key_move);
            key_AUTOPINGCommand = new RelayCommand(key_AUTOPING);
            key_ADDCommand = new RelayCommand(key_ADD);
            key_OPENCommand = new RelayCommand(key_OPEN);
            key_SAVECommand = new RelayCommand(key_SAVE);
            key_delCommand = new RelayCommand(key_del);
            key_DelFILTERCommand = new RelayCommand(key_delFILTER);
            key_FILTERCommand = new RelayCommand(key_FILTER);
            key_FilterOnlyBESTCommand = new RelayCommand(key_FILTERbest);
            key_DelALLkromeBESTCommand = new RelayCommand(key_delALLkromeBEST);
        }

        void key_dragdrop(object parameter)
        {


        }
        void Key_radio(object parameter)
        {


        }
        void key_move(object parameter)
        {


        }


        Window winrep;
        /// <summary>
        /// replace
        /// </summary>
        /// <param name="parameter"></param>
        void key_Replace(object parameter)
        {
            if (Wait.IsOpen) return;
            if (myLISTbase == null) return;
            if (myLISTbase.Count == 0) return;
            if (winrep != null) return;

            winrep = new WindowReplace()
            {
                Title = "ЗАМЕНА",
                Topmost = true,
                WindowStyle = WindowStyle.ToolWindow,
                Name = "winReplace"
            };

            winrep.Closing += Winrep_Closing;
            winrep.Show();
        }

        private void Winrep_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            winrep = null;
        }

        /// <summary>
        /// set all best
        /// </summary>
        /// <param name="parameter"></param>
        async void key_set_all_best(object parameter)
        {
            if (Wait.IsOpen) return;
            if (LongtaskPingCANCELING.isENABLE()) return;
            if (myLISTfull == null) return;
            if (data.canal.name == "") return;

            MessageBoxResult result = MessageAsk.Create("Переместить пустые группы в избранное?");

            CancellationTokenSource cts1 = new CancellationTokenSource();
            CancellationToken cancellationToken;

            //MessageBoxResult result = MessageBox.Show("  Переместить пустые группы в избранное" , " ПЕРЕМЕЩЕНИЕ",
            //                    MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;

            Wait.Create("Идет заполнение ... ", true);

            cancellationToken = cts1.Token;//для task1
            try
            {
                await AsyncSetBest(cancellationToken);
                Wait.Close();
                Update_collection(typefilter.last);
            }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА SetBest " + e.Message.ToString());
            }
        }

        public Task<string> AsyncSetBest(CancellationToken cts)
        {
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            return Task.Run(() =>
            {
                var tcs = new TaskCompletionSource<string>();
                try
                {
                    Wait.set_ProgressBar(ViewModelMain.myLISTbase.Count);
                    int ct = 0;
                    data.set_best();

                    foreach (var s in ViewModelMain.myLISTbase)
                    {
                        if (cts.IsCancellationRequested) break;
                        Wait.progressbar++;
                        ct = 0;
                        foreach (var j in ViewModelMain.myLISTfull)
                        {
                            if (j.Compare() == s.Compare())
                            {
                                if (s.group_title == "")
                                {
                                    ViewModelMain.myLISTfull[ct].ExtFilter = data.best1;
                                    ViewModelMain.myLISTfull[ct].group_title = data.best2;
                                    break;
                                }
                                else
                                {
                                    ViewModelMain.myLISTfull[ct].ExtFilter = data.best1;
                                    break;
                                }
                            }
                            ct++;
                        }

                    }
                    tcs.SetResult("ok");
                }
                catch (OperationCanceledException e)
                {
                    tcs.SetException(e);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                return tcs.Task;
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        }







        AUTOPING ap;
        Window winap;
        PING _ping;
        PING_prepare _pingPREPARE;
        /// <summary>
        /// AUTO PING
        /// </summary>
        /// <param name="parameter"></param>
        async void key_AUTOPING(object parameter)
        {
            if (Wait.IsOpen)  return; 
            if (LongtaskPingCANCELING.isENABLE())    return; 
            if (myLISTbase==null) return;
            if (myLISTbase.Count == 0) return;
            if (winap!=null) return;

            _ping = new PING();
            _pingPREPARE = new PING_prepare(_ping);

            ap = new AUTOPING(_ping, _pingPREPARE);
          
            winap = new WindowPING
            {
                Title = "АВТО ПИНГ",
                Topmost = true,
                WindowStyle = WindowStyle.ThreeDBorderWindow,//WindowStyle.ToolWindow,
                Name = "winPING"
            };

            winap.Closing += Ap_Closing;
            winap.Show();

            await ap.start();
            //winap.Owner = MainWindow.header;
        }

        private void Ap_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Trace.WriteLine("CLOSE WIN AUTOPING");

            LongtaskPingCANCELING.analiz_closing_thread(_ping, _pingPREPARE);
            winap = null;
            ap.stop();
            ap.Dispose();
            ap = null;
            Update_collection(typefilter.last);
        }



        Window mdb;
        /// <summary>
        /// UPDATE MDB
        /// </summary>
        /// <param name="parameter"></param>
        void Update_MDB(object parameter)
        {
            if (Wait.IsOpen) return;
            if (LongtaskPingCANCELING.isENABLE()) return;
            if (myLISTbase == null) return;
            if (myLISTbase.Count == 0) return;
            if (mdb != null) return;

            mdb = new WindowMDB
            {
                Title = "Обновление базы Access",
                Topmost = true,
                WindowStyle = WindowStyle.ToolWindow,
                Name = "update_mdb"
            };

            mdb.Closing += MDB_Closing;
            mdb.Show();
            //mdb.Owner = MainWindow.header;
        }

        private void MDB_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mdb = null;
        }

        /// <summary>
        ///   ADD
        /// </summary>
        /// <param name="parameter"></param>
        void key_ADD(object parameter)
        {
            if (Wait.IsOpen) return;
            CollectionisCreate();
            if (parameter == null) return;
            myLISTfull.Add(new ParamCanal
            { name = parameter.ToString(), ExtFilter = parameter.ToString(), group_title = "" });
            Update_collection(typefilter.last);
        }
        
        /// <summary>
        /// DEL
        /// </summary>
        /// <param name="parameter"></param>
        public void key_del(object parameter)
        {
            if (Wait.IsOpen) return;
            //if (parameter == null || !data.delete) return;
            if (myLISTfull == null) return;
            if (myLISTfull.Count == 0) return;
            if (data.canal.name == "") return;

            MessageBoxResult result = MessageAsk.Create("  УДАЛЕНИЕ " + data.canal.name + "\n" + data.canal.http);
            if (result != MessageBoxResult.Yes) return;

            var item = ViewModelMain.myLISTfull.Find(x =>
                  (x.http == data.canal.http && x.ExtFilter == data.canal.ExtFilter
                      && x.group_title == data.canal.group_title));

            myLISTfull.Remove(item);

            if (myLISTdub != null)
            {
                item = ViewModelMain.myLISTdub.Find(x =>
                      (x.http == data.canal.http && x.ExtFilter == data.canal.ExtFilter
                          && x.group_title == data.canal.group_title));

                myLISTdub.Remove(item);
            }
            Update_collection(typefilter.last);
        }
        /// <summary>
        /// del filter
        /// </summary>
        /// <param name="parameter"></param>
        void key_delFILTER(object parameter)
        {
            if (Wait.IsOpen) return;
            if (myLISTfull == null) return;

            MessageBoxResult result = MessageAsk.Create("  УДАЛЕНИЕ ВСЕХ ПО ФИЛЬТРУ !!!");
            if (result != MessageBoxResult.Yes) return;

            uint ct = 0;

            foreach (var obj in myLISTbase)
            {
                var item = ViewModelMain.myLISTfull.Find(x =>
                 (x.http == obj.http && x.ExtFilter == obj.ExtFilter && x.group_title == obj.group_title));

                if (item != null) { myLISTfull.Remove(item);  ct++; }

            }

            if (_update.lastfilter() == typefilter.dublicate) Update_collection(typefilter.normal);
            else
            Update_collection(typefilter.last);
            //dialog.Show("  УДАЛЕНО "+ct.ToString()+ " Каналов", " ",
            //                   MessageBoxButton.OK, MessageBoxImage.Information);

        }


        async void DelDUBLICAT(object parameter)
        {
            if (Wait.IsOpen) return;
            if (myLISTfull == null) return;
            if (myLISTbase == null) return;
            if (loc.finddublic) return;

            loc.finddublic = true;
            List<ParamCanal> rez =null;

            Wait.Create("Идет поиск дубликатов ...", true);
            Wait.set_ProgressBar(myLISTbase.Count);

            rez = await find_dublicate_task();
        }


        void end_task(List<ParamCanal> rez)
        {
            if (rez == null) { exit(); return; }
            if (rez.Count == 0) dialog.Show("Дубликатов не найдено");
            else
            {
                ViewModelMain.myLISTdub.Clear();
                foreach (var c in rez)
                {
                    ViewModelMain.myLISTdub.Add((ParamCanal)c.Clone());
                }
                Update_collection(typefilter.dublicate);
            }
            exit();
        }

        void exit()
        {
            Wait.Close();
            loc.finddublic = false;
        }

        List<ParamCanal> result = null;
        async Task<List<ParamCanal>> find_dublicate_task()
        {           
            Task task1 = Task.Run(() =>
            {
                var tcs = new TaskCompletionSource<string>();
                try
                {
                    result = _update.find_dublicate(myLISTbase);
                    tcs.SetResult("ok");
                }
                catch (OperationCanceledException e)
                {
                    tcs.SetException(e);
                    exit();
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                    exit();
                }
                return tcs.Task;
            });
            try
            {
                await task1;
                end_task(result);
            }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА find_dublicate " + e.Message.ToString());
                exit();
            }

            return result;
        }

       

           

        /// <summary>
        /// del krome best
        /// </summary>
        /// <param name="parameter"></param>
        void key_delALLkromeBEST(object parameter)
        {
            if (Wait.IsOpen) return;
            if (myLISTfull == null) return;

            MessageBoxResult result = MessageAsk.Create("  УДАЛЕНИЕ ВСЕХ КРОМЕ ИЗБРАННЫХ(ExtFilter)!!!");
            if (result != MessageBoxResult.Yes) return;

            uint ct = 0;

            try
            {
                int i;
                for (i = 0; i < myLISTfull.Count; i++)
                {
                    if (myLISTfull[i].ExtFilter != data.favorite1_1 &&
                        myLISTfull[i].ExtFilter != data.favorite2_1 &&
                        myLISTfull[i].ExtFilter != data.favorite3_1
                        /*|| myLISTfull[i].group_title != data.best2*/)
                    {  myLISTfull.RemoveAt(i); ct++; i--; }

                }
            }
            catch (Exception ex)
            {
                dialog.Show("Ошибка удаления \n" + ex.Message.ToString());
            }

            Update_collection(typefilter.last);
            dialog.Show("  УДАЛЕНО " + ct.ToString() + " Каналов");

        }

        
        /// <summary>
        ///  save
        /// </summary>
        /// <param name="parameter"></param>
        void key_SAVE(object parameter)
        {
            if (Wait.IsOpen) return;
            if (LongtaskPingCANCELING.isENABLE()) return;
            if (myLISTfull == null) return;
            if (myLISTfull.Count == 0) return;

            FileWork _file = new FileWork();
            _file.SAVE(myLISTfull, text_title);
            _file = null;
        }

        /// <summary>
        ///  filter
        /// </summary>
        /// <param name="parameter"></param>
        void key_FILTER(object parameter)
        {
            Update_collection(typefilter.normal);
        }
     
        void key_FILTERbest(object parameter)
        {
            Update_collection(typefilter.best); 
        }

        /// <summary>
        /// open from clipboard
        /// </summary>
        /// <param name="parameter"></param>
        void key_OPEN_clipboard(object parameter)
        {
            if (Wait.IsOpen) return;
            if (LongtaskPingCANCELING.isENABLE()) return;

            CollectionisCreate();
            string[] str = null;
            string get = Clipboard.GetText();

            try
            {
                str = get.Split(new Char[] { '\n' });
            }
            catch (Exception ex)
            {
                dialog.Show("ОШИБКА clb " + ex.Message.ToString());
            }

            if (str == null) return;
            FileWork _file = new FileWork();
            _file.OPEN_FROM_CLIPBOARD( ViewModelMain.myLISTfull,  str);
            _file = null;

        }

        
         void Open_arguments()
        {
            loc.openfile = true;
            CollectionisCreate();
            string name = data.arguments_startup[0];
            loc.block_dialog_window = true;

            FileWork _file = new FileWork();
            _file.LOAD(ViewModelMain.myLISTfull, name);

            _file = null;
            loc.block_dialog_window = false;
            loc.openfile = false;
        }

        void key_OPEN(object parameter)
        {
            if (Wait.IsOpen) return;
            if (LongtaskPingCANCELING.isENABLE()) return;
            if (loc.openfile) return;
            loc.openfile = true;
            CollectionisCreate();

            FileWork _file = new FileWork();
            _file.LOAD(ViewModelMain.myLISTfull, text_title,  chek1, chek2);

            text_title = _file.text_title;
            _file = null;
            loc.openfile = false;
        }




    }//class
}//namespace
