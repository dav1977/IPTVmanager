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

namespace IPTVman.ViewModel
{
    //class MyCollection<T> : ObservableCollection<T>
    //{
    //    public MyCollection(object d)
    //    {
            
    //    }
    //    public void Sort(Comparison<T> comparison)
    //    {
    //        // Не лучший вариант, т.к. код зависит от детали реализации свойства Items.
    //        // Вместо приведения типов лучше реализовать свой любимый алгоритм сортировки для IList<T>.
    //        var items = this.Items as List<T>;

    //        items.Sort(comparison);

    //        //OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //    }
    //}



    class ViewModelMain : ViewModelBase
    {
        public ObservableCollection<ParamCanal> Canal { get; set; }
        //  public MyCollection<ParamCanal> CanalOUT { get; set; }
        public static List<ParamCanal> myLIST;

        public static List<ParamCanal> myLISTbase;

        public static  bool load_ok =false;
        /// <summary>
        /// SelectedItem is an object instead of a ParamCanal, only because we are allowing "CanUserAddRows=true" 
        /// NewItemPlaceHolder represents a new row, and this is not the same as ParamCanal class
        /// 
        /// Change 'object' to 'ParamCanal', and you will see the following:
        /// 
        /// System.Windows.Data Error: 23 : Cannot convert '{NewItemPlaceholder}' from type 'NamedObject' to type 'IPTVman.Model.ParamCanal' for 'en-US' culture with default conversions; consider using Converter property of Binding. NotSupportedException:'System.NotSupportedException: TypeConverter cannot convert from MS.Internal.NamedObject.
        ///   at System.ComponentModel.TypeConverter.GetConvertFromException(Object value)
        ///   at System.ComponentModel.TypeConverter.ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        ///   at MS.Internal.Data.DefaultValueConverter.ConvertHelper(Object o, Type destinationType, DependencyObject targetElement, CultureInfo culture, Boolean isForward)'
        ///System.Windows.Data Error: 7 : ConvertBack cannot convert value '{NewItemPlaceholder}' (type 'NamedObject'). BindingExpression:Path=SelectedParamCanal; DataItem='ViewModelMain' (HashCode=47403907); target element is 'DataGrid' (Name=''); target property is 'SelectedItem' (type 'Object') NotSupportedException:'System.NotSupportedException: TypeConverter cannot convert from MS.Internal.NamedObject.
        ///   at MS.Internal.Data.DefaultValueConverter.ConvertHelper(Object o, Type destinationType, DependencyObject targetElement, CultureInfo culture, Boolean isForward)
        ///   at MS.Internal.Data.ObjectTargetConverter.ConvertBack(Object o, Type type, Object parameter, CultureInfo culture)
        ///   at System.Windows.Data.BindingExpression.ConvertBackHelper(IValueConverter converter, Object value, Type sourceType, Object parameter, CultureInfo culture)'
        /// </summary>

        object _SelectedParamCanal;
        public object SelectedParamCanal
        {
            get
            {
                return _SelectedParamCanal;
            }
            set
            {
                if (_SelectedParamCanal != value)
                {
                    _SelectedParamCanal = value;
                    
                    RaisePropertyChanged("SelectedParamCanal");
           
                }
            }
        }

        public object numberCANALS
        {
            get
            {
                return "Всего каналов: "+ myLIST.Count.ToString();
            }
            //set
            //{
            //    if (_numberCANALS != value)
            //    {
            //        _numberCANALS = value;
            //        RaisePropertyChanged("numberCANALS");

            //    }
            //}
        }
          

        string _TextProperty1;
        public string TextProperty1
        {
            get
            {
                return _TextProperty1;
            }
            set
            {
                if (_TextProperty1 != value)
                {
                    _TextProperty1 = value;
                    RaisePropertyChanged("TextProperty1");
                }
            }
        }


        string _text_title;
        public string text_title
        {
            get
            {
                return _text_title;
            }
            set
            {
                if (_text_title != value)
                {
                    _text_title = value;
                    RaisePropertyChanged("text_title");
                }
            }
        }



        public RelayCommand key_SORTCommand { get; set; }
        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand key_OPENCommand { get; set; }
        public RelayCommand key_SAVECommand { get; set; }
        public RelayCommand key_delCommand { get; set; }

