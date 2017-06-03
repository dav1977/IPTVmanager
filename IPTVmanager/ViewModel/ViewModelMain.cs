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
   
    public delegate void Delegate_UpdateMOVE( ParamCanal a );

    public class MultiValueConverter : IMultiValueConverter     //  http://www.codearsenal.net/2013/12/wpf-multibinding-example.html
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string one = values[0] as string;
            string two = values[1] as string;
            string three = values[2] as string;
            if (!string.IsNullOrEmpty(one) && !string.IsNullOrEmpty(two) && !string.IsNullOrEmpty(three))
            {
                return one + two + three;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

  

    partial class ViewModelMain : ViewModelBase
    {
        public static event Delegate_WIN_WAIT Event_WIN_WAIT;
        public static event Delegate_UpdateMOVE Event_UpdateAFTERmove;
        public static event Delegate_UpdateALL Event_UpdateLIST;
        public static event Delegate_SelectITEM Event_SelectITEM;
    

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
            ListViewDragDropManager.WindowMOVE.Event_UpdateAFTERmove += new Delegate_UpdateMOVE(updateLIST);
            ViewModelWindow2.Event_UpdateAFTERmove += new Delegate_UpdateMOVE(updateLIST);


            ViewModelWindow1.Event_UpdateEDIT += new Delegate_UpdateEDIT(updateEDIT);
            ViewModelWindow1.Event_ADDBEST += new Delegate_ADDBEST(BEST_ADD);
            newChannel = "новое значение";
            ini_command();
            CreateTimer1(500);
        }


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

       
        void BEST_ADD()
        {
            if (data.one_add) return;
            data.one_add = true;
            data.canal.ExtFilter = data.best1;
            data.canal.group_title = data.best2;
            myLISTfull.Add(data.canal);
            RaisePropertyChanged("mycol");
        }



        void updateEDIT()
        {
            int i = 0;
            foreach (var obj in myLISTfull)
            {
               
                if (obj.name == data.canal.name && obj.http == data.canal.http && obj.ExtFilter == data.canal.ExtFilter)
                {
                   myLISTfull[i].name = data.edit.name;
                   myLISTfull[i].ExtFilter = data.edit.ExtFilter;
                   myLISTfull[i].group_title = data.edit.group_title;
                   myLISTfull[i].http = data.edit.http;
                   myLISTfull[i].ping = data.edit.ping;
                   break;
                }
                i++;
            }

            UPDATE_FILTER("");
            RaisePropertyChanged("mycol");///update LIST!!
        }


        void updateLIST(ParamCanal item)
        {
      
            UPDATE_FILTER("");
            RaisePropertyChanged("mycol");///update LIST!!
        }




        void UPD1()
        {

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

                    if (m2.Success && data.f2 != "") myLISTbase.Add(c);
                    else
                    {
                        m3 = regex3.Match(c.group_title);
                        if (m1.Success && data.f1 != "" && m2.Success && data.f2 != "" &&
                            m3.Success && data.f3 != "") myLISTbase.Add(c);
                    }

                    //----------------------------------
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



        void UPD2best()
        {
            data.f1 = ""; data.f4 = ""; data.f2 = data.best1; data.f3 = data.best2;

            Match m1;
            Regex regex1 = new Regex(data.f1, RegexOptions.IgnoreCase);



            myLISTbase.Clear();
            foreach (var c in myLISTfull)
            {
                m1 = regex1.Match(c.name);
                if ((m1.Success && data.f2 == c.ExtFilter && data.f3 == c.group_title)) myLISTbase.Add(c);


            }

        }

        public void UPDATE_FILTER(string par)
        {

            if (data.f1 == null) data.f1 = "";
            if (data.f2 == null) data.f2 = "";
            if (data.f3 == null) data.f3 = "";
            if (data.f4 == null) data.f4 = "";

            if (myLISTfull != null && myLISTbase != null)
            {
                if (!filtr_best) {  UPD1(); }
                else
                {

                    UPD2best();
                }
            }

        }

        public void UPDATE_BEST(string b1, string b2)
        {
            if (b1!=null)data.best1 = b1;
            if (b2!=null)data.best2 = b2;
         
        }


    }//class
}//namespace
