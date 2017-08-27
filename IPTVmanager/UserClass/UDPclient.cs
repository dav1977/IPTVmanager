////using System.IO;
////using System.Net;
////using System.Net.Sockets;
////using System;


//string UDPtest(string url)
//{
//    //udp://@233.7.70.79:5000&tvssrc=N
//    //Query.Query sQuery = new Query.Query("233.7.70.50", 5000);

//    //sQuery.Send('K');

//    //int count = sQuery.Receive();

//    //string[] info = sQuery.Store(count);

//    //string rez = "";

//    //foreach (var s in info)
//    //{

//    //    rez += s.ToString();
//    //}

//    //result = rez + count.ToString();
//    ///* 
//    // * Variable 'info' might now contain:   
//    // *   Password   Players     Max. players    Hostname                Gamemode    Language
//    // * { "0",       "12",       "500",          "Query test server",    "LVDM",     "English" }
//    // */

//    ////-------------------------

//    ////Query.RCONQuery sQuery = new Query.RCONQuery("127.0.0.1", 7777, "changeme");

//    ////sQuery.Send("echo Hello from C#");

//    ////int count = sQuery.Receive();

//    ////string[] info = sQuery.Store(count);

//    ///* 
//    // * Variable 'info' might now contain:
//    // * { "Hello from C#" }
//    // */
//    return "";

//}


//string NEW_UDP()
//{

//    try
//    {        ///udp://@233.7.70.39:5000&tvssrc=K
//        IPAddress ipAddr = IPAddress.Parse("233.7.70.39");
//        IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 5000);
//        //IPEndPoint RemoteIPEndPoint = null;

//        // IP-адрес группы
//        IPAddress multicastIP = IPAddress.Parse("233.7.70.39");


//        // Создаем UdpClient
//        UdpClient udpClient = new UdpClient();

//        // Соединяемся с удаленным хостом
//        udpClient.Connect(ipAddr, 5000);





//        // Отправка простого сообщения
//        // byte[] bytes = Encoding.UTF8.GetBytes("Test");
//        // udpClient.Send(bytes, bytes.Length);

//        // Получение данных
//        // byte[] bytesREC = udpClient.Receive(ref RemoteIPEndPoint);
//        // string results = Encoding.UTF8.GetString(bytesREC);

//        string results = "";


//        // Присоединяемся к группе
//        //udpClient.JoinMulticastGroup(multicastIP);



//        IPEndPoint RemoteIpEndPoint = null;

//        while (true)
//        {



//            // Ожидание дейтаграммы
//            byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);

//            // Преобразуем и отображаем данные
//            string returnData = Encoding.UTF8.GetString(receiveBytes);
//            results = returnData;
//            if (results != "") break;
//        }



//        // Получаем информацию о файле
//        //receiveBytes = receivingUdpClient.Receive(ref RemoteIpEndPoint);


//        ////XmlSerializer fileSerializer = new XmlSerializer(typeof(FileDetails));
//        //MemoryStream stream1 = new MemoryStream();

//        //// Считываем информацию о файле
//        //stream1.Write(receiveBytes, 0, receiveBytes.Length);
//        //stream1.Position = 0;

//        //// Вызываем метод Deserialize
//        ////fileDet = (FileDetails)fileSerializer.Deserialize(stream1);

//        //results = stream1.ToString();

//        //Console.WriteLine("Получен файл типа ." + fileDet.FILETYPE +
//        //    " имеющий размер " + fileDet.FILESIZE.ToString() + " байт");


//        ///////////-------------------
//        IPEndPoint remoteIPEndPoint = new IPEndPoint(multicastIP, 5000);

//        udpClient.Client.Bind(remoteIPEndPoint);

//        //udpClient.Connect(ipAddr, 5000);

//        //// Отправка простого сообщения
//        //byte[] bytes = Encoding.UTF8.GetBytes("Test");
//        //udpClient.Send(bytes, bytes.Length);

//        //byte[] content = udpClient.Receive(ref remoteIPEndPoint);



