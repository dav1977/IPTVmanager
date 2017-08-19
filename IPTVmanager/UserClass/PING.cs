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
    public class PING
    {
        public string result = "";
        public bool done = false;

        public PING()
        {
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
                else ip0 += "";// " alias not ";

            }
            catch (Exception ex)
            {
                ip0 = "error "+data.NOT_URL+".  " + ex.Message.ToString();// Console.WriteLine(ex.ToString());
            }

            return ip0;
        }

        /// <summary>
        ///   СИНХРОННОЕ ОЖИДАНИЕ ОТВЕТА
        /// </summary>
        public string GETnoas(CancellationToken cancellationToken, string url)
        {
           

             done = false;
            url = url.Trim();
            string ip_url = "";
            string ip0="";
            char[] chars = new char[1000];
            if (new Regex("udp://").Match(url).Success)
            { }
            else
            {
                ip0 = GetIPAddress(url);
                if (ip0 == "") { ip0 = "не определен "; }
            }

            string ip = "ip=" + ip0 + "; ";
          
            string[] split = url.Split(new Char[] { '.' });

            if (split.Length < 2) {  return (exit("нет IP  "+data.NOT_URL+" " + ip) ); }

            result = ip;

            if (!iswork) {  exit("прерывание пинга "); return "прерывание пинга "; };



            try
            {
                if (new Regex("udp://").Match(url).Success)
                {
                    IPAddress ipAddr = IPAddress.Parse(parseUDP(url));
                    UdpClient client = new UdpClient(portUDP(url));
                    // IP-адрес группы
                    IPAddress multicastIP = IPAddress.Parse(parseUDP(url));

                    client.DontFragment = true;
                    client.EnableBroadcast = true;
                    // Соединяемся с удаленным хостом
                    client.Connect(ipAddr, portUDP(url));
                    // Присоединяемся к группе
                    //client.JoinMulticastGroup(multicastIP);

                    //Отправка простого сообщения
                    byte[] bytes = Encoding.UTF8.GetBytes("Test");
                    var sr=client.Send(bytes, bytes.Length);

                    // Создаем переменную IPEndPoint, чтобы передать ссылку на нее в Receive()
                    IPEndPoint RemoteIPEndPoint = null;

                    // Получение данных
                    //byte[] bytes = client.Receive(ref RemoteIPEndPoint);
                    // string results = Encoding.UTF8.GetString(bytes);

                    if (client != null) ip = parseUDP(url) + " port=" + portUDP(url)+ " result "+sr.ToString();
                  
                    // Закрываем соединение
                    if (client != null)
                    {
                        //client.Close();
                        //client.DropMulticastGroup(multicastIP);
                    }
                }
                else
                {
                    Stream stream;
                    MyWebClient client = new MyWebClient(5000);
                    stream = client.OpenRead(url);
                    if (stream == null) { result = data.NOT_URL + "."; return (exit("errorSTREAMnull")); }

                    StreamReader sr = new StreamReader(stream);

                    sr.ReadBlock(chars, 0, 500);
                    if (sr != null) sr.Close();
                    if (stream != null) stream.Close();
                }
                string rez = "";
                if (ip_url != "") rez += "IP=" + ip_url + " ";

                // string newLine;
                //byte[] data = client.(url);
                //if (data == null) { result = "+data.NOT_URL+" нулевой ответ"; return result;  }

                int i = 0;
                 if (chars != null)
                 {
                    foreach (var s in chars)
                    {
                        if (s < 32 || s>126) chars[i] = ' ';
                        i++;
                    }
                 }
               // char[] chars = Encoding.ASCII.GetChars(data);
                rez = new string(chars);
                return (exit(ip + rez));
            }
            catch (WebException ex)
            {        
                string rez = "";
                string error = ex.Message.ToString();
                var regex1 = new Regex("(500)");//ВНУТРЕННЯЯ ОШИБКА СЕРВЕРА

                var r = regex1.Match(error);

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
                     else   rez = data.NOT_URL+". WebException " + ex.Message.ToString() + " ";
                }
                return (exit(ip + rez));
            }

            catch (Exception ex)
            {
                //MessageBox.Show(data.NOT_URL+" "+ ex.Message.ToString(), "",    
                //                    MessageBoxButton.OK);
                return (exit(" не определено ExceptionWebClient " + ex.Message));
            }    
        }

        string parseUDP(string url)
        {
            string s = "";
            string[] w = url.Split(new char[] { ':' });
            s= w[1].Replace("//@", " ");
            s = s.Replace("//", " ");

            return s.Trim();
        }
        int portUDP(string url)
        {
            string s = "";
            string[] w = url.Split(new char[] { ':' });
            s = w[2].Replace("&tvssrc=T", " ");
            int rez = 0;
            try
            {
                rez = int.Parse(s.Trim());
            }
            catch { rez = 1234;  }
            return rez;
            
        }
        public string exit(string s)
        {
            result = s.Replace("#EXT-X-VERSION:3", "");
            result = result.Replace("#EXT-X-VERSION:2", "");
            result = result.Replace("#EXT-X-STREAM-INF", "");
            result = result.Replace("#EXT-X-TARGET", "");

            //Исключения
            Regex regex1 = new Regex("Сервер нарушил протокол");
            var m = regex1.Match(result);
            if (m.Success) { result = result.Replace(data.NOT_URL, ""); result = result.Replace(data.NOT_URL, ""); }

            regex1 = new Regex("Обычно - это временная ошибка");
            m = regex1.Match(result);
            if (m.Success) { result = result.Replace(data.NOT_URL, ""); result = result.Replace(data.NOT_URL, ""); }

            regex1 = new Regex("Этот хост неизвестен");
            m = regex1.Match(result);
            if (m.Success) { result = result.Replace(data.NOT_URL, ""); result = result.Replace(data.NOT_URL, ""); }

            done = true;
            return s;
        }
    }



    public class MyWebClient : WebClient
    {
        //time in milliseconds
        private int timeout;
        public int Timeout
        {
            get
            {
                return timeout;
            }
            set
            {
                timeout = value;
            }
        }

        public MyWebClient()
        {
            this.timeout = 5000;
        }

        public MyWebClient(int timeout)
        {
            this.timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest result=null;
            try
            {
                result = base.GetWebRequest(address);
            }
            catch { }
            if (result!=null)result.Timeout = this.timeout;
            return result;
        }
    }
}
