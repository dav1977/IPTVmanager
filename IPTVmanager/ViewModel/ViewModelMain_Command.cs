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

namespace IPTVman.ViewModel
{
    partial class ViewModelMain : ViewModelBase
    {

        //public RelayCommand key_EDITCommand { get; set; }
     //   public RelayCommand key_SORTCommand { get; set; }
        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand key_OPENCommand { get; set; }
        public RelayCommand key_SAVECommand { get; set; }
        public RelayCommand key_delCommand { get; set; }

        public RelayCommand key_FILTERCommand { get; set; }

        void ini_command()
        {
 
           // key_EDITCommand = new RelayCommand(key_EDIT);
         //   key_SORTCommand = new RelayCommand(key_SORT);
            key_ADDCommand = new RelayCommand(key_ADD);
            key_OPENCommand = new RelayCommand(key_OPEN);
            key_SAVECommand = new RelayCommand(key_SAVE);
            key_delCommand = new RelayCommand(key_del);
            key_FILTERCommand = new RelayCommand(key_FILTER);

        }


        //******************************* ADD **************
        void key_ADD(object parameter)
        {
            CollectionisCreate();
            if (parameter == null) return;
            myLISTfull.Add(new ParamCanal
            { name = parameter.ToString(), ExtFilter = parameter.ToString(), group_title = "" });
            
            // if (Event_UpdateLIST != null) Event_UpdateLIST(myLISTbase.Count);
            RaisePropertyChanged("mycol");
            
        }
        

      


        void key_del(object parameter)
        {
            //if (parameter == null || !data.delete) return;
            if (myLISTfull == null) return;
            if (data.edit_index<0) return;

            MessageBoxResult result = MessageBox.Show("  УДАЛЕНИЕ " + data.name + "\n" + data.http, "  УДАЛЕНИЕ",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes) return;
                 
            bool rez = false;
            int index = 0;
            foreach (var obj in myLISTfull)
            {
                if (data.edit_index==index) { myLISTfull.RemoveAt(index); rez = true;  break; }
                index++;
               
            }
            if (!rez) return;
            

    
            RaisePropertyChanged("mycol");
        }

        //void key_EDIT(object parameter)
        //{
        //    if (parameter == null) return;
        //    bool rez = false;

        //    int index = 0;
        //    foreach (var obj in myLISTfull)
        //    {
        //        if (obj.http == data.http) { rez = true; data.edit_index = index;  break; }
        //        index++;

        //    }
        //    if (!rez) return;

        //    ParamCanal p;
        //    p = new ParamCanal { };
        //    myLISTfull.IndexOf(p, data.edit_index);

        //    myLISTfull[data.edit_index].name = data.name;
        //    myLISTfull[data.edit_index].ExtFilter = data.extfilter;
        //    myLISTfull[data.edit_index].group_title = data.grouptitle;
        //    //myLISTfull[data.edit_index].name = select1.ToString();
        //    //myLISTfull[data.edit_index].ExtFilter = select2.ToString();
        //    //myLISTfull[data.edit_index].group_title = select3.ToString();


        //    RaisePropertyChanged("mycol");
        //}


        //void key_SORT(object parameter)
        //{
        //    // ascending
        //    //collection = new ObservableCollection<int>(collection.OrderBy(a => a));

        //    //// descending
        //    //collection = new ObservableCollection<int>(collection.OrderByDescending(a => a));



        //    //  ObservableCollection<string> _animals = new ObservableCollection<string>()
        //    //{ "Cat", "Dog", "Bear", "Lion", "Mouse",
        //    //"Horse", "Rat", "Elephant", "Kangaroo", "Lizard",
        //    //"Snake", "Frog", "Fish", "Butterfly", "Human",
        //    //"Cow", "Bumble Bee"};

        //    //_animals = new ObservableCollection<string>(_animals.OrderBy(i => i));

        //    //Canal = new ObservableCollection<ParamCanal>(Canal.OrderBy(a => a.name));

        //}





