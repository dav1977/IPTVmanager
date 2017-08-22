// Программа обновляет записи (Update) в таблице базы данных MS Access
using System;
using System.Data;
using System.Windows;
using System.Linq;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using System.Windows.Data;
using System.Threading;
using IPTVman.Helpers;
using IPTVman.Model;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Net.NetworkInformation;


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

        public static event Action<string> Event_Print;
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

      

        int Save_to_base()
        {
            // Сохранить в базе данных
            sql.CommandText = "UPDATE [main] SET [Name] = ?, Adress = ?  WHERE ([Id] = ?)";
            // Имя, тип и длина параметра
            sql.Parameters.Add("Name", OleDbType.VarWChar, 500, "Name");
            sql.Parameters.Add("Adress", OleDbType.VarWChar, 500, "Adress");

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
            int kol = 0;
            try
            {
                // Update возвращает количество измененных строк
                kol = adapter.Update(data, "main"); 
            }
            catch (Exception ex)
            {
                dialog.Show(ex.Message.ToString());
            }

            return kol;
        }

        void UpdateBD(string id_best, string filterMDB, string filterManager, string mask)
        {
            DataTable dt = get_table("main");
            //DataRow[] foundRows = data.Tables["main"].Select("Name = 'FOX HD'");
            if (Event_Print != null) Event_Print("Старт обновления "+ filterMDB + "    id=" + id_best + "\n");

                string work_mask="";
                foreach (var s in ViewModelMain.myLISTbase)
                {

                    if (!find_mask(mask, s.http, ref work_mask)) { continue; }
                    //if (!find_mask(mask, row[2].ToString(),  ref work_mask)) { index++; continue; };

                    int index = 0;
                    foreach (DataRow row in dt.Rows)// перебор всех строк таблицы
                    {
                        // получаем все ячейки строки
                        // object[] cells = row.ItemArray;
                        // dialog.Show((row[1].ToString() + "\n" + row[2].ToString()));

                            if (row[34].ToString() == id_best && (new Regex(work_mask).Match(s.http).Success))
                            {
                                if (s.name == row[1].ToString() && (s.ExtFilter == filterManager || filterManager == "") )
                                {

                                    data.Tables["main"].Rows[index]["Adress"] = s.http;
                                    if (Event_Print != null) Event_Print(
                                    "Обновлено " + s.name + " url = " + row[2].ToString() + "\n  новый url = " + s.http + "\n");
                                    //dialog.Show("Обновление ссылки\n " + "старый url:\n"+ row[2].ToString() +"\nновый url: \n"+ s.http);
                                    break;//обновляем только одну запись, последующие совпадения ссылки пропускаем
                               
                                }
                            }
                    index++;
                    }
                }
        }

        Task task1;
        //******************* UPDATE data in table *************************
        public async Task<string> UPDATE_DATA(CancellationToken Token, string filterMDB, string filterManager, string mask)
        {
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            task1 = Task.Run(() =>
            {
                int kol = 0;
                var tcs = new TaskCompletionSource<string>();
                try
                {
                    init_data();
                    if (Event_Print != null) Event_Print("");
                   
                    string[] word = filterMDB.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var group in word)
                    {
                        if (group == "" || group ==" " || group == "  ") continue;
                        string newgroup = group.Trim();
                        string id_best = readIDbest(newgroup);

                        if (id_best == "")
                        {
                            dialog.Show("не найдена группа " + newgroup + "(ExtFilter)\nв базе mdb");
                            return tcs.Task;
                        }

                        init_data();
                        UpdateBD(id_best.Trim(), newgroup.Trim(), filterManager.Trim(), mask.Trim());
                        kol += Save_to_base();
                    }
                    
                    tcs.SetResult("ok");
                }
                catch (OperationCanceledException e)
                {
                    tcs.SetException(e);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }

                dialog.Show("Обновлено " + kol.ToString() + " записей");
                return tcs.Task; 
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try { await task1; }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА " + e.Message.ToString());
            }
            task1.Dispose();
            task1 = null;
            return "";
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
            catch (Exception ex) { dialog.Show("ошибка " + ex.Message.ToString()); return ""; }

            DataTable dt = data.Tables[0];//выбираем первую таблицу

            // перебор всех столбцов таблицы
            //foreach (DataColumn col in dt.Columns)
            //{

            //}


            DataRow[] foundRows = data.Tables[column].Select("Name = '" + val + "'");

            //if (foundRows.Length == 0) dialog.Show("НЕ НАЙДЕНО " + val);
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
            return ret;

        }


        bool find_mask(string mask, string url, ref string maskextern)
        {
            string[] list_mask=null;
            try
            {
                list_mask = mask.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch { maskextern = ""; return false; }

            Regex regex1;
            Match r;

            foreach (string s in list_mask)
            {
                regex1 = new Regex(s.Trim(), RegexOptions.IgnoreCase);
                r = regex1.Match(url);
                if (r.Success) { maskextern = s.Trim();  return true; }
            }
            maskextern = "";
            return false;
        }

        //------------
    }
}
