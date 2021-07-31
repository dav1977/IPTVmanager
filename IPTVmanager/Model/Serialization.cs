using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace IPTVman.Model
{
    [Serializable]
    public class ser_data
    {
        public string favorite1_1 { get; set; }
        public string favorite1_2 { get; set; }
        public string favorite2_1 { get; set; }
        public string favorite2_2 { get; set; }
        public string favorite3_1 { get; set; }
        public string favorite3_2 { get; set; }
        public string filter1 { get; set; }
        public string filter2 { get; set; }
        public string mask { get; set; }
        public string pathBD { get; set; }

        public ser_data()
        {
        }

        public void Prepare_to_save()
        {
            favorite1_1 = data.favorite1_1;
            favorite2_1 = data.favorite2_1;
            favorite3_1 = data.favorite3_1;
            favorite1_2 = data.favorite1_2;
            favorite2_2 = data.favorite2_2;
            favorite3_2 = data.favorite3_2;
            filter1 = bd_data.filter1;
            filter2 = bd_data.filter2;
            mask = bd_data.mask;
            pathBD = bd_data.path;
        }

        public void Update_new_data()
        {
            data.favorite1_1 = favorite1_1;
            data.favorite2_1 = favorite2_1;
            data.favorite3_1 = favorite3_1;
            data.favorite1_2 = favorite1_2;
            data.favorite2_2 = favorite2_2;
            data.favorite3_2 = favorite3_2;
            bd_data.filter1 = filter1;
            bd_data.filter2 = filter2;
            bd_data.mask = mask;
            bd_data.path = pathBD;
        }
    }
    public static class SETTING 
    {   
        static string path = AppDomain.CurrentDomain.BaseDirectory + "//settings.xml";
        
        public static void SaveInXmlFormat(ser_data dt)
        {         
            XmlSerializer formatter = new XmlSerializer(typeof(ser_data));

            using (Stream fStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(fStream, dt);
            }

        }

        public static ser_data ReadFromXML()
        {
            ser_data dt = new ser_data();
            XmlSerializer formatter = new XmlSerializer(typeof(ser_data));

            try
            {
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                if (fs == null || fs.Length == 0) 
                {
                    //файл еще не создан
                    dt.Prepare_to_save();
                    SaveInXmlFormat(dt);
                    return dt;
                }
                // десериализация
                using (fs)
                {
                    dt = (ser_data)formatter.Deserialize(fs);
                }
                //dt.Update_new_data();
            }
            catch (Exception Ситуация)
            {
                // Отчет обо всех возможных ошибках:
                MessageBox.Show(Ситуация.Message, "Ошибка в файле настроек ",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            return dt;
        }

        public static bool SaveToBin()
        {
           
            // Open a file and serialize the object into it in binary format.
            // EmployeeInfo.osl is the file that we are creating. 
            // Note:- you can give any extension you want for your file
            // If you use custom extensions, then the user will now 
            //   that the file is associated with your program.
            //pathq + "//data.bin"

            try
            {
                Stream stream = File.Open(path, FileMode.Create);
                BinaryFormatter bformatter = new BinaryFormatter();
                //bformatter.Serialize(stream, IPTVman.Model.data);
                stream.Close();
                return true;
            }
            catch (Exception Ситуация)
            {
                // Отчет обо всех возможных ошибках:
                MessageBox.Show(Ситуация.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            // return;

            //System.Xml.Serialization.XmlSerializer writer =
            //    new System.Xml.Serialization.XmlSerializer(typeof(SerializationDATA));
            //var path = AppDomain.CurrentDomain.BaseDirectory + "SerializationOverview.xml";
            //System.IO.FileStream file = System.IO.File.Create(path);
            //writer.Serialize(file, overview);
            //file.Close();
        }


       // static SerializationDATA loadCLASS;
        public static bool ReadFromBin()
        {
            try
            {
                //loadCLASS = new SerializationDATA();

                //Open the file written above and read values from it.
                Stream stream = File.Open(path, FileMode.Open);
                BinaryFormatter  bformatter = new BinaryFormatter();


                //loadCLASS = (SerializationDATA)bformatter.Deserialize(stream);
                stream.Close();

          
                return true;
        
            }
            catch (Exception Ситуация)
            {
                // Отчет обо всех возможных ошибках:
                MessageBox.Show(Ситуация.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }
    }


}
