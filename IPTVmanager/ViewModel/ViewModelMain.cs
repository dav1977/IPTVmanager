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
                return "Всего каналов: "+Canal.Count.ToString();
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
                //new ParamCanal { FirstName="Tom1", LastName="Jones", Age=80 },
                //new ParamCanal { FirstName="Dick", LastName="Tracey", Age=40 },
                //new ParamCanal { FirstName="Harry", LastName="Hill", Age=60 },
                //new ParamCanal { FirstName="param4", LastName="Lastp4", Age=99 },
            };
            
            //ParamCanal p = new ParamCanal { FirstName = "wirte1", LastName = "Jones", Age = 80 };
            //Canal.Add(p);

            //p = new ParamCanal { FirstName = "wirte2", param3 = "iik", Age = 99 };
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
            Canal.Add(new ParamCanal { FirstName = parameter.ToString(), LastName = parameter.ToString(), Age = DateTime.Now.Second });
            RaisePropertyChanged("numberCANALS");
        }











        void key_del(object parameter)
        {
            if (parameter == null) return;
 
            Canal.Remove(new ParamCanal { FirstName = parameter.ToString(), LastName = parameter.ToString(), Age = DateTime.Now.Second });
            RaisePropertyChanged("numberCANALS");
        }

        void key_OPEN(object parameter)
        {
            uint ct = 0;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    String header = sr.ReadLine();

                    while (!sr.EndOfStream && ct<200)
                    {
                        // Read the stream to a string, and write the string to the console.
                        String line = sr.ReadLine();
                        string[] words = line.Split(new char[] { ',' });
                        String http0 = sr.ReadLine();
                        ct++;
                        Canal.Add(new ParamCanal { FirstName = words[1], LastName = line, http=http0, Age = DateTime.Now.Second });
                      
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

            Canal = new ObservableCollection<ParamCanal>(Canal.OrderBy(a => a.FirstName));

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
                        sr.Write(e.LastName+'\n'+e.http + '\n');
                       
                    }
                }// string name = File.ReadAllText(openFileDialog.FileName);

            }
        }



    }
}
