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

    //пользовательский делегат
    delegate void Delegate_UpdateALL(int size);

    partial class ViewModelMain : ViewModelBase
    {
        public static event Delegate_UpdateALL Event_UpdateLIST;

        //public ObservableCollection<ParamCanal> Canal { get; set; }
        //  public MyCollection<ParamCanal> CanalOUT { get; set; }

        public static List<ParamCanal> myLIST;//ВРЕМЕННАЯ ДЛЯ ВЫВОДА НА ЭКРАН

        public static List<ParamCanal> myLISTbase;//ПОСЛЕ ФИЛЬТРА

        public static List<ParamCanal> myLISTfull;//ДО ФИЛЬТРА


       // public static ParamCanal list;
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

       
        public CollectionProvider myProvider;
        public AsyncVirtualizingCollection<ParamCanal> ACOLL;


        private void Create_Virtual_Collection()
            {
            int numItems=100000;
            int fetchDelay = 1;// 
            myProvider = new CollectionProvider(numItems, fetchDelay);

            myLISTbase = new List<ParamCanal>();//после фильтра
            myLISTfull = new List<ParamCanal>();

            // create the collection according to specified parameters
            int pageSize = 100;// размер страницы
            int pageTimeout = 1000;//мс ВРЕМЯ ЖИЗНИ ВРЕМЕННОЙ СТРАНИЦЫ после неиспользования
            ACOLL = new AsyncVirtualizingCollection<ParamCanal>(myProvider, pageSize, pageTimeout);

        }


        bool one_open = false;

        void CollectionisCreate()
        {
            
            if (!one_open) { Create_Virtual_Collection(); one_open = true; }

        }

        public ViewModelMain()
        {
            //Canal = new ObservableCollection<ParamCanal>
            //{
            //    //new ParamCanal { Title="Tom1", ExtFilter="Jones", group_title=80 },
            //    //new ParamCanal { Title="Dick", ExtFilter="Tracey", group_title=40 },
            //    //new ParamCanal { Title="Harry", ExtFilter="Hill", group_title=60 },
            //    //new ParamCanal { Title="param4", ExtFilter="Lastp4", group_title=99 },
            //};
            
            //ParamCanal p = new ParamCanal { Title = "wirte1", ExtFilter = "Jones", group_title = 80 };
            //Canal.Add(p);

            //p = new ParamCanal { Title = "wirte2", param3 = "iik", group_title = 99 };
            //Canal.Add(p);

           


            newChannel = "новое значение";
            ini_command();
            CreateTimer1(500);
        }


        public void UPDATE_FILTER(string f)
        {

            if (myLISTfull != null && myLISTbase != null)
            {


                Match match;
                Regex regex1 = new Regex(ViewModelMain._filter, RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace);
                myLISTbase.Clear();
                foreach (var c in myLISTfull)
                {
                    // Trace.WriteLine("z = " + ViewModelMain._filter + "n="+ c.name + " ");
                   match = regex1.Match(c.name);
                   if (match.Success || ViewModelMain._filter == "") myLISTbase.Add(c);  

                }
            }

        }



    }//class
}//namespace
