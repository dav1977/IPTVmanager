using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Windows;
using System.Threading;
using System.Windows.Threading;


namespace IPTVman.ViewModel
{
    class ScannerRadio
    {


        List<string> lst= new List<string>();

        public void add(string s)
        {
            lst.Add(s);
        }


        public void save()
        {
            var mmF = MemoryMappedFile.CreateOrOpen("iptv_manager_scanner_radio_list1", 10000);
            using (var writer = mmF.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Write))
            {
                foreach (string s in lst)
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
