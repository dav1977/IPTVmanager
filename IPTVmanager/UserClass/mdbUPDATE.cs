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
using System.Diagnostics;
using System.Data.SQLite;


namespace IPTVman.ViewModel
{

    /// <summary>
    /// Класс для работы с базой данных Access и SQLLite
    /// </summary>
    class DataBaseWork
    {
        DataSet data_set;

        OleDbDataAdapter adapter;
        OleDbConnection connector;
        OleDbCommand sql;
        string NameTablMDB = "main";


        SQLiteDataAdapter adapterSQL;
        SQLiteConnection m_dbConn;
        SQLiteCommand m_sqlCmd;
        DataTable tabl;
        public string sqllstr = "";
        string NameTablSQLlite = "Channels";

        Task task1;
        public CancellationTokenSource cts1 = new CancellationTokenSource();
        public CancellationToken cancellationToken;

        public static event Action<string> Event_Print;
        public string error = "";
        public void connect(string path)
        {
            cancellationToken = cts1.Token;

            try
            {
                if (bd_data.path.Contains("mdb"))
                {
                    connector = new OleDbConnection(
                     // "Data Source=\"D:\\1.mdb\";User " +
                     "Data Source=" + "\"" + path + "\";User " +  //"D:\\1.mdb"
                       "ID=Admin;Provider=\"Microsoft.Jet.OLEDB.4.0\";");
                    if (connector != null) connector.Open();
                }
                else
                {
                    m_dbConn = new SQLiteConnection();
                    sqllstr = "Data Source = " + path +";";

                    m_dbConn = new SQLiteConnection(sqllstr);
                    m_dbConn.Open();

                    m_sqlCmd = new SQLiteCommand();
                    m_sqlCmd.Connection = m_dbConn;


                  
                }
            }
            catch (Exception ex) { error = ex.Message.ToString(); }
        }

        public bool is_connect()
        {
            if (connector == null)
            {
                if (m_dbConn == null) return false;
                if (m_dbConn.State == ConnectionState.Open) return true;
            }
            if (connector.State == ConnectionState.Open) return true;
            return false;
        }
        void init_data()
        {
            data_set = new DataSet();

            if (bd_data.path.Contains("mdb"))
            {
                sql = new OleDbCommand();
            }
            else
            {
                m_sqlCmd = new SQLiteCommand();
                m_sqlCmd.Connection = m_dbConn;
            }
        }
        DataTable get_table(string table)
        {
            // Заполняем DataSet результатом SQL-запроса
            try
            {
                if (connector.State == ConnectionState.Closed) connector.Open();
                adapter = new OleDbDataAdapter("Select * From [" + table + "]", connector);
                adapter.Fill(data_set, table);
            }
            catch (SQLiteException ex)
            { 
                MessageBox.Show("Error get_tabl: " + ex.Message);
            }

            return data_set.Tables[0];//выбираем первую таблицу
        }

        DataTable get_tableSQLL(string table)
        {
            DataTable dTable = new DataTable();
            String sqlQuery;

            if (m_dbConn.State == ConnectionState.Closed) m_dbConn.Open();
      
                sqlQuery = "SELECT * FROM " + NameTablSQLlite;
                adapterSQL = new SQLiteDataAdapter(sqlQuery, m_dbConn);
                adapterSQL.Fill(data_set, table);

       
            return data_set.Tables[0];//выбираем первую таблицу   dTable;

        }

        public DataTable GetDataTable(SQLiteConnection con, string tablename)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            DataTable DT = new DataTable();
            if (con.State != ConnectionState.Open)  con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = string.Format("SELECT * FROM {0}", tablename);
            var adapter = new SQLiteDataAdapter(cmd);
            adapter.AcceptChangesDuringFill = false;
            adapter.Fill(DT);
            con.Close();
            DT.TableName = tablename;
            foreach (DataRow row in DT.Rows)
            {
                row.AcceptChanges();
            }
            return DT;
        }


