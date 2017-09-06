using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Threading;
using System.Windows;

namespace IPTVman.ViewModel
{ 
    
        // Реализация методов, которые описаны в интерфейсе
        public class MyService : IWCFmyService
        {
           
            List<string> IWCFmyService.Get_Playing(List<string> s)
            {
                Result.RUN_SCAN(s);
                List<string> rez = new List<string>();

                while (true)
                {
                    if (Result.data_ok)
                    {
                        //отправляем резултьтаты сканирования
                        foreach (var r in Result.data)
                        {
                            rez.Add(r);
                        }
                        Result.data_ok = false;
                        break;
                    }
                    else Thread.Sleep(100);

                }
                return rez;
            }
        }


        class WCFSERVER
        {
            ServiceHost host;
            /// <summary>
            /// SERVER(url)
            /// </summary>
            /// <param name="url"></param>
            public WCFSERVER(string url)
            {
                //"http://localhost:8000/IPTVmanagerSevice"
                // Инициализируем службу, указываем адрес, по которому она будет доступна
                host = new ServiceHost(typeof(MyService), new Uri(url));
                // Добавляем конечную точку службы с заданным интерфейсом, привязкой (создаём новую) и адресом конечной точки
                host.AddServiceEndpoint(typeof(IWCFmyService), new BasicHttpBinding(), "");
                // Запускаем службу
                host.Open();
               // Console.WriteLine("Сервер запущен");
               // Console.ReadLine();
               
            }


            public void stop()
            {
                if (host == null) return;
                // Закрываем службу
                host.Close();
            }
        }

    }

  
    [ServiceContract]
    public interface IWCFmyService
    {
        [OperationContract]
        List<string> Get_Playing(List<string> s);

    }


    class WCFCLIENT
    {
        IWCFmyService service;
        ChannelFactory<IWCFmyService> factory;

        /// <summary>
        /// CLIENT(url)
        /// </summary>
        /// <param name="url"></param>
        public WCFCLIENT(string url)
        {
            // Задаём адрес нашей службы //"http://localhost:8000/IPTVmanagerSevice"
            Uri tcpUri = new Uri(url);
            // Создаём сетевой адрес, с которым клиент будет взаимодействовать
            EndpointAddress address = new EndpointAddress(tcpUri);
            BasicHttpBinding binding = new BasicHttpBinding();
            // Данный класс используется клиентами для отправки сообщений
            factory = new ChannelFactory<IWCFmyService>(binding, address);
            // Открываем канал для общения клиента со службой
            service = factory.CreateChannel();
            
        }

        public List<string> GetPlaying(List<string> s)
        {
            return (service.Get_Playing(s));
        }
  

}
