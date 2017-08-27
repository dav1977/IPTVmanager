using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using IPTVman.Model;
using IPTVman.Helpers;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Data;
using System.Threading.Tasks;

namespace IPTVman.ViewModel
{
    class Update_Collection
    {
        private List<ParamCanal> myLISTbase;
        private List<ParamCanal> myLISTfullNEW;

        /// <summary>
        /// 
        /// </summary>
        public Update_Collection()
        {

        }

        void UPD_normal()
        {
            Match m1, m2, m3, m4;
            Regex regex1 = new Regex(data.f1, RegexOptions.IgnoreCase);
            Regex regex2 = new Regex(data.f2, RegexOptions.IgnoreCase);
            Regex regex3 = new Regex(data.f3, RegexOptions.IgnoreCase);
            Regex regex4 = new Regex(data.f4);


            myLISTbase.Clear();
            foreach (var c in myLISTfullNEW)
            {
                //Trace.WriteLine("z = " + ViewModelMain._filter + "n="+ c.name + " ");


                bool not_filter = false;
                if (Model.data.f1 == "" && Model.data.f2 == "" && Model.data.f3 == "" && Model.data.f4 == "") not_filter = true;

                if (not_filter) { myLISTbase.Add(c); continue; }


                m1 = regex1.Match(c.name);
                if ((m1.Success && Model.data.f2 == "" && Model.data.f3 == "" && Model.data.f4 == "")) myLISTbase.Add(c);

                else
                {
                    //----------------------------------
                    m2 = regex2.Match(c.ExtFilter);

                    if (m2.Success && Model.data.f2 != "" && Model.data.f3 == "") myLISTbase.Add(c);
                    //----------------------------------


                    //----------------------------------
                    m3 = regex3.Match(c.group_title);

                    if (m3.Success && Model.data.f3 != "" && Model.data.f2 == "") myLISTbase.Add(c);
                    //----------------------------------

                    if (m3.Success && Model.data.f3 != "" && m2.Success && Model.data.f2 != "") myLISTbase.Add(c);

                }

                if (Model.data.f4 != "" && (m1.Success || Model.data.f1 == ""))
                {

                    //all
                    if (c.http != null)
                    {
                        m4 = regex4.Match(c.http);
                        if ((m4.Success)) myLISTbase.Add(c);
                    }
                    if (c.ping != null)
                    {
                        m4 = regex4.Match(c.ping);
                        if ((m4.Success)) myLISTbase.Add(c);
                    }
                    if (c.tvg_name != null)
                    {
                        m4 = regex4.Match(c.tvg_name);
                        if ((m4.Success)) myLISTbase.Add(c);
                    }


                }

            }
        }



        void UPD_best()
        {
            Model.data.f2 = Model.data.best1;
            Model.data.f3 = Model.data.best2;

            Match m1;
            Regex regex1 = new Regex(Model.data.f1, RegexOptions.IgnoreCase);

            myLISTbase.Clear();
            foreach (var c in myLISTfullNEW)
            {
                m1 = regex1.Match(c.name);


                if (( //(m1.Success && data.f2 == c.ExtFilter && data.f3 == c.group_title)
                    (Model.data.best1 == c.ExtFilter && Model.data.best2 == c.group_title) ||
                    (Model.data.best1 == c.ExtFilter && Model.data.best2 == "") ||
                    (Model.data.best1 == "" && Model.data.best2 == c.group_title)
                    ) && (Model.data.f1 == "" || m1.Success))
                    myLISTbase.Add(c);
            }
        }

        void UPD_dub()
        {
            myLISTbase.Clear();

            foreach (var c in IPTVman.ViewModel.ViewModelMain.myLISTdub)
            {
                    myLISTbase.Add((ParamCanal)c.Clone());
            }
        }

        typefilter last = typefilter.normal;
        public void UPDATE_FILTER(typefilter type, List<ParamCanal> p1, List<ParamCanal> p2)
        {
            myLISTbase = p1;
            myLISTfullNEW = p2;

            if (Model.data.f1 == null) Model.data.f1 = "";
            if (Model.data.f2 == null) Model.data.f2 = "";
            if (Model.data.f3 == null) Model.data.f3 = "";
            if (Model.data.f4 == null) Model.data.f4 = "";

            if (myLISTfullNEW != null && myLISTbase != null)
            {
                if (type == typefilter.last) type = last;
                if (type == typefilter.normal) UPD_normal();
                if (type == typefilter.best) UPD_best();
                if (type == typefilter.dublicate) { UPD_dub(); }

                last = type;
            }

        }

        public typefilter lastfilter()
        {
            return last;
        }

        public void UPDATE_BEST(string b1, string b2)
        {
            if (b1 != null) Model.data.best1 = b1;
            if (b2 != null) Model.data.best2 = b2;

        }

        public List<ParamCanal> find_dublicate(List<ParamCanal> glob)
        {
            const string _NAME = "$delete$_COPY";
            List<ParamCanal> myLISTdublicate1a;//временный 1
            List<ParamCanal> myLISTdublicate1b;//временный 2
            List<ParamCanal> myLISTdublicate2;//временный итоговый

            myLISTdublicate1a = new List<ParamCanal>();
            myLISTdublicate1b = new List<ParamCanal>();
            myLISTdublicate2 = new List<ParamCanal>();
            int ct = 0;
            foreach (var c in glob)
            {
                myLISTdublicate1a.Add( (ParamCanal)c.Clone());
                myLISTdublicate1b.Add((ParamCanal)c.Clone());
            }
            bool first = false;
            int index = 0;
            int ind = 0;
            ParamCanal nextitem = null;
            ParamCanal firsttItem = null;

            myLISTdublicate2.Clear();
            //1111111111111111
            foreach (var main in myLISTdublicate1a)
            {
                first = false;
                if (ind != 0) myLISTdublicate1a[ind - 1].name = _NAME + ind.ToString();
                index = 0;
                if (main.http == null) continue;
                string mn = main.http.Trim();

                //22222222222222222
                foreach (var j in myLISTdublicate1b)
                {
                    if (j.http == null) continue;
                    if (mn == j.http.Trim() )
                    {
                            if (first)
                            {
                                nextitem = (ParamCanal)j.Clone();
                                if (firsttItem!=null) myLISTdublicate2.Add(firsttItem);
                                firsttItem = null;
                                myLISTdublicate2.Add(nextitem);
                                ct++; 
                                myLISTdublicate1b[index].name = _NAME +ind.ToString() + index.ToString();
                                myLISTdublicate1b[index].http = _NAME + ind.ToString() + index.ToString();
                            }
                            else
                            {//нахождение самого себя
                                first = true;
                                firsttItem = (ParamCanal)main.Clone();                       
                            }
                    }
                    index++;
                }
                ind++;
                GUI.progressbar++;
            }

           
            //чистка
            List<ParamCanal> resul = new List<ParamCanal>();
            foreach (var c in myLISTdublicate2)
            {
                if (c.name.Contains("$delete$")) { }
                else resul.Add((ParamCanal)c.Clone());
            }

            myLISTdublicate1a = null;
            myLISTdublicate1b = null;
            myLISTdublicate2 = null;
            return resul;
        }  
      

    }
}
    
