using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace IPTVman.ViewModel
{
    public partial class WindowSettings : Window 
    { 
        public WindowSettings()
        {
            InitializeComponent();
            data.Upadate_LIST += Data_Upadate_LIST;
            data.UpdateLIST();
            lw1.ItemsSource = vst1;
            lw2.ItemsSource = vst2;
            comboBox.ItemsSource = devices;

            if (!WinPOP.init_ok) this.Close();
            string def= "0";
            for (byte i = 0; i < 10; i++)
            {
                string name = data._bass.getNameDevice(i);
                if (name == "") break;
                _devices.Add(name);
                if (i == 1) def = name;
            }

            comboBox.SelectedItem = def;
            lok_combo = false;
        }


        ObservableCollection<string> _vst1 = new ObservableCollection<string>();
        public ObservableCollection<string> vst1
        { get { return _vst1; } }

        ObservableCollection<string> _vst2 = new ObservableCollection<string>();
        public ObservableCollection<string> vst2
        { get { return _vst2; } }

        ObservableCollection<string> _devices = new ObservableCollection<string>();
        public ObservableCollection<string> devices
        { get { return _devices; } }

        private void Data_Upadate_LIST(ObservableCollection<string> arg1, ObservableCollection<string> arg2)
        {
            _vst1.Clear();
                foreach (var s in arg1)
                    _vst1.Add(data._bass.get_nameVTS(s));


            _vst2.Clear();
            foreach (var s in arg2)
                _vst2.Add(data._bass.get_nameVTS(s));

            //lw1.Dispatcher.Invoke(new Action(() =>
            //{
            //    lw1.Items.Clear();
            //    foreach (var s in arg1)
            //        lw1.Items.Add(data.get_nameVTS(s) + "\n");

            //}));

            //lw2.Dispatcher.Invoke(new Action(() =>
            //{
            //    lw2.Items.Clear();
            //    foreach (var s in arg2)
            //        lw1.Items.Add(data.get_nameVTS(s) + "\n");

            //}));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
          
        }

        private void exit_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

      
        //ENABLE VST
        private void bVSTon_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (data.selected_indexLIST < 0)
                {
                    current.Dispatcher.Invoke(new Action(() =>
                    {
                        current.Content = "no select";
                    }));
                    return;
                }
                string sel = data._bass.get_nameVTS(data.pathVST[data.selected_indexLIST]);

                if (data._bass.VSTisENABLED(sel)) { MessageBox.Show(sel+" Уже подключен");  return; }//уже подключен

                string p = data.pathVST[data.selected_indexLIST];
                if (data._bass.VST_ENABLE(p) )
                {
                    data.workVST.Add(p);
                    data.UpdateLIST();
                }
               
            }
            catch (Exception ex) { Trace.WriteLine(ex.Message); }
        }



        //Disable VST
        private void bVSToff_Click(object sender, RoutedEventArgs e)
        {
            data._bass.VST_DISABLE(currentWORK.Content.ToString());
        }

        //add VST
        private void baddVST_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory  = AppDomain.CurrentDomain.BaseDirectory + @"\VST";
            openFileDialog.Filter = "VST dll files(*.dll)|*.dll";

            if (openFileDialog.ShowDialog() == true)
            {
                if (data._bass.path_is_ok(openFileDialog.FileName)) return;
                data._bass.addVST(openFileDialog.FileName);
            }
        }

        
        private void bOpen_Click(object sender, RoutedEventArgs e)
        {
            if (data.selected_indexWORK < 0)
            {
                currentWORK.Dispatcher.Invoke(new Action(() =>
                {
                    currentWORK.Content = "no select";
                }));
                return;
            }
            string s = "";
            currentWORK.Dispatcher.Invoke(new Action(() =>
            {
                s = currentWORK.Content.ToString();
            }));
            if (s == "" || s=="--") return;
            data._bass.OPEN_VST(s);
        }

        private void lw1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string s = "--";
            lw1.Dispatcher.Invoke(new Action(() =>
            {
                int ind = lw1.SelectedIndex;
                if (ind < 0) return;
                s = lw1.Items[ind].ToString();
                data.selected_indexLIST = ind;
            }));

            if (s == "") s = "???";
            current.Dispatcher.Invoke(new Action(() =>
            {
                current.Content = s;
            }));

        }

        private void lw2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string s = "--";
            lw2.Dispatcher.Invoke(new Action(() =>
            {
                int ind = lw2.SelectedIndex;
                if (ind < 0) return;
                s = lw2.Items[ind].ToString();
                data.selected_indexWORK = ind;
            }));

            if (s == "") s = "???";
            currentWORK.Dispatcher.Invoke(new Action(() =>
            {
                currentWORK.Content = s;
            }));
        }

        private void bVSTremove_Click(object sender, RoutedEventArgs e)
        {
            string s = "--";
            lw1.Dispatcher.Invoke(new Action(() =>
            {
                int ind = lw1.SelectedIndex;
                if (ind < 0) return;
                s = lw1.Items[ind].ToString();
                data.selected_indexLIST = -1;
            }));
            data._bass.remove_VST(s, ref data.pathVST);

            current.Dispatcher.Invoke(new Action(() =>
            {
                current.Content = "--";
            }));
            data.UpdateLIST();
        }

       

        bool lok_combo=true;
        //select device output
        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lok_combo) return;
            var name = comboBox.SelectedItem;
            byte ind = (byte)comboBox.SelectedIndex;
            data._bass.ChannelSetDevice( ind , name.ToString());
        }

        private void bSAVExmlDefault_Click(object sender, RoutedEventArgs e)
        {
            datafile.SAVEtoXML(data.DefaultPath);
        }

        private void bSAVExml_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog FileDialog = new SaveFileDialog();
            FileDialog.Filter = "params (*.xml)|*.xml";
            if (FileDialog.ShowDialog() == true)
            {

                data._bass.Get_All_Param_VST();
                datafile.SAVEtoXML(FileDialog.FileName);
            }
        }

        private void bLOADxml_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "params (*.xml)|*.xml";
            if (openFileDialog.ShowDialog() == true)
            {
                data.listPARAM.Clear();
                datafile.ReadFromXML(openFileDialog.FileName);
                data._bass.SET_All_Param_VST();
            }

        }
    }
}
