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

        public RelayCommand key_SORTCommand { get; set; }
        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand key_OPENCommand { get; set; }
        public RelayCommand key_SAVECommand { get; set; }
        public RelayCommand key_delCommand { get; set; }

        public RelayCommand key_FILTERCommand { get; set; }

        void ini_command()
        {
            key_SORTCommand = new RelayCommand(key_SORT);
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
            if (parameter == null) return;

            RaisePropertyChanged("numberCANALS");
        }



        void key_SORT(object parameter)
        {
            // ascending
            //collection = new ObservableCollection<int>(collection.OrderBy(a => a));

            //// descending
            //collection = new ObservableCollection<int>(collection.OrderByDescending(a => a));



            //  ObservableCollection<string> _animals = new ObservableCollection<string>()
            //{ "Cat", "Dog", "Bear", "Lion", "Mouse",
            //"Horse", "Rat", "Elephant", "Kangaroo", "Lizard",
            //"Snake", "Frog", "Fish", "Butterfly", "Human",
            //"Cow", "Bumble Bee"};

            //_animals = new ObservableCollection<string>(_animals.OrderBy(i => i));

            //Canal = new ObservableCollection<ParamCanal>(Canal.OrderBy(a => a.name));

        }





        void key_SAVE(object parameter)
        {

            SaveFileDialog openFileDialog = new SaveFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamWriter sr = new StreamWriter(openFileDialog.FileName))
                {
                    //foreach (var e in Canal)
                    //{
                    //    sr.Write(e.ExtFilter + '\n' + e.http + '\n');

                    //}
                }// string name = File.ReadAllText(openFileDialog.FileName);

            }
        }

        void key_FILTER(object parameter)
        {
            filter = parameter.ToString();
            UPDATE_FILTER();
            RaisePropertyChanged("mycol");
           
        }


        void key_OPEN(object parameter)
        {
            string line = null, http0 = null;
            uint ct = 0;

            CollectionisCreate();



                        OpenFileDialog openFileDialog = new OpenFileDialog();

                        if (openFileDialog.ShowDialog() == true)
                        {
                            Regex regex1 = new Regex("#EXTINF");
                            Regex regex2 = new Regex("#EXTM3U");
                            Match match = null;
                            bool badtitle = false;



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
                                        badtitle = true;
                                        MessageBox.Show("Ошибка файла", "заголовок с ошибкой",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                                    }



                                    if (header == null || sr.EndOfStream) break;

                                    match = regex2.Match(header);
                                    if (match.Success) break;


                                }



                                if (!match.Success)
                                {
                                    badtitle = true;
                                    MessageBox.Show("заголовок с ошибкой", "Ошибка файла",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                //------------------------------------------------------------
                                text_title = header;
                                string str_ex = "";
                                string str_gr = "";

                                bool yes = false;
                                byte ct00 = 0;

                                while (!sr.EndOfStream && ct00 < 100)
                                {


                                    try
                                    {
                                        line = sr.ReadLine();
                                    }
                                    catch { }

                                    ct00++;
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

                                        Regex regex3 = new Regex("ExtFilter=");
                                        Regex regex4 = new Regex("group-title=", RegexOptions.IgnoreCase);
                                        Regex regex5 = new Regex("tvg-logo=", RegexOptions.IgnoreCase);


                                        str_gr = "vv";
                                        str_ex = "vv";


                                        //string[] split = line.Split(new Char[] { ' ', ',', '.', ':', '"' });
                                        string[] split = line.Split(new Char[] { ' ' });


                            if (split.Length > 3)
                            {
                                str_ex = split[2];
                                str_gr = split[3];

                                foreach (string s in split)
                                {

                                    match = regex4.Match(s);
                                    if (match.Success)
                                    {



                                        int f = s.LastIndexOf("lter=");
                                        if (f != -1)
                                        {
                                            // обрезаем сначала до последних двух символов
                                            // string ss = s.Substring(0, f);

                                            //charsToTrim = { '*', ' ', '\'' };
                                            str_ex = s;
                                        }

                                    }



                                }//foreach

                            }




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

                                    ViewModelMain.myLISTfull.Add(new ParamCanal { name = str_ex, ExtFilter = "f", http = "vcnbn", group_title ="hsfdgfh" });



                                }///чтение фала







                            }// string name = File.ReadAllText(openFileDialog.FileName);

                        }

                        RaisePropertyChanged("mycol");///updte LIST!!
                        RaisePropertyChanged("numberCANALS");
                    }







                }//class

}//namespace
