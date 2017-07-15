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

      

        void Save_to_base()
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
            try
            {
                // Update возвращает количество измененных строк
                var kol = adapter.Update(data, "main");
                dialog.Show("Обновлено " + kol.ToString() + " записей");
            }
            catch (Exception ex)
            {
                dialog.Show(ex.Message.ToString());
            }


        }


        Task task1;
        //******************* UPDATE data in table *************************
        public async Task<string> UPDATE_DATA(CancellationToken Token, string filterMDB, string filterManager, string mask)
        {
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            string except = "";
            task1 = Task.Run(() =>
            {
                try
                {

                    init_data();
                    string id_best = readIDbest(filterMDB);
                    //int id_bestcod = int.Parse(id_best);

                    if (id_best == "")
                    {
                        dialog.Show("не найдена группа " + filterMDB + "(ExtFilter)\nв базе mdb");
                        return "";
                    }

                    init_data();
                    DataTable dt = get_table("main");

                    if (Event_Print != null) Event_Print("");
                    if (Event_Print != null) Event_Print("Старт обновления id=" + id_best + "\n");
                    //DataRow[] foundRows = data.Tables["main"].Select("Name = 'FOX HD'");

                    int index = 0;
                    foreach (DataRow row in dt.Rows)// перебор всех строк таблицы
                    {
                        if (!find_mask(mask, row[2].ToString())) { index++; continue; }
                        // получаем все ячейки строки
                        // object[] cells = row.ItemArray;
                        // dialog.Show((row[1].ToString() + "\n" + row[2].ToString()));
                        if (row[34].ToString() == id_best)
                        {
                            //if (Event_Print != null) Event_Print("Анализ "+ row[1].ToString()+"\n");

                            foreach (var s in ViewModelMain.myLISTbase)
                            {
                                if (s.name == row[1].ToString() && s.ExtFilter == filterManager && s.http != "")
                                {
                                    if (Event_Print != null) Event_Print(
                                        "Обновлено " + s.name + " url = " + row[2].ToString() + "\n  новый url = " + s.http + "\n");
                                    //dialog.Show("Обновление ссылки\n " + "старый url:\n"+ row[2].ToString() +"\nновый url: \n"+ s.http);
                                    data.Tables["main"].Rows[index]["Adress"] = s.http;
                                    break;//обновляем только одну запись
                                }
                            }
                        }
                        index++;
                    }

                    Save_to_base();

                }
                catch (OperationCanceledException e)
                {
                    except += e.Message.ToString();
                }
                catch (Exception e)
                {
                    except += e.Message.ToString();
                }

                return "ok";
            });
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            try
            {
                await task1;  
            }
            catch (OperationCanceledException e)
            {
                except += e.Message.ToString();
            }
            catch (Exception e)
            {
                except += e.Message.ToString();
            }

            //dialog.Show("Статус закрытя "+ task1.Status.ToString());
            if (except != "") dialog.Show("ОШИБКА " + except);
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


        bool find_mask(string mask, string url)
        {
            Regex regex1;
            Match r;
           
                regex1 = new Regex(mask, RegexOptions.IgnoreCase);
                r = regex1.Match(url);
                if (r.Success) return true;
       
            return false;
        }

        //------------
    }
}
