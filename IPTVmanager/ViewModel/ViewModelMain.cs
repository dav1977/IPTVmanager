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
using System.Windows.Data;
using System.Threading.Tasks;


namespace IPTVman.ViewModel
{

    public delegate void Delegate_UpdateCollection(ParamCanal a);

    //public class MultiValueConverter : IMultiValueConverter     //  http://www.codearsenal.net/2013/12/wpf-multibinding-example.html
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        string one = values[0] as string;
    //        string two = values[1] as string;
    //        string three = values[2] as string;
    //        if (!string.IsNullOrEmpty(one) && !string.IsNullOrEmpty(two) && !string.IsNullOrEmpty(three))
    //        {
    //            return one + two + three;
    //        }
    //        return null;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}



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

        //**********************************************************
        // INIT
        //**********************************************************
        public ViewModelMain()
        {
            ViewModelWindow1.Event_UpdateEDIT += new Delegate_UpdateEDIT(updateEDIT);
            ViewModelWindow1.Event_ADDBEST += new Delegate_ADDBEST(BEST_ADD);
            ViewModelWindow2.Event_UpdateCollection += new Delegate_UpdateCollection(updateLIST);
            ListViewDragDropManager.WindowMOVE.Event_UpdateCollection += new Delegate_UpdateCollection(updateLIST);
            ViewModelWindowReplace.Event_UpdateCollection += new Delegate_UpdateCollection(updateLIST);
            WindowPING.Event_Refresh += new Delegate_UpdateCollection(updateLIST);


            ini_command();
            CreateTimer1(500);

            
        }



        private void Create_Virtual_Collection()
            {
            int numItems=1000000;
            int fetchDelay = 1;// 
            myProvider = new CollectionProvider(numItems, fetchDelay);

            myLISTbase = new List<ParamCanal>();//после фильтра
            myLISTfull = new List<ParamCanal>();

            // create the collection according to specified parameters
            int pageSize = 100;// размер страницы
            int pageTimeout = 1000;//мс ВРЕМЯ ЖИЗНИ ВРЕМЕННОЙ СТРАНИЦЫ после неиспользования
            ACOLL = new AsyncVirtualizingCollection<ParamCanal>(myProvider, pageSize, pageTimeout);

        }


       
        void CollectionisCreate()
        {
            if (myLISTfull==null) { Create_Virtual_Collection();  }
        }

       
        void BEST_ADD()
        {
            if (loc.keyadd) return;
            loc.keyadd = true;
            data.canal.ExtFilter = data.best1;
            data.canal.group_title = data.best2;
            myLISTfull.Add(data.canal);
            Update_collection(); 
        }


        void updateEDIT(ParamCanal item)
        {
            int i = 0;
  
            if (GUI.edit) return;
            GUI.edit = true;
            foreach (var obj in myLISTfull)
            {
               if (obj.Compare()==data.canal.Compare())
                {
                    myLISTfull[i] = (ParamCanal)item.Clone();
                    break;
                }
                i++;
            }

            Update_collection();
        }


        void updateLIST(ParamCanal item)
        {
            Update_collection();
        }


        private object threadLock = new object();
        public void Update_collection()
        {
            lock (threadLock)
            {
                UPDATE_FILTER();
                RaisePropertyChanged("mycol");///update LIST!!
                RaisePropertyChanged("numberCANALS");
            }
        }


        void UPD_normal()
        {
            if (Wait.WaitIsOpen()) return;

            Match m1, m2, m3, m4;
            Regex regex1 = new Regex(data.f1, RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(data.f2, RegexOptions.IgnoreCase);
            Regex regex3 = new Regex(data.f3, RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(data.f4);


            myLISTbase.Clear();
            foreach (var c in myLISTfull)
            {
                //Trace.WriteLine("z = " + ViewModelMain._filter + "n="+ c.name + " ");


                bool not_filter = false;
                if (data.f1 == "" && data.f2 == "" && data.f3 == "" && data.f4 == "") not_filter = true;

                if (not_filter) { myLISTbase.Add(c); continue; }


                m1 = regex1.Match(c.name);
                if ((m1.Success && data.f2 == "" && data.f3 == "" && data.f4 == "")) myLISTbase.Add(c);

                else
                {
                    //----------------------------------
                    m2 = regex2.Match(c.ExtFilter);

                    if (m2.Success && data.f2 != "" && data.f3 == "") myLISTbase.Add(c);
                    //----------------------------------


                    //----------------------------------
                    m3 = regex3.Match(c.group_title);

                    if (m3.Success && data.f3 != "" && data.f2 == "") myLISTbase.Add(c);
                    //----------------------------------

                    if (m3.Success && data.f3 != "" && m2.Success && data.f2 != "") myLISTbase.Add(c);

                }

                if (data.f4 != "" && (m1.Success || data.f1 == ""))
                {

                    //all
                    if (c.http != null)
                    {
                        m4 = regex4.Match(c.http);
                        if ((m4.Success)) myLISTbase.Add(c);
                    }
                    if (c.ping != null)
                    {
                        m4 = regex4.Match(c.ping);
                        if ((m4.Success)) myLISTbase.Add(c);
                    }
                    if (c.tvg_name != null)
                    {
                        m4 = regex4.Match(c.tvg_name);
                        if ((m4.Success)) myLISTbase.Add(c);
                    }


                }

            }
        }



        void UPD_best()
        {
            if (Wait.WaitIsOpen()) return;

            data.f2 = data.best1;
            data.f3 = data.best2;

            Match m1;
            Regex regex1 = new Regex(data.f1, RegexOptions.IgnoreCase);

            myLISTbase.Clear();

            foreach (var c in myLISTfull)
            {
                m1 = regex1.Match(c.name);


                if( ( //(m1.Success && data.f2 == c.ExtFilter && data.f3 == c.group_title)
                    (data.best1== c.ExtFilter && data.best2 == c.group_title)  ||
                    (data.best1 == c.ExtFilter && data.best2 == "") ||
                    (data.best1 == "" && data.best2 == c.group_title)
                    )  && (data.f1=="" || m1.Success ))
                        myLISTbase.Add(c);
            }

        }

        public void UPDATE_FILTER()
        {
            if (Wait.WaitIsOpen()) return;

            if (data.f1 == null) data.f1 = "";
            if (data.f2 == null) data.f2 = "";
            if (data.f3 == null) data.f3 = "";
            if (data.f4 == null) data.f4 = "";

            if (myLISTfull != null && myLISTbase != null)
            {
                if (!data.filtr_best) { UPD_normal(); } else UPD_best();
            }

        }

        public void UPDATE_BEST(string b1, string b2)
        {
            if (b1!=null)data.best1 = b1;
            if (b2!=null)data.best2 = b2;
         
        }


    }//class
}//namespace
