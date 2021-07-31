using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Xml.Serialization;


namespace IPTVman.ViewModel
{

    [Serializable]
    public class ser_data
    {
        //[XmlArray("Collection"), XmlArrayItem("Item")]
        public ObservableCollection<string> pathVST
        { get { return data.pathVST; } set { data.pathVST = pathVST; } }

        public ObservableCollection<string> workVST
        { get { return data.workVST; } set { data.workVST = workVST; } }

        public ObservableCollection<List<float>> Params
        { get { return data.listPARAM ; } set { data.listPARAM = Params; } }
    }

    public class datafile
    {
        public static void SAVEtoXML(string path)
        {
            try
            {
                ser_data dt = new ser_data();
                XmlSerializer formatter = new XmlSerializer(typeof(ser_data));

                using (Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    formatter.Serialize(fStream, dt);
                }
            }
            catch (Exception ex) { System.Windows.MessageBox.Show("Ошибка " + ex.Message); }
        }

        public static void ReadFromXML(string path)
        {
            FileStream fs = null;
            try
            {
                ser_data dt = new ser_data();
                XmlSerializer formatter = new XmlSerializer(typeof(ser_data));
                fs = new FileStream(path, FileMode.OpenOrCreate);
                if (fs.Length == 0 || fs == null)
                {
                    //файл настроек VST еще  не создан
                    if (fs != null) fs.Dispose();
                    SAVEtoXML(path);
                   return;
                }

                // десериализация
                using (fs)
                {
                    dt = (ser_data)formatter.Deserialize(fs);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, 
                    "Ошибка в файле настроек ");
            }
        }

    }
}
