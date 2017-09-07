using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPTVman.Helpers;
using IPTVman.Model;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace IPTVman.ViewModel
{
    partial class ViewModelWindowReplace : ViewModelMain
    {
        public static event Delegate_UpdateCollection Event_UpdateCollection;
        CancellationTokenSource cts1 = new CancellationTokenSource();
        CancellationToken cancellationToken;
        Task task1;
        bool loc = false;
        public static event Action<bool> Event_Waiting;

        public RelayCommand key_ReplaceCommandSTART { get; set; }

        //============================== INIT ==================================
        public ViewModelWindowReplace()
        {
            key_ReplaceCommandSTART = new RelayCommand(key_replace);
        }
        //======================================================================
        bool find = false;
        
        async void key_replace(object selectedItem)
        {
            if (loc) return;
            loc = true;
            if (Event_Waiting != null) Event_Waiting(true);
            find = false;

            if (ViewModelMain.myLISTbase == null) return;
            if (ViewModelMain.myLISTbase.Count == 0) return;

            cancellationToken = cts1.Token;//для task1
            try
            {
                string rez = await Replace(cancellationToken);
            }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА замены " + e.Message.ToString());
            }
            loc = false;
            if (Event_Waiting != null) Event_Waiting(false);
        }

        async Task<string> Replace(CancellationToken Token)
        {
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            var tcs = new TaskCompletionSource<string>();
            task1 = Task.Run(() =>
            {
                testNULL();
                try
                {
                    foreach (var k in ViewModelMain.myLISTbase)
                    {
                        if (chek1) { if (prov(k.name, k)) { continue; } }
                        if (chek2) { if (prov(k.ping, k)) { continue; } }
                        if (chek3) { if (prov(k.ExtFilter, k)) { continue; } }
                        if (chek4) { if (prov(k.group_title, k)) { continue; } }
                        if (chek5) { if (prov(k.http, k)) { continue; } }
                        if (chek6) { if (prov(k.logo, k)) { continue; } }
                        if (chek7) { if (prov(k.tvg_name, k)) { continue; } }
                    }

                    if (!find) if (sel1 != "") dialog.Show("Не найдено '" + sel1 + "'");
                                                else dialog.Show("НЕ ЗАДАНА СТРОКА ПОИСКА");
                    else
                    if (Event_UpdateCollection != null) Event_UpdateCollection(ViewModelMain.myLISTbase[0]);
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
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

            try { await task1; }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА замены " + e.Message.ToString());
            }
            task1.Dispose();
            task1 = null;
            return "";
        }

        bool prov(string s, ParamCanal kan)
        {
            if (s == null) return false;

            if (sel1 == "*")
            {
                setregular(kan); return true;
            }

            Regex regex = new Regex(sel1);
            Match match = null;
            match = regex.Match(s);
            if (match.Success)
            {
                set(kan); return true;
            }
            return false;
        }

        void testNULL()
        {
            int index = 0;
            foreach (var j in ViewModelMain.myLISTfull)
            {
                if (ViewModelMain.myLISTfull[index].name == null) ViewModelMain.myLISTfull[index].name = "";
                if (ViewModelMain.myLISTfull[index].ping == null) ViewModelMain.myLISTfull[index].ping = "";
                if (ViewModelMain.myLISTfull[index].ExtFilter == null) ViewModelMain.myLISTfull[index].ExtFilter = "";
                if (ViewModelMain.myLISTfull[index].group_title == null) ViewModelMain.myLISTfull[index].group_title = "";
                if (ViewModelMain.myLISTfull[index].http == null) ViewModelMain.myLISTfull[index].http = "";
                if (ViewModelMain.myLISTfull[index].logo == null) ViewModelMain.myLISTfull[index].logo = "";
                if (ViewModelMain.myLISTfull[index].tvg_name == null) ViewModelMain.myLISTfull[index].tvg_name = "";
                index++;
            }
        }


        void setregular(ParamCanal k)
        {
            int index = 0;
            foreach (var j in ViewModelMain.myLISTfull)
            {
                if (k.Compare() == j.Compare())
                {
                    if (chek1) ViewModelMain.myLISTfull[index].name = sel2.Trim();
                    if (chek2) ViewModelMain.myLISTfull[index].ping = sel2.Trim();
                    if (chek3) ViewModelMain.myLISTfull[index].ExtFilter = sel2.Trim();
                    if (chek4) ViewModelMain.myLISTfull[index].group_title = sel2.Trim();
                    if (chek5) ViewModelMain.myLISTfull[index].http = sel2.Trim();
                    if (chek6) ViewModelMain.myLISTfull[index].logo = sel2.Trim();
                    if (chek7) ViewModelMain.myLISTfull[index].tvg_name = sel2.Trim();
                    find = true;
                    return;
                }
                index++;
            }
        }

        void set(ParamCanal k)
        {
            int index = 0;
            foreach (var j in ViewModelMain.myLISTfull)
            {
                if (k.Compare() == j.Compare())
                {
                    if (chek1) ViewModelMain.myLISTfull[index].name = ViewModelMain.myLISTfull[index].name.Replace(sel1, sel2).Trim();
                    if (chek2) ViewModelMain.myLISTfull[index].ping = ViewModelMain.myLISTfull[index].ping.Replace(sel1, sel2).Trim();
                    if (chek3) ViewModelMain.myLISTfull[index].ExtFilter = ViewModelMain.myLISTfull[index].ExtFilter.Replace(sel1, sel2).Trim();
                    if (chek4) ViewModelMain.myLISTfull[index].group_title = ViewModelMain.myLISTfull[index].group_title.Replace(sel1, sel2).Trim();
                    if (chek5) ViewModelMain.myLISTfull[index].http = ViewModelMain.myLISTfull[index].http.Replace(sel1, sel2).Trim();
                    if (chek6) ViewModelMain.myLISTfull[index].logo = ViewModelMain.myLISTfull[index].logo.Replace(sel1, sel2).Trim();
                    if (chek7) ViewModelMain.myLISTfull[index].tvg_name = ViewModelMain.myLISTfull[index].tvg_name.Replace(sel1, sel2).Trim();
                    find = true;
                    return;
                }
                index++;
            }
        }

        //============================== object ==================================
        string sel1="";
        public string Selected1
        {
            get { return sel1; }
            set
            {
                sel1 = value;
                RaisePropertyChanged("Selected1");
            }
        }
        string sel2="";
        public string Selected2
        {
            get { return sel2; }
            set
            {
                sel2 = value;
                RaisePropertyChanged("Selected2");
            }
        }

        bool chek1;
        public bool CH1
        {
            get { return chek1; }
            set
            {
                if (chek1) chek1 = false; else chek1 = true;
                RaisePropertyChanged("CH1");
            }
        }


        bool chek2;
        public bool CH2
        {
            get { return chek2; }
            set
            {
                if (chek2) chek2 = false; else chek2 = true;
                RaisePropertyChanged("CH2");
            }
        }


        bool chek3;
        public bool CH3
        {
            get { return chek3; }
            set
            {
                if (chek3) chek3 = false; else chek3 = true;
                RaisePropertyChanged("CH3");
            }
        }

        bool chek4;
        public bool CH4
        {
            get { return chek4; }
            set
            {
                if (chek4) chek4 = false; else chek4 = true;
                RaisePropertyChanged("CH4");
            }
        }


        bool chek5;
        public bool CH5
        {
            get { return chek5; }
            set
            {
                if (chek5) chek5 = false; else chek5 = true;
                RaisePropertyChanged("CH5");
            }
        }


        bool chek6;
        public bool CH6
        {
            get { return chek6; }
            set
            {
                if (chek6) chek6 = false; else chek6 = true;
                RaisePropertyChanged("CH6");
            }
        }


        bool chek7;
        public bool CH7
        {
            get { return chek7; }
            set
            {
                if (chek7) chek7 = false; else chek7 = true;
                RaisePropertyChanged("CH7");
            }
        }

        bool chek8;
        public bool CH8
        {
            get { return chek8; }
            set
            {
                if (chek8) chek8 = false; else chek8 = true;
                RaisePropertyChanged("CH8");
            }
        }

       
       

    }
}
