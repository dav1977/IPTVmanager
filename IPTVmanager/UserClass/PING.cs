using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Threading;
using IPTVman.Helpers;
using IPTVman.Model;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace IPTVman.ViewModel
{
    public delegate void Delegate_CLOSEPING(string s);
    class PING
    {
        public string result = "";
        public bool done = false;

        public PING()
        {
            WindowPING.Event_Close_ping += WindowPING_Event_Close_ping;
        }


        bool _iswork = false;
        public bool iswork
        {
            get
            {
                return _iswork;
            }
            set
            {
                _iswork = true;
            }
        }

        public void stop()
        {
            done = false;
            _iswork = false;
        }

        private void WindowPING_Event_Close_ping(string s)
        {
           string nl=exit(s);
        }

        public static string GetIPlocalAddress(string serverName)
        {
            string ip0 = "";
            try
            {
                // IPHostEntry hostname = Dns.GetHostByName(serverName);
                IPAddress[] IPs = Dns.GetHostAddresses(Dns.GetHostName());
                foreach (IPAddress addr in IPs)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ip0 += addr.ToString();
                    }
                }
            }
            catch
            {
                ip0 = "error ip ";// Console.WriteLine(ex.ToString());
            }

            return ip0;
        }

        /// <summary>
        /// ВОЗВРАЩАЕТ IP
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetIPAddress(string url)
        {
            string[] split = { "", "" };
            string ip0 = "";
            try
            {

                string hostname = "";
                // hostname = "cdn-01.bonus-tv.ru";
                split = url.Split(new Char[] { '/', ':' });
                hostname = split[3];

                IPHostEntry entry = Dns.GetHostEntry(hostname);

                foreach (IPAddress a in entry.AddressList)
                    ip0 += a.ToString() + ";";

                if (ip0 == "") return "";

                if (entry.Aliases.Length != 0)
                {
                    foreach (string aliasName in entry.Aliases)
                        ip0 += aliasName + "\n";
                }
                else ip0 += " alias not ";

            }
            catch (Exception ex)
            {
                ip0 = "error НЕ СУЩЕСТВУЕТ.  " + ex.Message.ToString();// Console.WriteLine(ex.ToString());
            }

            return ip0;
        }

        /// <summary>
        ///   СИНХРОННОЕ ОЖИДАНИЕ ОТВЕТА
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GETnoas(CancellationToken cancellationToken, string url)
        {
            done = false;
            url = url.Trim();
            string ip_url = "";

            string ip0 = GetIPAddress(url);

            Regex regex1 = new Regex("(udp/)");//udp не возвращает url

            var r = regex1.Match(url);

            if (r.Success)
            {

            }
            else
            {
                if (ip0 == "") {   return (exit("нет IP   НЕ СУЩЕСТВУЕТ ")); }
            }
            string ip = "ip=" + ip0 + "; ";
           
   
            string[] split = url.Split(new Char[] { '.' });

           
            if (split.Length < 2) {  return (exit("нет IP  НЕ СУЩЕСТВУЕТ " + ip) ); }

            result = ip;

            if (!iswork) {  exit("прерывание пинга "); return "прерывание пинга "; };

            try
            {
                WebClient client = new WebClient();
 
                string rez = "";
                if (ip_url != "") rez += "IP=" + ip_url + " ";

                Stream stream = client.OpenRead(url);
                if (stream == null) { result = "НЕ СУЩЕСТВУЕТ.";  return (exit("errorSTREAMnull"));  }

                StreamReader sr = new StreamReader(stream);
                string newLine;
                while ((newLine = sr.ReadLine()) != null)
                {
                    if (!iswork) {   exit("прерывание пинга "); return "прерывание пинга "; ; };

                    string[] words;
                    words = newLine.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);

                    foreach (var s in words)
                    {
                        rez += s.ToString() + '\n';
                        if (!iswork) {  exit("прерывание пинга "); return "прерывание пинга "; };
                    }
                }
                if (stream != null) stream.Close();

                if (stream == null) { result = "НЕ СУЩЕСТВУЕТ."; }


                return (exit(ip + rez));
            }
            catch (WebException ex)
            {        
                string rez = "";
                string error = ex.Message.ToString();
                regex1 = new Regex("(500)");//ВНУТРЕННЯЯ ОШИБКА СЕРВЕРА

                r = regex1.Match(error);

                if (r.Success)
                {
                    rez = "WebException(500) НЕТ ДАННЫХ  " + " ";
                }
                else
                {
                    regex1 = new Regex("(404)");//не найден

                    r = regex1.Match(url);

                    if (r.Success)
                    {
                    }
                     else   rez = "НЕ СУЩЕСТВУЕТ. WebException " + ex.Message.ToString() + " ";
                }

                // using (var str = ex.Response.GetResponseStream())
                //using (var read = new StreamReader(str))
                //{
                //    rez += read.ToString() + '\n';
                //}

                // WebClient client = new WebClient();
                // Stream stream = client.;

            
                return (exit(ip + rez));
            }

            catch (Exception ex)
            {
                //MessageBox.Show("НЕ СУЩЕСТВУЕТ "+ ex.Message.ToString(), "",    
                //                    MessageBoxButton.OK);

                return (exit("не определено ExceptionWebClient " + ex.Message));
            }

             
        }

        string exit(string s)
        {
            result = s;
            data.start_ping = false;
            done = true;
            return s;
        }

       

    }
}
