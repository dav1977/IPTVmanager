// Программа обновляет записи (Update) в таблице базы данных MS Access
using System;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.Data.OleDb;
using System.Threading.Tasks;

namespace IPTVman.ViewModel
{

    /// <summary>
    /// Класс для работы с базой данных Access
    /// </summary>
    class Access
    {
        DataSet data;
        OleDbDataAdapter adapter;
        OleDbConnection connector;
        OleDbCommand sql;

        public string error = "";
        public void connect(string path)
        {
            try
            {
                connector = new OleDbConnection(
                  // "Data Source=\"D:\\1.mdb\";User " +
                  "Data Source="+ "\""+ path + "\";User " +  //"D:\\1.mdb"
                    "ID=Admin;Provider=\"Microsoft.Jet.OLEDB.4.0\";");
                if (connector != null) connector.Open();
            }
            catch (Exception ex) { error = ex.Message.ToString(); }
        }

        public bool is_connect()
        {
            if (connector == null) return false;
            if (connector.State == ConnectionState.Open) return true;
            return false;
        }
        void init_data()
        {
            data = new DataSet();
            sql = new OleDbCommand();

        }
        DataTable get_table(string table)
        {

            // string column = "main";
            // Читать из БД:
            if (connector.State == ConnectionState.Closed) connector.Open();
            adapter = new OleDbDataAdapter("Select * From [" + table + "]", connector);
            // Заполняем DataSet результатом SQL-запроса
            try
            {
                adapter.Fill(data, table);
            }
            catch { }

            return data.Tables[0];//выбираем первую таблицу

        }

        string readIDbest(string val)
        {
            string ret = "";
            string column = "ExtFilter";

            // Читать из БД:
            if (connector.State == ConnectionState.Closed) connector.Open();
            adapter = new OleDbDataAdapter("Select * From [" + column + "]", connector);
            // Заполняем DataSet результатом SQL-запроса
            try
            {
                adapter.Fill(data, column);
            }
            catch (Exception ex) { MessageBox.Show("ошибка " + ex.Message.ToString()); return ""; }

            DataTable dt = data.Tables[0];//выбираем первую таблицу

            // перебор всех столбцов таблицы
            //foreach (DataColumn col in dt.Columns)
            //{

            //}


            DataRow[] foundRows = data.Tables[column].Select("Name = '" + val + "'");

            if (foundRows.Length == 0) MessageBox.Show("НЕ НАЙДЕНО " + val);
            // перебор всех строк таблицы
            foreach (DataRow row in foundRows)
            {

                // получаем все ячейки строки
                object[] cells = row.ItemArray;

                ret += cells[0].ToString();// первый элемент
                //foreach (object cell in cells)
                //{
                //   ret += cell.ToString();
                //}
            }


            connector.Close();
            //  MessageBox.Show(ret);
            return ret;

        }


        void Save_to_base()
        {
            // Сохранить в базе данных
            sql.CommandText = "UPDATE [main] SET [Name] = ?, Adress = ?  WHERE ([Id] = ?)";
            // Имя, тип и длина параметра
            sql.Parameters.Add("Name", OleDbType.VarWChar, 50, "Name");
            sql.Parameters.Add("Adress", OleDbType.VarWChar, 50, "Adress");



            sql.Parameters.Add
            (
                new OleDbParameter
                (
                "Original_Id",
                OleDbType.Integer,
                0,
                ParameterDirection.Input,
                false,
                (Byte)0,
                (Byte)0,
                "Id",
                System.Data.DataRowVersion.Original,
                null
                )
            );



            adapter.UpdateCommand = sql;
            sql.Connection = connector;
            try
            {
                // Update возвращает количество измененных строк
                var kol = adapter.Update(data, "main");
                MessageBox.Show(
                          "Обновлено " + kol.ToString() + " записей");
            }
            catch (Exception Ситуация)
            {
                MessageBox.Show(Ситуация.Message, "Недоразумение");
            }


        }






        //******************* UPDATE data in table *************************
        public void UPDATE_ITEM()
        {
            init_data();
            string id_best = readIDbest("best");
            if (id_best == "") MessageBox.Show("не найдена группа");

            init_data();
            DataTable dt = get_table("main");

            //DataRow[] foundRows = data.Tables["main"].Select("Name = 'FOX HD'");

            int index = 0;
            // перебор всех строк таблицы
            foreach (DataRow row in dt.Rows)
            {
                // получаем все ячейки строки
                // object[] cells = row.ItemArray;
                // MessageBox.Show((row[1].ToString() + "\n" + row[2].ToString()));

                if (row[1].ToString() == "ТНТ" && row[34].ToString() == id_best)
                {
                    MessageBox.Show(index.ToString() + "  " + row[0].ToString());
                    data.Tables["main"].Rows[index]["Adress"] = "lllllllllllllll ";

                }
                index++;

            }

            Save_to_base();



        }


    }
}
