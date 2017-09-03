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

namespace IPTVman.ViewModel
{
    class MemoryFile
    {
        MemoryMappedFile mms;
        MemoryMappedFile mmr;

        /// <summary>
        /// read (имя)
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
        /// save (имя, размер файла)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sizfile"></param>
        public void SaveCreateStream(string name, int sizfile)
        {
             mms = MemoryMappedFile.CreateOrOpen(name, sizfile);
        }


        public void SaveListString(List<string> mess)
        { 
            using (var writer = mms.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Write))
            {
                foreach (string s in mess)
                    writeString(s, writer);
            }

        }

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
