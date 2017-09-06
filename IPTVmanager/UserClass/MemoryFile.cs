using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.IO.MemoryMappedFiles;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace IPTVman.ViewModel
{
    class MemoryFile
    {
        MemoryMappedFile mms;
        MemoryMappedFile mmr;

        /// <summary>
        /// без создания файла на диске
        /// </summary>
        /// <param name="mmfFile"></param>
        /// <param name="objectData"></param>
        public void WriteObjectToMemory(string mmfFile, object objectData)
        {
            // Convert .NET object to byte array
            byte[] buffer = ObjectToByteArray(objectData);

                try
                {
                    new Task(() =>
                    {
                        using (MemoryMappedFile mmf = MemoryMappedFile.CreateNew(mmfFile, buffer.Length))
                        {
                            // Lock
                            bool mutexCreated;
                            Mutex mutex = new Mutex(true, "MMF_IPC", out mutexCreated);

                            // Create accessor to MMF
                            using (var accessor = mmf.CreateViewAccessor(0, buffer.Length))
                            {
                                // Write to MMF
                                accessor.WriteArray<byte>(0, buffer, 0, buffer.Length);
                            }

                            mutex.ReleaseMutex();

                            Thread.Sleep(1000 * 60 * 5);
                        }

                    }).Start();
                }
                catch (Exception ex) { MessageBox.Show("base mmap save error " + ex.Message); }
            
        }


        /// <summary>
        /// с созданием файла
        /// </summary>
        /// <param name="mmfFile"></param>
        /// <param name="objectData"></param>
        public void WriteObjectToMMF(string mmfFile, object objectData)
        {
            // Convert .NET object to byte array
            byte[] buffer = ObjectToByteArray(objectData);

            try
            {
                // Create a new memory mapped file
                using (MemoryMappedFile mmf =
                       MemoryMappedFile.CreateFromFile(mmfFile, FileMode.Create, null, buffer.Length))
  
                {
                    // Create a view accessor into the file to accommmodate binary data size
                    using (MemoryMappedViewAccessor mmfWriter = mmf.CreateViewAccessor(0, buffer.Length))
                    {
                        // Write the data
                        mmfWriter.WriteArray<byte>(0, buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("mmap error "+ex.Message); }
        }

        public byte[] ObjectToByteArray(object inputObject)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();    // Create new BinaryFormatter
            MemoryStream memoryStream = new MemoryStream();             // Create target memory stream
            binaryFormatter.Serialize(memoryStream, inputObject);       // Serialize object to stream
            return memoryStream.ToArray();                              // Return stream as byte array
        }


        /// <summary>
        ///  без создания файла на диске
        /// </summary>
        /// <param name="mmfFile"></param>
        /// <returns></returns>
        public object ReadObjectFromMemory(string mmfFile)
        {
            try
            {
                byte[] buffer;

                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("sv"))
                {

                    // Create accessor to MMF
                    using (var accessor = mmf.CreateViewAccessor())
                    {
                        // Wait for the lock
                        Mutex mutex = Mutex.OpenExisting("MMF_IPC2");
                        mutex.WaitOne();

                        buffer = new byte[(int)accessor.Capacity];
                        // Read from MMF
                        accessor.ReadArray<byte>(0, buffer, 0, (int)accessor.Capacity);


                    }

                    return ByteArrayToObject(buffer);
                }
            }
            catch (Exception ex) { MessageBox.Show("base read error " + mmfFile + ex.Message); }
            return null;
        }


        /// <summary>
        /// с созданием файла
        /// </summary>
        /// <param name="mmfFile"></param>
        /// <returns></returns>
        //user_obj data = ReadObjectFromMMF("C:\\TEMP\\TEST.MMF") as user_obj; 
        public object ReadObjectFromMMF(string mmfFile)
        {
            try
            { 
                // Get a handle to an existing memory mapped file
                using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(mmfFile, FileMode.Open))
                {
                    // Create a view accessor from which to read the data
                    using (MemoryMappedViewAccessor mmfReader = mmf.CreateViewAccessor())
                    {
                        // Create a data buffer and read entire MMF view into buffer
                        byte[] buffer = new byte[mmfReader.Capacity];
                        mmfReader.ReadArray<byte>(0, buffer, 0, buffer.Length);

                        // Convert the buffer to a .NET object
                        return ByteArrayToObject(buffer);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("mmap error " + ex.Message); }
            return null;
        }

        public object ByteArrayToObject(byte[] buffer)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter(); // Create new BinaryFormatter
            MemoryStream memoryStream = new MemoryStream(buffer);    // Convert buffer to memorystream
            return binaryFormatter.Deserialize(memoryStream);        // Deserialize stream to an object
        }



        /// <summary>
        /// read (имя)   чтение MemStream
        /// </summary>
        /// <param name="name"></param>
        public void LoadCreateStream(string name)
        {
            mmr = MemoryMappedFile.OpenExisting("iptv_manager_scanner_radio_list");
        }

        public void Load()
        {
            using (mmr)
            using (var reader = mmr.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
            {
                var count = reader.ReadInt32(0);
                byte[] bytes = new byte[count];
                reader.ReadArray(sizeof(Int32), bytes, 0, count);
                var content = System.Text.ASCIIEncoding.Unicode.GetString(bytes);

            }
        }


        /// <summary>
        ///  СОЗДАНИЕ MemStream для ЗАПИСИ save (имя, размер файла)  
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sizfile"></param>
        public void SaveCreateStream(string name, int sizfile)
        {
             mms = MemoryMappedFile.CreateOrOpen(name, sizfile);
        }




        /// <summary>
        /// метод записи в созданный mms
        /// </summary>
        /// <param name="mess"></param>
        public void SaveListString(List<string> mess)
        { 
            using (var writer = mms.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Write))
            {
                foreach (string s in mess)
                    writeString(s, writer);
            }

        }
        /// <summary>
        /// метод записи в созданный mms
        /// </summary>
        /// <param name="content"></param>
        /// <param name="writer"></param>
        private static void writeString(string content, MemoryMappedViewAccessor writer)
        {
            var contentBytes = System.Text.ASCIIEncoding.Unicode.GetBytes(content);
            int count = contentBytes.Length;
            writer.Write<Int32>(0, ref count);

            writer.WriteArray<byte>(sizeof(Int32), contentBytes, 0, contentBytes.Length);
            writer.Flush();
        }

    }
}
