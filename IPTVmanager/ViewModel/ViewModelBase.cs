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
    class ViewModelBase : INotifyPropertyChanged
    {
        System.Timers.Timer Timer1;

        public void CreateTimer1(int ms)
        {
            if (Timer1 == null)
            {
                Timer1 = new System.Timers.Timer();
                //Timer1.AutoReset = false; //
                Timer1.Interval = ms;
                Timer1.Elapsed += Timer1Tick;
                Timer1.Enabled = true;
                Timer1.Start();
            }

            

        }

        private void Timer1Tick(object source, System.Timers.ElapsedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                RaisePropertyChanged("memory");

            }
        }




        string UDPtest(string url)
        {
            //udp://@233.7.70.79:5000&tvssrc=N
            Query.Query sQuery = new Query.Query("233.7.70.50", 5000);

            sQuery.Send('K');

            int count = sQuery.Receive();

            string[] info = sQuery.Store(count);

            string rez = "";

            foreach (var s in info)
            {

                rez += s.ToString();
            }

            result77 = rez + count.ToString();
            /* 
             * Variable 'info' might now contain:   
             *   Password   Players     Max. players    Hostname                Gamemode    Language
             * { "0",       "12",       "500",          "Query test server",    "LVDM",     "English" }
             */

            //-------------------------

            //Query.RCONQuery sQuery = new Query.RCONQuery("127.0.0.1", 7777, "changeme");

            //sQuery.Send("echo Hello from C#");

            //int count = sQuery.Receive();

            //string[] info = sQuery.Store(count);

            /* 
             * Variable 'info' might now contain:
             * { "Hello from C#" }
             */
            return "";

        }

        /// <summary>
        /// ///////////////////////////////// NEWUDP
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>




        // Детали файла
        [Serializable]
        public class FileDetails
        {
            public string FILETYPE = "";
            public long FILESIZE = 0;
        }

        private static FileDetails fileDet;
        // Поля, связанные с UdpClient
        private static int localPort = 5000;
        private static UdpClient receivingUdpClient = new UdpClient(localPort);
        private static IPEndPoint RemoteIpEndPoint = null;

        private static FileStream fs;
        private static Byte[] receiveBytes = new Byte[0];


        string NEW_UDP()
        {
            try
            {        ///udp://@233.7.70.39:5000&tvssrc=K
                IPAddress ipAddr = IPAddress.Parse("233.7.70.39");
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 5000);
                //IPEndPoint RemoteIPEndPoint = null;

                // IP-адрес группы
                IPAddress multicastIP = IPAddress.Parse("233.7.70.39");


                // Создаем UdpClient
                UdpClient udpClient;//= new UdpClient();

                // Соединяемся с удаленным хостом
                /// udpClient.Connect(ipAddr, 5000);





                // Отправка простого сообщения
                // byte[] bytes = Encoding.UTF8.GetBytes("Test");
                // udpClient.Send(bytes, bytes.Length);

                // Получение данных
                // byte[] bytesREC = udpClient.Receive(ref RemoteIPEndPoint);
                // string results = Encoding.UTF8.GetString(bytesREC);

                string results = "";


                // Присоединяемся к группе
                //udpClient.JoinMulticastGroup(multicastIP);

                IPEndPoint RemoteIpEndPoint = null;

                //while (true)
                //{



                //    // Ожидание дейтаграммы
                //    byte[] receiveBytes = udpClient.Receive( ref RemoteIpEndPoint);

                //    // Преобразуем и отображаем данные
                //    string returnData = Encoding.UTF8.GetString(receiveBytes);
                //    results = returnData;
                //    if (results != "") break;
                //}



                // Получаем информацию о файле
                //receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);


                ////XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
                //MemoryStream stream1 = new MemoryStream();

                //// Считываем информацию о файле
                //stream1.Write(receiveBytes, 0, receiveBytes.Length);
                //stream1.Position = 0;

                //// Вызываем метод Deserialize
                ////fileDet = (FileDetails)fileSerializer.Deserialize(stream1);

                //results = stream1.ToString();

                //Console.WriteLine("Получен файл типа ." + fileDet.FILETYPE +
                //    " имеющий размер " + fileDet.FILESIZE.ToString() + " байт");


                ///////////-------------------
                IPEndPoint remoteIPEndPoint = new IPEndPoint(multicastIP, 5000);

                //  udpClient.Client.Bind(remoteIPEndPoint);

                //udpClient.Connect(ipAddr, 5000);

                //// Отправка простого сообщения
                //byte[] bytes = Encoding.UTF8.GetBytes("Test");
                //udpClient.Send(bytes, bytes.Length);

                //byte[] content = udpClient.Receive(ref remoteIPEndPoint);



                //while (true)
                //{
                //    if (content.Length > 0)
                //    {
                //        string message = Encoding.ASCII.GetString(content);

                //        return message;
                //    }

                //}




                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
               ProtocolType.Udp);

                IPAddress broadcast = IPAddress.Parse("233.7.70.39");

                byte[] sendbuf = Encoding.ASCII.GetBytes("test");
                IPEndPoint ep = new IPEndPoint(broadcast, 5000);

                s.SendTo(sendbuf, ep);

                bool done = false;

                UdpClient listener = new UdpClient(5001);
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 5001);



              var  ping = new Socket(AddressFamily.InterNetwork,
                                SocketType.Dgram, ProtocolType.Udp);


 ping.SetSocketOption(SocketOptionLevel.Udp,
SocketOptionName.SendTimeout, 2000);

                byte[] msg = new byte[1] { 1 };
                EndPoint PingEndPoint = (EndPoint)remoteIPEndPoint;
                ping.SendTo(msg, PingEndPoint);


                

                try
                {
                    while (!done)
                    {
                        //Console.WriteLine("Waiting for broadcast");
                        byte[] bytes = listener.Receive(ref groupEP);

                        //  Console.WriteLine("Received broadcast from {0} :\n {1}\n",
                        //     groupEP.ToString(),
                        //     Encoding.ASCII.GetString(bytes, 0, bytes.Length));

                        string rez = groupEP.ToString();
                        rez = ping.Available.ToString();

                        if (rez != "") { result77 = rez; done = true; }
                    }

                }
                catch (Exception e)
                {
                  //  Console.WriteLine(e.ToString());
                }





                // Закрываем соединение
                //udpClient.Close();
                return results;

            }
            catch (ArgumentNullException ex)
            {
                //Console.WriteLine(ex.ToString());
                return ex.Message.ToString();
            }


            return "--";

        }



        public Task<string> AsyncTaskGet(string url)
        {
         
            return Task.Run(() =>
            {
                //----------------

                return GETnoas(url);
                
                //----------------
            });
        }

        public static string result77="";

         async void GETasyn(string url)
        {

            data.start_ping = true;
            string ss = await AsyncTaskGet(url);

        }

        public string GET(string u)
        {
            result77 = "";

            if (task_ping !=null)
            if (task_ping.Status == TaskStatus.Running) return "task is running";
           GETasyn(u);//асинхр

   //         test(u);

            //string ss= UDPtest("");
           // result77 = NEW_UDP();


            //if (result77 == "") result77 = "НЕТУ UDP";
            //result77=GETnoas(u);//синхр

            return result77;
        }


        public Task task_ping;

        async void test(string url)
        {

            task_ping = Task.Run(async delegate
            {
                GETnoas(url);
                await Task.Delay(TimeSpan.FromSeconds(3));
                
            });

            await Task.WhenAll(task_ping);
            

            // using (var cts = new CancellationTokenSource())
            //{
            //    var ct = cts.Token;
            //    var tasks =  Enumerable.Range(0, 30).Select(i => Task.Run(() => GETnoas(url), ct));

            //    cts.CancelAfter(TimeSpan.FromSeconds(3));



            //    try
            //    {
            //        //await Task.WhenAll(tasks);
            //        await Task.WhenAll(tasks);

            //        // await Task.Delay(5);
            //    }
            //    catch (AggregateException)
            //    {
            //        result77 = "errror";
            //    }
            //}

        }


        public static string GetIPAddress(string serverName)
        {


            IPHostEntry ipHostInfo = Dns.GetHostEntry(serverName);
            IPAddress ipAddress = ipHostInfo.AddressList
                .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            return ipAddress.ToString();
        }

        public string GETnoas(string url)
        {
            url = url.Trim();
            string ip_url = "";

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
               if (ip_url!="") rez += "IP=" + ip_url + " ";


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
                if (stream!=null) stream.Close();

                if (stream == null) { result77 = "НЕ СУЩЕСТВУЕТ."; }
                result77 = rez;
                data.start_ping = false;
                return rez;
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



        //basic ViewModelBase
        internal void RaisePropertyChanged(string prop)
        {
           if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
               
            }
         

        }
       public  event PropertyChangedEventHandler PropertyChanged; //событие выбора канала




       



        //Extra Stuff, shows why a base ViewModel is useful
        bool? _CloseWindowFlag;
        public bool? CloseWindowFlag
        {
            get { return _CloseWindowFlag; }
            set
            {
                _CloseWindowFlag = value;
                RaisePropertyChanged("CloseWindowFlag");
            }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null 
                    ? true 
                    : !CloseWindowFlag;
            }));
        }
    }
}
