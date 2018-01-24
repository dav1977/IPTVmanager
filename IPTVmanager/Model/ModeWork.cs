﻿using System;
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
        /// не выводить сообщения о пропущеных ссылках
        /// </summary>
        public static bool skip_message_skiplinks = false;

        /// <summary>
        /// Отключить обрезание скобок
        /// </summary>
        public static bool skip_obrez_skobki = false;


        public static bool OpenWindow_db_update = false;
        public static bool OpenWindow_radio = false;
        public static bool OpenWindow_db_updateREADY = false;
        public static bool OpenWindow_radioREADY = false;

        public static bool CLOSE_ALL = false;

        public static void ResetMODEApplication()
        {
            skip_message_skiplinks = false;
            skip_obrez_skobki = false;

        }
    }
}