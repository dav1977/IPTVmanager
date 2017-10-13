using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace IPTVman.ViewModel
{
        static class data
        {
            public static AudioBass _bass;
            public static event Action<string> Upadate_Setting_Data;
            public static event Action<ObservableCollection<string>, ObservableCollection<string>> Upadate_LIST;

            public static ObservableCollection<List<float>> listPARAM = new ObservableCollection<List<float>>();
            public static ObservableCollection<string> pathVST = new ObservableCollection<string>();
            public static ObservableCollection<string> workVST = new ObservableCollection<string>();
            public static int selected_indexLIST;
            public static int selected_indexWORK;
            public static WCFSERVER _server;
            public static string url = "";
            public static string name = "";
            public static bool mode_radio = false;
            public static bool mode_scan = false;
            public static string title = "";
            public static string buff = "";
            public static string scanURL = "init";
            public static bool exit_programm = false;
            public static string DefaultPath = AppDomain.CurrentDomain.BaseDirectory + "playersettings.xml";

        public static void UpdateLIST()
        {
            if (Upadate_LIST != null) Upadate_LIST(pathVST, workVST);
        }

        public static void UpdateSettings()
        {
            if (Upadate_Setting_Data != null) Upadate_Setting_Data("CURRENTVST");
        }
        
    }


        /// <summary>
        /// данные для окна Settings
        /// </summary>
        public class SettingData : INotifyPropertyChanged
    {
        public SettingData()
        {
            data.Upadate_Setting_Data += Data_Upadate_Setting_Data;
        }

        private void Data_Upadate_Setting_Data(string obj)
        {
            OnPropertyChanged(obj);  
        }

        public string CURRENTVST
        {
            get { return data._bass.current_pathVST; }
            set { }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
