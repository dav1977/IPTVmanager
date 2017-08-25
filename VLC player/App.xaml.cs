using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VLC_player
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string[] par = new string[100];
            byte i = 0;
            foreach (string arg in e.Args)
            { par[i] = arg; i++;
 
            }

            IPTVman.ViewModel.data.url = par[0];
            IPTVman.ViewModel.data.name = par[1];
        }
    }

   
}