//        //while (true)
//        //{
//        //    if (content.Length > 0)
//        //    {
//        //        string message = Encoding.ASCII.GetString(content);

//        //        return message;
//        //    }

//        //}


//        //----------------------------------------------------------------------------------

//        //                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
//        //               ProtocolType.Udp);

//        //                IPAddress broadcast = IPAddress.Parse("233.7.70.39");

//        //                byte[] sendbuf = Encoding.ASCII.GetBytes("test");
//        //                IPEndPoint ep = new IPEndPoint(broadcast, 5000);

//        //                s.SendTo(sendbuf, ep);

//        //                bool done = false;

//        //                UdpClient listener = new UdpClient(5001);
//        //                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, 5001);



//        //              var  ping = new Socket(AddressFamily.InterNetwork,
//        //                                SocketType.Dgram, ProtocolType.Udp);


//        // ping.SetSocketOption(SocketOptionLevel.Udp,
//        //SocketOptionName.SendTimeout, 2000);

//        //                byte[] msg = new byte[1] { 1 };
//        //                EndPoint PingEndPoint = (EndPoint)remoteIPEndPoint;
//        //                ping.SendTo(msg, PingEndPoint);




//        //                try
//        //                {
//        //                    while (!done)
//        //                    {
//        //                        //Console.WriteLine("Waiting for broadcast");
//        //                        byte[] bytes = listener.Receive(ref groupEP);

//        //                        //  Console.WriteLine("Received broadcast from {0} :\n {1}\n",
//        //                        //     groupEP.ToString(),
//        //                        //     Encoding.ASCII.GetString(bytes, 0, bytes.Length));

//        //                        string rez = groupEP.ToString();
//        //                        rez = ping.Available.ToString();

//        //                        if (rez != "") { result = rez; done = true; }
//        //                    }

//        //                }
//        //                catch (Exception e)
//        //                {
//        //                  //  Console.WriteLine(e.ToString());
//        //                }


//        //----------------------------------------------------------------------------------



//        //try
//        //{

//        //    IPEndPoint Address = new IPEndPoint(IPAddress.Parse("233.7.70.39"), (int)5000);
//        //    var socket = new Socket(AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, ProtocolType.Udp);
//        //    socket.Connect(Address);
//        //    //Send
//        //    byte[] recvData = new byte[1];
//        //    socket.Send(receiveBytes);


//        //    int recv = socket.Receive(recvData);

//        //}
//        //catch {

//        //    MessageBox.Show("НЕТ udp", "error");
//        //}





//        //MessageBox.Show(" OK ", "error");








//        //----------------------------------------------------------------------------------



//        //        Start building the headers
//        //        Console.WriteLine("Building the packet header...");
//        //        int messageSize = 64;
//        //        byte[] builtPacket, payLoad = new byte[messageSize];
//        //        var UdpHeader udpPacket = new UdpHeader();
//        //        var ArrayList headerList = new ArrayList();
//        //        Socket rawSocket = null;
//        //        SocketOptionLevel socketLevel;

//        //        Initialize the payload
//        //        Console.WriteLine("Initialize the payload...");
//        //        for (int i = 0; i < payLoad.Length; i++)
//        //            payLoad[i] = (byte)'0';

//        //        Fill out the UDP header first
//        //        Console.WriteLine("Filling out the UDP header...");
//        //        udpPacket.SourcePort = 33434;
//        //        udpPacket.DestinationPort = 33434;
//        //        udpPacket.Length = (ushort)(UdpHeader.UdpHeaderLength + messageSize);
//        //        udpPacket.Checksum = 0;

//        //        Ipv4Header ipv4Packet = new Ipv4Header();

