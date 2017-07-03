using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPTVman.Helpers;
using IPTVman.Model;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows;
using System.Threading;



/// <summary>
///   ОКНО ПЕРЕМЕЩЕНИЯ
/// </summary>
namespace IPTVman.ViewModel
{
    partial class ViewModelWindowReplace : ViewModelMain
    {

      // public static event Delegate_UpdateMOVE Event_UpdateAFTERmove;
      // public static event Delegate_SelectITEM Event_SELECT;

        public RelayCommand key_ReplaceCommandSTART { get; set; }
   

        //============================== INIT ==================================
        public ViewModelWindowReplace()
        {

            key_ReplaceCommandSTART = new RelayCommand(key_replace);
        
  

        }
        //=============================================================================

        /// <summary>
        /// UP
        /// </summary>
        
        void key_replace(object selectedItem)
        {
         


        }


           

       
    
    }
}
