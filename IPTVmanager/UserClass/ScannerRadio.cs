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
        MemoryFile  m = new MemoryFile();


        public void add_to_save(string s)
        {
            lst.Add(s);
        }

        public void save()
        {
            m.WriteObjectToMMF("C:\\TEMP\\IPTVMANAGERSAVELINKS", lst);
        }

        public void read()
        {
            return;
            try
            {
                List<string> data = m.ReadObjectFromMMF("C:\\TEMP\\IPTVMANAGERSAVEPLAYERS") as List<string>;

                string rez = "";
                foreach (var s in data)
                {
                    rez += s;
                }

               // MessageBox.Show("sz"+data.Count+"yyy="+rez); 
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("memorymaps error=" + ex.Message); }
        }


    }
}