        /// <summary>
        /// *************************** MAIN *********************************
        /// </summary>
        public ViewModelMain()
        {
            Canal = new ObservableCollection<ParamCanal>
            {
                //new ParamCanal { Title="Tom1", ExtFilter="Jones", group_title=80 },
                //new ParamCanal { Title="Dick", ExtFilter="Tracey", group_title=40 },
                //new ParamCanal { Title="Harry", ExtFilter="Hill", group_title=60 },
                //new ParamCanal { Title="param4", ExtFilter="Lastp4", group_title=99 },
            };
            
            //ParamCanal p = new ParamCanal { Title = "wirte1", ExtFilter = "Jones", group_title = 80 };
            //Canal.Add(p);

            //p = new ParamCanal { Title = "wirte2", param3 = "iik", group_title = 99 };
            //Canal.Add(p);




            TextProperty1 = "новое значение";

            key_SORTCommand = new RelayCommand(key_SORT);
            key_ADDCommand = new RelayCommand(key_ADD);
            key_OPENCommand = new RelayCommand(key_OPEN);
            key_SAVECommand = new RelayCommand(key_SAVE);
            key_delCommand = new RelayCommand(key_del);
        }




        //******************************* ADD **************
        void key_ADD(object parameter)
        {
            if (parameter == null) return;
            Canal.Add(new ParamCanal { name = parameter.ToString(), ExtFilter = parameter.ToString(), group_title = "" });
            RaisePropertyChanged("numberCANALS");
        }











        void key_del(object parameter)
        {
            if (parameter == null) return;
 
            Canal.Remove(new ParamCanal { name = parameter.ToString(), ExtFilter = parameter.ToString(), group_title = "" });
            RaisePropertyChanged("numberCANALS");
        }


        void key_OPEN(object parameter)
        {
            string line=null, http0=null;
            uint ct = 0;
       


            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                Regex regex1 = new Regex("#EXTINF");
                Regex regex2 = new Regex("#EXTM3U");
                Match match;
                bool badtitle = false;

                Regex regex3 = new Regex("ExtFilter=");
                Regex regex4 = new Regex("group-title=");
                string[] words1 = { "", "" };
                string[] words = { "", "" };
                string str_ex="";
                string str_gr = "";

                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    string header = sr.ReadLine();
                    match = regex2.Match(header);
                    if (!match.Success) badtitle = true;

                    text_title = header;

                    while (!sr.EndOfStream /* && ct<400*/)
                    {
                        while (true)
                        {
                            if (!badtitle)  line = sr.ReadLine();  else badtitle = false;
                            if (sr.EndOfStream) break;

                            if (line == null) continue;

                            match = regex1.Match(line);
                            if (match.Success) break;

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
                           
                        }
                        str_gr = "";
                        str_ex = "";

                        match = regex3.Match(line);
                        if (match.Success)
                        {
                            words1 = line.Split(new char[] { '"' });
                            str_ex = words1[1];
                           
                        }

                        match = regex4.Match(line);
                        if (match.Success)
                        {
                            words1 = line.Split(new char[] { '"' });
                            if (str_ex != "")
                            {
                                if (words1.Length > 3) str_gr = words1[3];
                                else if (words1.Length > 2) str_gr = words1[2];
                            }
                            else str_gr = words1[1];
                        }

                        if (line != null) words = line.Split(new char[] { ',' });


                        bool y = false;
                        while (1 == 1)
                        {
                            // Read the stream to a string, and write the string to the console.
                            http0 = sr.ReadLine();
                           // if (sr.EndOfStream) break;

                            if (http0 == null) continue;
                            foreach (var c in http0)
                            {
                                if (char.IsPunctuation(c)) { y = true; break; }
                               // else if (IsLatin(c)) { y = true; break; }
                            }
                            if (y) break;
                        }



                     



                        ct++;
                        Canal.Add(new ParamCanal { name = words[1], ExtFilter = str_ex, http = http0, group_title = str_gr });


                        
                    }
                }// string name = File.ReadAllText(openFileDialog.FileName);

            }
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

            Canal = new ObservableCollection<ParamCanal>(Canal.OrderBy(a => a.name));

        }





        void key_SAVE(object parameter)
        {
           
            SaveFileDialog openFileDialog = new SaveFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamWriter sr = new StreamWriter(openFileDialog.FileName))
                {
                    foreach (var e in Canal)
                    {
                        sr.Write(e.ExtFilter+'\n'+e.http + '\n');
                       
                    }
                }// string name = File.ReadAllText(openFileDialog.FileName);

            }
        }



    }
}