//        //        Build the IPv4 header
//        //        Console.WriteLine("Building the IPv4 header...");
//        //        ipv4Packet.Version = 4;
//        //        ipv4Packet.Protocol = (byte)ProtocolType.Udp;
//        //        ipv4Packet.Ttl = 30;
//        //        ipv4Packet.Offset = 0;
//        //        ipv4Packet.Length = (byte)Ipv4Header.Ipv4HeaderLength;
//        //        ipv4Packet.TotalLength = (ushort)Convert.ToUInt16(Ipv4Header.Ipv4HeaderLength + UdpHeader.UdpHeaderLength + messageSize);
//        //        ipv4Packet.SourceAddress = sourceAddress;
//        //        ipv4Packet.DestinationAddress = destAddress;

//        //        Set the IPv4 header in the UDP header since it is required to calculate the
//        //           pseudo header checksum
//        //        Console.WriteLine("Setting the IPv4 header for pseudo header checksum...");
//        //        udpPacket.ipv4PacketHeader = ipv4Packet;

//        //        Add IPv4 header to list of headers-- headers should be added in th order
//        //            they appear in the packet (i.e.IP first then UDP)
//        //        Console.WriteLine("Adding the IPv4 header to the list of header, encapsulating packet...");
//        //        headerList.Add(ipv4Packet);
//        //        socketLevel = SocketOptionLevel.IP;

//        //        Add the UDP header to list of headers after the IP header has been added
//        //        Console.WriteLine("Adding the UDP header to the list of header, after IP header...");
//        //        headerList.Add(udpPacket);

//        //        Convert the header classes into the binary on-the - wire representation
//        //        Console.WriteLine("Converting the header classes into the binary...");
//        //        builtPacket = udpPacket.BuildPacket(headerList, payLoad);

//        //        Create the raw socket for this packet
//        //        Console.WriteLine("Creating the raw socket using Socket()...");
//        //       rawSocket = new Socket(sourceAddress.AddressFamily, SocketType.Raw, ProtocolType.Udp);

//        //        Bind the socket to the interface specified
//        //        Console.WriteLine("Binding the socket to the specified interface using Bind()...");
//        //        IPAddress bindAddress = IPAddress.Any;
//        //rawSocket.Bind(new IPEndPoint(bindAddress, 0));

//        //         Set the HeaderIncluded option since we include the IP header
//        //        Console.WriteLine("Setting the HeaderIncluded option for IP header...");
//        //        rawSocket.SetSocketOption(socketLevel, SocketOptionName.HeaderIncluded, 1);
//        //        rawSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);

//        //        Stopwatch timer = new Stopwatch();

//        //        try
//        //        {
//        //            for (int i = 0; i< 5; i++)
//        //            {
//        //                timer.Reset();

//        //                timer.Start();

//        //                int rc = rawSocket.SendTo(builtPacket, new IPEndPoint(destAddress, 0));
//        //Console.WriteLine("Sent {0} bytes to {1}", rc, destAddress);

//        //                Socket icmpListener = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
//        //icmpListener.Bind(new IPEndPoint(sourceAddress, 0));
//        //                icmpListener.IOControl(IOControlCode.ReceiveAll, new byte[] { 1, 0, 0, 0 }, new byte[] { 1, 0, 0, 0 });
//        //                icmpListener.Shutdown(SocketShutdown.Send);

//        //                byte[] buffer = new byte[4096];
//        //EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

//        //                try
//        //                {
//        //                    int bytesRead = icmpListener.ReceiveFrom(buffer, ref remoteEndPoint);
//        //timer.Stop();
//        //                    Console.WriteLine("Recieved " + bytesRead + " bytes from " + destAddress + " in " + timer.ElapsedMilliseconds + "ms\n");
//        //                }
//        //                catch
//        //                {
//        //                     Console.WriteLine("Server is not responding!");
//        //                }
//        //                finally
//        //                {
//        //                    icmpListener.Close();
//        //                }
//        //            }

//        //        }
//        //        catch (SocketException err)
//        //        {
//        //            Console.WriteLine("Socket error occurred: {0}", err.Message);
//        //        }
//        //        finally
//        //        {
//        //            rawSocket.Close();
//        //        }


//        //------------------------------------------------------------------------------------------------------