        public int SaveDataTable(SQLiteConnection con, DataTable DT, string nameTABL)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand();
                if (con.State!= ConnectionState.Open) con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = string.Format("SELECT * FROM {0}", nameTABL);
                var adapter = new SQLiteDataAdapter(cmd);
               // adapter.AcceptChangesDuringUpdate = false;
                SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter);
                int k=adapter.Update(DT);
                
                con.Close();
                return k;
            }
            catch (SQLiteException Ex)
            {
                System.Windows.MessageBox.Show(Ex.Message);
            }
            catch (Exception Ex)
            {
                System.Windows.MessageBox.Show(Ex.Message);
            }
            return 0;
        }


        int Save_to_SQLL()
        {    
            int kol = 0;
            kol = SaveDataTable(m_dbConn, tabl, NameTablSQLlite);
            return kol;
        }

        int Save_to_mdb()
        {
            // Сохранить в базе данных
            sql.CommandText = "UPDATE ["+ NameTablMDB + "] SET [Name] = ?, Adress = ?  WHERE ([Id] = ?)";
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
                //  возвращает количество измененных строк
                kol = adapter.Update(data_set,  NameTablMDB); 
            }
            catch (Exception ex)
            {
                dialog.Show(ex.Message.ToString());
            }

            return kol;
        }


       


        void UpdateBD(string id_best, byte typeBD, string filterMDB, string filterManager, string mask)
        {
             byte idGroup=0;

            if (typeBD == 1)  { idGroup = 34; tabl = get_table(NameTablMDB); }
            else
            { idGroup = 21; tabl = get_tableSQLL(NameTablSQLlite); }


            if (Event_Print != null) Event_Print("поиск ссылок ...\n");

            string work_mask="";
                foreach (var s in ViewModelMain.myLISTbase)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    if (!find_mask(mask, s.http, ref work_mask)) { continue; }
                    int index = 0;
  
                    foreach (DataRow row in tabl.Rows)// перебор всех строк таблицы
                    {
                        if (cancellationToken.IsCancellationRequested) break;

                   
                      if (row[idGroup].ToString() == id_best)//выбор Группы
                      {

                        Debug.WriteLine("name==" + row[1].ToString());


                        ////выбор имени первого
                        if (s.name == row[1].ToString() && (s.ExtFilter == filterManager || filterManager == ""))
                        {
                            Debug.WriteLine("find ok ="+ s.name+" "+ row[2].ToString());

                            if (typeBD == 1)//************* MDB
                            {
                                // if (Event_Print != null) Event_Print("<<" + row[1].ToString()+ "  uuuu=" + row[2].ToString());
                                if (find_mask(work_mask, (string)data_set.Tables["main"].Rows[index]["Adress"]))
                                {

                                    if (Event_Print != null) Event_Print(
                                    "Обновлено " + s.name + " url = " + row[2].ToString() + "\n  новый url = " + s.http + "\n");

                                    data_set.Tables[NameTablMDB].Rows[index]["Adress"] = s.http;
                                   
                                }
                            }
                            else//****************** SQLlite
                            {
                                if (find_mask(work_mask, row[2].ToString()) )
                                {
                                    data_set.Tables[0].Rows[index][2] = s.http;

                                    if (Event_Print != null) Event_Print(
                                    "Обновлено " + s.name + " url = " + data_set.Tables[0].Rows[index].ItemArray[2].ToString() +
                                    "\n  новый url = " + data_set.Tables[0].Rows[index][2].ToString() + "\n");
                                }
                            }

                            break;//обновляем только одну запись, последующие совпадения ссылки пропускаем



                        }
                      }
                        index++;
                    }
                }
        }

        public void Stop()
        {
            if (cts1!=null) cts1.Cancel();
        }
        /// <summary>
        /// Обновление в БАЗЕ
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="filterMDB"></param>
        /// <param name="filterManager"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        public async Task<string> UPDATE_DATA(CancellationToken Token, byte typeBD, string filterMDB, string filterManager, string mask)
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
                        if (cancellationToken.IsCancellationRequested) break;

                        if (group == "" || group ==" " || group == "  " || group==null) continue;
                        string newgroup = group.Trim();

                        if (Event_Print != null) Event_Print("Старт обновления " + group + "\n");


                        string id_best="";
                        if (typeBD == 1)  id_best = readIDbest(newgroup);
                        if (typeBD == 2) id_best = readIDbestSQLL(newgroup);

                        if (Event_Print != null) Event_Print("ID " + group + " = "+ id_best .ToString() + "\n");

                        if (id_best == "")
                        {
                            dialog.Show("не найдена группа " + newgroup + "(ExtFilter)\nв базе mdb");
                            return tcs.Task;
                        }

                        init_data();

                        UpdateBD(id_best.Trim(), typeBD, newgroup.Trim(), filterManager.Trim(), mask.Trim());

                        if (typeBD == 1) kol += Save_to_mdb();
                        else kol += Save_to_SQLL();
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


                if (Event_Print != null) Event_Print(
                              "Обновлено "  +kol.ToString() + " записей");

               // dialog.Show("Обновлено \n" + kol.ToString() + " записей");
                return tcs.Task; 
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try { await task1; }
            catch (Exception e)
            {
                dialog.Show("ОШИБКА БД " + e.Message.ToString());
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
                adapter.Fill(data_set, column);
            }
            catch (Exception ex) { dialog.Show("ошибка id " + ex.Message.ToString()); return ""; }

            DataTable dt = data_set.Tables[0];//выбираем первую таблицу

          
            DataRow[] foundRows = data_set.Tables[column].Select("Name = '" + val + "'");

            //if (foundRows.Length == 0) dialog.Show("НЕ НАЙДЕНО " + val);
            // перебор всех строк таблицы
            foreach (DataRow row in foundRows)
            {
                // получаем все ячейки строки
                object[] cells = row.ItemArray;

                ret += cells[0].ToString();// первый элемент
             
            }
            connector.Close();
            return ret;

        }


        string readIDbestSQLL(string val)
        {
            string ret = "";
            string column = "ExtFilter";
            
            DataTable dTable = new DataTable();
            String sqlQuery;

    
                // Читать из БД:
                if (m_dbConn.State == ConnectionState.Closed) m_dbConn.Open();

                sqlQuery = "SELECT * FROM "+ column;
                adapterSQL = new SQLiteDataAdapter(sqlQuery, m_dbConn);
                adapterSQL.Fill(dTable);

            //string log = "";
           
            if (dTable.Rows.Count > 0)
            {
               
                for (int i = 0; i < dTable.Rows.Count; i++)
                {
                    var d = dTable.Rows[i].ItemArray;

                    Debug.WriteLine("  find=" + d[1].ToString());
                    // log += d[1].ToString();

                    if (d[1].ToString() == val)
                    {
                        ret = d[0].ToString();
                       
                        break;
                    }
                   // Debug.WriteLine(d[1].ToString()+" = "+ d[0].ToString());

                }
            }
            m_dbConn.Close();

           // if (ret == "") MessageBox.Show("no data list: "+log);
            return ret;

        }
        string[] list_mask = null;
        bool find_mask(string mask, string url, ref string maskextern)
        {
            try
            {
                list_mask = mask.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                maskextern = "";
                return false;
            }

            foreach (string s in list_mask)
            {
                if (new Regex(s.Trim()).Match(url).Success) { maskextern = s.Trim(); return true; }
            }
            maskextern = "";
            return false;
        }

        bool find_mask(string workmask, string url)
        {
            if (new Regex(workmask).Match(url).Success) return true; 
            return false;
        }

        //------------
    }
}
