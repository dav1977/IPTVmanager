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
    class ViewModelMain : ViewModelBase
    {
        public ObservableCollection<Person> People { get; set; }

        /// <summary>
        /// SelectedItem is an object instead of a Person, only because we are allowing "CanUserAddRows=true" 
        /// NewItemPlaceHolder represents a new row, and this is not the same as Person class
        /// 
        /// Change 'object' to 'Person', and you will see the following:
        /// 
        /// System.Windows.Data Error: 23 : Cannot convert '{NewItemPlaceholder}' from type 'NamedObject' to type 'IPTVman.Model.Person' for 'en-US' culture with default conversions; consider using Converter property of Binding. NotSupportedException:'System.NotSupportedException: TypeConverter cannot convert from MS.Internal.NamedObject.
        ///   at System.ComponentModel.TypeConverter.GetConvertFromException(Object value)
        ///   at System.ComponentModel.TypeConverter.ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        ///   at MS.Internal.Data.DefaultValueConverter.ConvertHelper(Object o, Type destinationType, DependencyObject targetElement, CultureInfo culture, Boolean isForward)'
        ///System.Windows.Data Error: 7 : ConvertBack cannot convert value '{NewItemPlaceholder}' (type 'NamedObject'). BindingExpression:Path=SelectedPerson; DataItem='ViewModelMain' (HashCode=47403907); target element is 'DataGrid' (Name=''); target property is 'SelectedItem' (type 'Object') NotSupportedException:'System.NotSupportedException: TypeConverter cannot convert from MS.Internal.NamedObject.
        ///   at MS.Internal.Data.DefaultValueConverter.ConvertHelper(Object o, Type destinationType, DependencyObject targetElement, CultureInfo culture, Boolean isForward)
        ///   at MS.Internal.Data.ObjectTargetConverter.ConvertBack(Object o, Type type, Object parameter, CultureInfo culture)
        ///   at System.Windows.Data.BindingExpression.ConvertBackHelper(IValueConverter converter, Object value, Type sourceType, Object parameter, CultureInfo culture)'
        /// </summary>

        object _SelectedPerson;
        public object SelectedPerson
        {
            get
            {
                return _SelectedPerson;
            }
            set
            {
                if (_SelectedPerson != value)
                {
                    _SelectedPerson = value;
                    RaisePropertyChanged("SelectedPerson");
                }
            }
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

        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand key_OPENCommand { get; set; }
        public RelayCommand key_delCommand { get; set; }

        /// <summary>
        /// *************************** MAIN *********************************
        /// </summary>
        public ViewModelMain()
        {
            People = new ObservableCollection<Person>
            {
                //new Person { FirstName="Tom1", LastName="Jones", Age=80 },
                //new Person { FirstName="Dick", LastName="Tracey", Age=40 },
                //new Person { FirstName="Harry", LastName="Hill", Age=60 },
                //new Person { FirstName="param4", LastName="Lastp4", Age=99 },
            };

            //Person p = new Person { FirstName = "wirte1", LastName = "Jones", Age = 80 };
            //People.Add(p);

            //p = new Person { FirstName = "wirte2", param3 = "iik", Age = 99 };
            //People.Add(p);




            TextProperty1 = "новое значение";

            key_ADDCommand = new RelayCommand(key_ADD);
            key_OPENCommand = new RelayCommand(key_OPEN);
            key_delCommand = new RelayCommand(key_del);
        }

        void key_ADD(object parameter)
        {
            if (parameter == null) return;
            People.Add(new Person { FirstName = parameter.ToString(), LastName = parameter.ToString(), Age = DateTime.Now.Second });
        }

        void key_del(object parameter)
        {
            if (parameter == null) return;
 
            People.Remove(new Person { FirstName = parameter.ToString(), LastName = parameter.ToString(), Age = DateTime.Now.Second });
        }

        void key_OPEN(object parameter)
        {
            uint ct = 0;
            if (parameter == null) return;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                {
                    while (!sr.EndOfStream && ct<50)
                    {
                        // Read the stream to a string, and write the string to the console.
                        String line = sr.ReadLine();
                        string[] words = line.Split(new char[] { ',' });
                        String http0 = sr.ReadLine();
                        ct++;
                        People.Add(new Person { FirstName = words[1], LastName = line, http=http0, Age = DateTime.Now.Second });
                      
                    }
                }// string name = File.ReadAllText(openFileDialog.FileName);

            }
        }


       
    }
}