        void key_SAVE(object parameter)
        {

            SaveFileDialog openFileDialog = new SaveFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamWriter sr = new StreamWriter(openFileDialog.FileName))
                {
                    sr.Write(text_title +'\n');

                    string n = "";
                    foreach (var obj in myLISTfull)
                    {
                        n = "";
                       if (text_title == @"#EXTM3U $BorpasFileFormat="+'"'+'1'+'"') n += "#EXTINF:-1";
                       else n += "#EXTINF:0";

                        if (obj.ExtFilter != "") n += " $ExtFilter=" + '"' + obj.ExtFilter + '"';
                        if (obj.group_title != "") n += " group-title=" + '"' + obj.group_title + '"';
                        if (obj.logo != "") n+= " tvg-logo=" + '"' + obj.logo + '"';
                        if (obj.tvg_name != "") n += " tvg-name=" + '"' + obj.tvg_name + '"';

                        n += "," +obj.name + '\n';
                        sr.Write(n+ obj.http + '\n');
                    } 
                }

            }
        }

        void key_FILTER(object parameter)
        {
            filter = data.f1;
            filter2 = data.f2;
            filter3 = data.f3;
            
            UPDATE_FILTER("");
            RaisePropertyChanged("mycol");
           
        }


        void key_OPEN(object parameter)
        {
            string line = null;
            string newname = "";

            CollectionisCreate();


            try {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    Regex regex1 = new Regex("#EXTINF");
                    Regex regex2 = new Regex("#EXTM3U");
                    Match match = null;




                    using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                    {


                        string header = "";



                        while (true)
                        {
                            try
                            {
                                header = sr.ReadLine();
                            }

                            catch
                            {

                                //DialogWindow dialog = new DialogWindow();
                                //if (dialog.ShowDialog() == true)
                                //{
                                //    // Пользователь разрешил действие. Продолжить1
                                //}
                                //else
                                //{
                                //    // Пользователь отменил действие.
                                //}

                                MessageBox.Show("Ошибка файла", "заголовок с ошибкой",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            }



                            if (header == null || sr.EndOfStream) break;

                            match = regex2.Match(header);
                            if (match.Success) break;


                        }



                        if (!match.Success)
                        {

                            MessageBox.Show("заголовок с ошибкой", "Ошибка файла",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                           
                        }
                        //------------------------------------------------------------
                        text_title = header;

                        string str_ex = "", str_name = "", str_http = "", str_gt = "", str_logo = "", str_tvg = "";

                        bool yes = false;

                       

                        while (!sr.EndOfStream)
                        {


                            try
                            {
                                line = sr.ReadLine();
                            }
                            catch { }

                            if (line == null || line == "") continue;


                            match = regex1.Match(line);
                            if (match.Success) yes = true; else yes = false;



                            //while (match.Success)
                            //{
                            //    // Т.к. мы выделили в шаблоне одну группу (одни круглые скобки),
                            //    // ссылаемся на найденное значение через свойство Groups класса Match
                            //    //Console.WriteLine(match.Groups[1].Value);
                            //    y = true; break;
                            //    // Переходим к следующему совпадению
                            //    //match = match.NextMatch();
                            //}



                            //}



                            ///========== разбор EXINF
                            if (yes)
                            {

                                Regex regex3 = new Regex("ExtFilter=", RegexOptions.IgnoreCase);
                                Regex regex4 = new Regex("group-title=", RegexOptions.IgnoreCase);
                                Regex regex5 = new Regex("logo=", RegexOptions.IgnoreCase);
                                Regex regex6 = new Regex("tvg-name=", RegexOptions.IgnoreCase);




                                //string[] split = line.Split(new Char[] { ' ', ',', '.', ':', '"' });
                                string[] split = line.Split(new Char[] { '"' });


                                str_ex = ""; str_name = ""; str_http = ""; str_gt = ""; str_logo = ""; str_tvg = "";

                                if (split.Length <= 1)
                                {
                                    split = line.Split(new Char[] { ',' });
                                    str_name = split[split.Length - 1];
                                    newname = str_name;
                                }
                                else
                                {


                                    int i = 0;
                                    for (i = 0; i < split.Length; i++)
                                    {
                                        string s = split[i];
                                        //------------- разбор строки EXINF
                                        if (str_ex == "!") str_ex = s;
                                        if (str_gt == "!") str_gt = s;
                                        if (str_logo == "!") str_logo = s;
                                        if (str_tvg == "!") str_tvg = s;


                                        if (i >= split.Length - 1) { str_name = s; }





                                        match = regex3.Match(s);
                                        if (match.Success)
                                        {
                                            str_ex = "!";
                                        }

                                        match = regex4.Match(s);
                                        if (match.Success)
                                        {
                                            str_gt = "!";
                                        }

                                        match = regex5.Match(s);
                                        if (match.Success)
                                        {
                                            str_logo = "!";
                                        }

                                        match = regex6.Match(s);
                                        if (match.Success)
                                        {
                                            str_tvg = "!";
                                        }
                                    }//foreach

                                    newname = "";
                                    if (str_name != "") newname = str_name.Remove(0, 1);

                                }

                                //========= http =============
                                str_http = sr.ReadLine();
                                //Regex regex100 = new Regex("://");

                                //match = regex100.Match(str_http);
                                //if (!match.Success)
                                //{
                                //    str_http = "error "+ str_http;
                                //}


                                //match = regex4.Match(line);
                                //if (match.Success)
                                //{
                                //    words1 = line.Split(new char[] { '"' });
                                //    if (str_ex != "")
                                //    {
                                //        if (words1.Length > 3) str_gr = words1[3];
                                //        else if (words1.Length > 2) str_gr = words1[2];
                                //    }
                                //    else str_gr = words1[1];
                                //}

                                //if (line != null) words = line.Split(new char[] { ',' });


                                //bool y = false;
                                //while (1 == 1)
                                //{
                                //    // Read the stream to a string, and write the string to the console.
                                //    http0 = sr.ReadLine();
                                //    // if (sr.EndOfStream) break;

                                //    if (http0 == null) continue;
                                //    foreach (var c in http0)
                                //    {
                                //        if (char.IsPunctuation(c)) { y = true; break; }
                                //        // else if (IsLatin(c)) { y = true; break; }
                                //    }
                                //    if (y) break;
                                //}


                            }


                          


                            ViewModelMain.myLISTfull.Add(new ParamCanal
                            {
                                name = newname.Trim(),
                                ExtFilter = str_ex.Trim(),
                                http = str_http.Trim(),
                                group_title = str_gt.Trim(),
                                logo = str_logo.Trim(),
                                tvg_name = str_tvg.Trim()

                            });

                        }///чтение фала







                    }// string name = File.ReadAllText(openFileDialog.FileName);

                }

                RaisePropertyChanged("mycol");///updte LIST!!
                RaisePropertyChanged("numberCANALS");
            }


            catch { }
        }
    



                }//class

}//namespace
