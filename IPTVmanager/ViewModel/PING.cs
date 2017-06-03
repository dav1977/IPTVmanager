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
    class PING
    {
        public string result77 = "";


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
                //  ip0 = IPs[0].ToString();
            }
            catch
            {
                ip0 = "error";// Console.WriteLine(ex.ToString());
            }

            return ip0;
        }


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

                if (entry.Aliases.Length != 0)
                {
                    foreach (string aliasName in entry.Aliases)
                        ip0 += aliasName + "\n";
                }
                else ip0 += " alias not ";



            }
            catch (Exception ex)
            {
                ip0 = "error " + ex.Message.ToString();// Console.WriteLine(ex.ToString());
            }

            return ip0;
        }


        public string GETnoas(string url)
        {
            url = url.Trim();
            string ip_url = "";

            string ip = "ip=" + GetIPAddress(url) + "; ";
            result77 = ip;

            string[] split = url.Split(new Char[] { '.' });

            if (split.Length < 2) return "НЕТ IP " + ip;

            try
            {
                //Ping ping = new Ping();
                //var replay = ping.Send(url);

                //if (replay.Status == IPStatus.Success)
                //{
                //    ip_url = replay.Address.ToString();
                //}
                //ip_url = GetIPAddress(url);


                WebClient client = new WebClient();

                // MessageBox.Show("IP="+ ">>" + result.ToString() + "<<", "non",
                //   MessageBoxButton.OK);

                string rez = "";
                if (ip_url != "") rez += "IP=" + ip_url + " ";


                Stream stream = client.OpenRead(url);
                if (stream == null) { result77 = "НЕ СУЩЕСТВУЕТ."; data.start_ping = false; return "erroMNN"; }
                StreamReader sr = new StreamReader(stream);
                string newLine;
                while ((newLine = sr.ReadLine()) != null)
                {
                    string[] words;
                    words = newLine.Split(default(Char[]), StringSplitOptions.RemoveEmptyEntries);

                    foreach (var s in words)
                        rez += s.ToString() + '\n';

                }
                if (stream != null) stream.Close();

                if (stream == null) { result77 = "НЕ СУЩЕСТВУЕТ."; }



                result77 = ip + rez;
                data.start_ping = false;
                return result77;
            }
            catch (WebException ex)
            {
                string rez = "";
                string error = ex.Message.ToString();
                Regex regex1 = new Regex("(500)");//ВНУТРЕННЯЯ ОШИБКА СЕРВЕРА

                var r = regex1.Match(error);

                if (r.Success)
                {
                    rez = "WebException(500) НЕТ ДАННЫХ  " + " ";
                }
                else
                    rez = "НЕ СУЩЕСТВУЕТ " + ex.Message.ToString() + " ";


                // using (var str = ex.Response.GetResponseStream())
                //using (var read = new StreamReader(str))
                //{
                //    rez += read.ToString() + '\n';
                //}

                // WebClient client = new WebClient();
                // Stream stream = client.;

                result77 = ip + rez;
                data.start_ping = false;
                return result77;
            }

            catch (Exception ex)
            {
                //MessageBox.Show("НЕ СУЩЕСТВУЕТ "+ ex.Message.ToString(), "",    
                //                    MessageBoxButton.OK);
                result77 = "НЕ СУЩЕСТВУЕТ " + ex.Message;
                data.start_ping = false;
                return ("НЕ СУЩЕСТВУЕТ " + ex.Message);
            }
        }



    }
}
