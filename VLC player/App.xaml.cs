using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace VLC_player
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                byte i = 0;
                IPTVman.ViewModel.data.url = e.Args[0];
                IPTVman.ViewModel.data.name = "";
                foreach (string arg in e.Args)
                {
                    if (i != 0)
                    {
                        if (arg == "--scan")
                        { IPTVman.ViewModel.data.mode_scan = true; break;}

                        if (arg != "--radio") IPTVman.ViewModel.data.name += arg + " ";
                        else IPTVman.ViewModel.data.mode_radio = true;
                    }
                    i++;
                }
                
            }
            catch { }
           
        }


    }

   
}
