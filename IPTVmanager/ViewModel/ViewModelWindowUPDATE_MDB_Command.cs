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

namespace IPTVman.ViewModel
{
    partial class ViewModelWindowMDB : ViewModelMain
    {
        public static Access _bd;
        public RelayCommand key_UPDATECommand { get; set; }

        //============================== INIT ==================================
        public ViewModelWindowMDB()
        {
            key_UPDATECommand = new RelayCommand(key_update);
            //sel1 = "что";
            //sel2 = "чем";
            _bd = new Access();

        }
        //======================================================================



        void key_update(object selectedItem)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "access files(*.mdb)|*.mdb";

            if (openFileDialog.ShowDialog() == true)
            {

                _bd.connect(openFileDialog.FileName);

                if (!_bd.is_connect()) { MessageBox.Show("НЕТ ВОЗМОЖНОСТИ ПОДКЛЮЧИТЬСЯ К БАЗЕ\n"+_bd.error);  return; }
            }

            _bd.UPDATE_ITEM();
        }



        //============================== object ==================================
        string sel1;
        public string Selected1
        {
            get { return sel1; }
            set
            {
                sel1 = value;
                RaisePropertyChanged("Selected1");
            }
        }
        string sel2;
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
        public new bool CH1
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