//        // Закрываем соединение
//        //udpClient.Close();
//        return results;

//    }
//    catch (ArgumentNullException ex)
//    {
//        //Console.WriteLine(ex.ToString());
//        return ex.Message.ToString();
//    }


//}




////namespace Query
////{
////    class RCONQuery
////    {
////        Socket qSocket;
////        IPAddress address;
////        int _port = 0;
////        string _password = null;

////        string[] results = new string[50];
////        int _count = 0;

////        public RCONQuery(string IP, int port, string password)
////        {
////            qSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

////            qSocket.SendTimeout = 5000;
////            qSocket.ReceiveTimeout = 5000;

////            try
////            {
////                address = Dns.GetHostAddresses(IP)[0];
////            }

////            catch
////            {

////            }

////            _port = port;
////            _password = password;
////        }

////        public bool Send(string command)
////        {
////            try
////            {
////                IPEndPoint endpoint = new IPEndPoint(address, _port);

////                using (MemoryStream stream = new MemoryStream())
////                {
////                    using (BinaryWriter writer = new BinaryWriter(stream))
////                    {
////                        writer.Write("SAMP".ToCharArray());

////                        string[] SplitIP = address.ToString().Split('.');

////                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[0])));
////                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[1])));
////                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[2])));
////                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[3])));

////                        writer.Write((ushort)_port);

////                        writer.Write('x');

////                        writer.Write((ushort)_password.Length);
////                        writer.Write(_password.ToCharArray());

////                        writer.Write((ushort)command.Length);
////                        writer.Write(command.ToCharArray());
////                    }

////                    if (qSocket.SendTo(stream.ToArray(), endpoint) > 0)
////                        return true;
////                }
////            }

////            catch
////            {
////                return false;
////            }

////            return false;
////        }

////        public int Receive()
////        {
////            try
////            {
////                for (int i = 0; i < results.GetLength(0); i++)
////                    results.SetValue(null, i);

////                _count = 0;

////                EndPoint endpoint = new IPEndPoint(address, _port);

////                byte[] rBuffer = new byte[500];

////                int count = qSocket.ReceiveFrom(rBuffer, ref endpoint);

////                using (MemoryStream stream = new MemoryStream(rBuffer))
////                {
////                    using (BinaryReader reader = new BinaryReader(stream))
////                    {
////                        if (stream.Length <= 11)
////                            return _count;

////                        reader.ReadBytes(11);
////                        short len;

////                        try
////                        {
////                            while ((len = reader.ReadInt16()) != 0)
////                                results[_count++] = new string(reader.ReadChars(Convert.ToInt32(len)));
////                        }

////                        catch
////                        {
////                            return _count;
////                        }
////                    }
////                }
////            }

////            catch
////            {
////                return _count;
////            }

////            return _count;
////        }

////        public string[] Store(int count)
////        {
////            string[] rString = new string[count];

////            for (int i = 0; i < count && i < _count; i++)
////                rString[i] = results[i];

////            _count = 0;

////            return rString;
////        }
////    }

////    class Query
////    {
////        Socket qSocket;
////        IPAddress address;
////        int _port = 0;

////        string[] results;
////        int _count = 0;

////        DateTime[] timestamp = new DateTime[2];

////        public Query(string IP, int port)
////        {
////            qSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

////            qSocket.SendTimeout = 5000;
////            qSocket.ReceiveTimeout = 5000;

////            try
////            {
////                address = Dns.GetHostAddresses(IP)[0];
////            }

////            catch
////            {

////            }

////            _port = port;
////        }

////        public bool Send(char opcode)
////        {
////            try
////            {
////                EndPoint endpoint = new IPEndPoint(address, _port);

////                using (MemoryStream stream = new MemoryStream())
////                {
////                    using (BinaryWriter writer = new BinaryWriter(stream))
////                    {
////                        writer.Write("SAMP".ToCharArray());

////                        string[] SplitIP = address.ToString().Split('.');

////                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[0])));
////                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[1])));
////                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[2])));
////                        writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[3])));

