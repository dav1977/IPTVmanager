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
        public static event Action<string> EVENT_ADD;
        public static event Action EVENT_CLOSE_ALL;
        public static event Action EVENT_OPENWIN_UpdateDB;
        public static event Action EVENT_OPENWIN_Radio;
        public static event Action<int> Event_UpdateLIST;

        Update_Collection _update = new Update_Collection();

        //public ObservableCollection<ParamCanal> Canal { get; set; }
        //  public MyCollection<ParamCanal> CanalOUT { get; set; }

        public static List<ParamCanal> myLIST;//ВРЕМЕННАЯ ДЛЯ ВЫВОДА НА ЭКРАН

        public static List<ParamCanal> myLISTbase;//ПОСЛЕ ФИЛЬТРА

        public static List<ParamCanal> myLISTfull;//ДО ФИЛЬТРА

        public static List<ParamCanal> myLISTselect;//ОКНО ВЫБОРКИ

        public static List<ParamCanal> myLISTdub;//ДО ФИЛЬТРА

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
        System.Windows.Threading.DispatcherTimer tmr;
        
        //**********************************************************
        // INIT
        //**********************************************************
        public ViewModelMain()
        {
            if (loc.start_one) return;
            loc.start_one = true;
            ViewModelWindow1.Event_UpdateEDIT += new Delegate_UpdateEDIT(updateEDIT);
            ViewModelWindow1.Event_ADDBEST += new Action(BEST_ADD);
            ViewModelWindow2.Event_UpdateCollection += new Delegate_UpdateCollection(updateLIST);
            Update_Collection.Event_UpdateCollection += new Delegate_UpdateCollection(updateLIST);
            ListViewDragDropManager.WindowMOVE.Event_UpdateCollection += new Delegate_UpdateCollection(updateLIST);
            ViewModelWindowReplace.Event_UpdateCollection += new Delegate_UpdateCollection(updateLIST);
            FileWork.Event_UpdateLIST += new Action<typefilter>(Update_collection);

            ini_command();
            CreateTimer1(500);//500 мс
            CreateTMR(1);// 1sec
        }

        void CreateTMR(int s)
        {
            tmr = new System.Windows.Threading.DispatcherTimer();
            tmr.Tick += new EventHandler(timerTick);
            tmr.Interval = new TimeSpan(0, 0, s);
            tmr.Start();
        }

       
        private void timerTick(object sender, EventArgs e)
        {
            if (loc.timer_tmr) return;
            loc.timer_tmr = true;
            if (Model.data.arguments_startup != null)
                if (Model.data.arguments_startup[0] != "")
                { Open_arguments(); data.arguments_startup = null; }

            if (ModeWork.OpenWindow_db_update && ModeWork.OpenWindow_db_updateREADY)
            {
                if (EVENT_OPENWIN_UpdateDB != null) EVENT_OPENWIN_UpdateDB();
                tmr.Stop();
                tmr = null;
            }

            if (ModeWork.OpenWindow_radio && ModeWork.OpenWindow_radioREADY)
            {
                if (EVENT_OPENWIN_Radio != null) EVENT_OPENWIN_Radio();
                if (ModeWork.CLOSE_ALL) if (EVENT_CLOSE_ALL != null) EVENT_CLOSE_ALL();
                tmr.Stop();
                tmr = null;
            }

            if (ModeWork.add )
            {
                ModeWork.skip_message_skiplinks = true;
                if (EVENT_ADD != null)
                {
                    ModeWork.add = false;
                    EVENT_ADD(FileWork.Get_ApplPath() + IPTVman.Model.ModeWork.addpath);
                }
            }

            Trace.WriteLine("tik timer");
            loc.timer_tmr = false;

        }
      
        private void Create_Virtual_Collection()
            {
            int numItems=1000000;
            int fetchDelay = 1;// 
            myProvider = new CollectionProvider(numItems, fetchDelay);

            myLISTdub = new List<ParamCanal>();//после фильтра
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
            Update_collection(typefilter.last); 
        }

        void updateEDIT(ParamCanal item)
        {
            int i = 0;
  
            if (loc.edit) return;
            loc.edit = true;
            foreach (var obj in myLISTfull)
            {
               if (obj.Compare()==data.canal.Compare())
                {
                    myLISTfull[i] = (ParamCanal)item.Clone();
                    break;
                }
                i++;
            }

            Update_collection(typefilter.last);
        }


        void updateLIST(ParamCanal item)
        {
            Update_collection(typefilter.last);
        }


        private object threadLock = new object();
        public void Update_collection(typefilter t)
        {
            lock (threadLock)
            {
                _update.UPDATE_FILTER(t, myLISTbase, myLISTfull);
                Thread.Sleep(300);
                RaisePropertyChanged("mycol");///update LIST!!
                RaisePropertyChanged("numberCANALS");
            }
        }


      

    }//class
}//namespace
