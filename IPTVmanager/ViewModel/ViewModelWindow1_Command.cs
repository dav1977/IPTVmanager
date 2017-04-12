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




namespace IPTVman.ViewModel
{
    partial class ViewModelWindow1 : ViewModelMain
    {
       
        public RelayCommand key_PING { get; set; }


        //====== INIT =====
        public ViewModelWindow1(string lastText)
        {
            p = new ParamCanal
            {
                name = data.name,
                ExtFilter =data.extfilter,
                group_title =data.grouptitle,
                http =data.http,
                logo =data.logo,
                tvg_name = data.tvg
            };

            key_PING = new RelayCommand(PING);
     
        }

        public string GetExternalIP()
        {
            String url = p.http;
            String result = null;
            try
            {
                WebClient client = new WebClient();
                result = client.DownloadString(url);
             

                MessageBox.Show("IP="+ ">>" + result.ToString() + "<<", "non",
                 MessageBoxButton.OK);

                return result;
            }
            catch (Exception ex) {
                MessageBox.Show("НЕ СУЩЕСТВУЕТ "+ ex.Message.ToString(), "",    
                                    MessageBoxButton.OK);
                return ("");
            }
        }


        void PING(object selectedItem)
        {

            GetExternalIP();
        }


   


    }
}