////                        writer.Write((ushort)_port);

////                        writer.Write(opcode);

////                        if (opcode == 'p')
////                            writer.Write("8493".ToCharArray());

////                        timestamp[0] = DateTime.Now;
////                    }

////                    if (qSocket.SendTo(stream.ToArray(), endpoint) > 0)
////                        return true;
////                }
////            }

////            catch
////            {
////                return false;
////            }

////            return false;
////        }

////        public int Receive()
////        {
////            try
////            {
////                _count = 0;

////                EndPoint endpoint = new IPEndPoint(address, _port);

////                byte[] rBuffer = new byte[3402];
////                qSocket.ReceiveFrom(rBuffer, ref endpoint);

////                timestamp[1] = DateTime.Now;

////                using (MemoryStream stream = new MemoryStream(rBuffer))
////                {
////                    using (BinaryReader reader = new BinaryReader(stream))
////                    {
////                        if (stream.Length <= 10)
////                            return _count;

////                        reader.ReadBytes(10);

////                        switch (reader.ReadChar())
////                        {
////                            case 'i': // Information
////                                {
////                                    results = new string[6];

////                                    results[_count++] = Convert.ToString(reader.ReadByte());

////                                    results[_count++] = Convert.ToString(reader.ReadInt16());

////                                    results[_count++] = Convert.ToString(reader.ReadInt16());

////                                    int hostnamelen = reader.ReadInt32();
////                                    results[_count++] = new string(reader.ReadChars(hostnamelen));

////                                    int gamemodelen = reader.ReadInt32();
////                                    results[_count++] = new string(reader.ReadChars(gamemodelen));

////                                    int mapnamelen = reader.ReadInt32();
////                                    results[_count++] = new string(reader.ReadChars(mapnamelen));

////                                    return _count;
////                                }

////                            case 'r': // Rules
////                                {
////                                    int rulecount = reader.ReadInt16();

////                                    results = new string[rulecount * 2];

////                                    for (int i = 0; i < rulecount; i++)
////                                    {
////                                        int rulelen = reader.ReadByte();
////                                        results[_count++] = new string(reader.ReadChars(rulelen));

////                                        int valuelen = reader.ReadByte();
////                                        results[_count++] = new string(reader.ReadChars(valuelen));
////                                    }

////                                    return _count;
////                                }

////                            case 'c': // Client list
////                                {
////                                    int playercount = reader.ReadInt16();

////                                    results = new string[playercount * 2];

////                                    for (int i = 0; i < playercount; i++)
////                                    {
////                                        int namelen = reader.ReadByte();
////                                        results[_count++] = new string(reader.ReadChars(namelen));

////                                        results[_count++] = Convert.ToString(reader.ReadInt32());
////                                    }

////                                    return _count;
////                                }

////                            case 'd': // Detailed player information
////                                {
////                                    int playercount = reader.ReadInt16();

////                                    results = new string[playercount * 4];

////                                    for (int i = 0; i < playercount; i++)
////                                    {
////                                        results[_count++] = Convert.ToString(reader.ReadByte());

////                                        int namelen = reader.ReadByte();
////                                        results[_count++] = new string(reader.ReadChars(namelen));

////                                        results[_count++] = Convert.ToString(reader.ReadInt32());
////                                        results[_count++] = Convert.ToString(reader.ReadInt32());
////                                    }

////                                    return _count;
////                                }

////                            case 'p': // Ping
////                                {
////                                    results = new string[1];

////                                    results[_count++] = timestamp[1].Subtract(timestamp[0]).Milliseconds.ToString();

////                                    return _count;
////                                }

////                            default:
////                                return _count;
////                        }
////                    }
////                }
////            }

////            catch
////            {
////                return _count;
////            }
////        }

////        public string[] Store(int count)
////        {
////            string[] rString = new string[count];

////            for (int i = 0; i < count && i < _count; i++)
////                rString[i] = results[i];

////            _count = 0;

////            return rString;
////        }
////    }
////}


