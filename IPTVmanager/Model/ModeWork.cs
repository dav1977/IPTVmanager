using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPTVman.Model
{

    /// <summary>
    /// Режим работы приложения
    /// </summary>
    public static class ModeWork
    {
    
        /// <summary>
        ///  ИДЕТ ПРОЦЕСС скрипта (длительная операция)
        /// </summary>
        public static bool process_script = false;

        /// <summary>
        /// не выводить сообщения о пропущеных ссылках
        /// </summary>
        public static bool en_skip_message_skiplinks = false;

        /// <summary>
        /// Отключить обрезание скобок
        /// </summary>
        public static bool skip_obrez_skobki = false;

        /// <summary>
        /// Включить обновление
        /// </summary>
        public static bool enable_update = false;
        /// <summary>
        /// Добавить файл
        /// </summary>
        public static bool add = false;
        public static string addpath = "";
        /// <summary>
        /// флаг события добавления
        /// </summary>
        public static bool flag_add = false;

        //---------------------------------------------
        public static bool OpenWindow_db_update = false;
        public static bool OpenWindow_radio = false;
        public static bool OpenWindow_db_updateREADY = false;
        public static bool OpenWindow_radioREADY = false;

        public static bool CLOSE_ALL = false;

        public static void ResetMODEApplication()
        {
            en_skip_message_skiplinks = false;
            skip_obrez_skobki = false;
        }
    }
}
