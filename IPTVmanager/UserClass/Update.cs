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


        List<ParamCanal> myLISTdublicate1a;//временный
        List<ParamCanal> myLISTdublicate1b;//временный
        List<ParamCanal> myLISTdublicate2;//временный

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
            myLISTdublicate1a = new List<ParamCanal>();
            myLISTdublicate1b = new List<ParamCanal>();
            myLISTdublicate2 = new List<ParamCanal>();

            foreach (var c in glob)
            {
                myLISTdublicate1a.Add( (ParamCanal)c.Clone());
                myLISTdublicate1b.Add((ParamCanal)c.Clone());
            }
            bool first = false;
            bool next = false;
            int index = 0;
            int ind = 0;
            ParamCanal nextitem = new ParamCanal();
            ParamCanal firsttItem = null;

            foreach (var main in myLISTdublicate1a)
            {
                first = false;
                next = false;
                if (ind != 0) myLISTdublicate1a[ind - 1].name = "delete" + ind.ToString();
                index = 0;

                foreach (var j in myLISTdublicate1b)
                {
                    if (/*main.name == j.name &&*/ main.http.Trim() == j.http.Trim())
                    {
                            if (first)
                            {
                                if (!next)
                                {
                                    nextitem = (ParamCanal)j.Clone();
                                    myLISTdublicate2.Add(firsttItem);
                                    myLISTdublicate2.Add(nextitem);
                                    myLISTdublicate1b[index].name = "deleteCOPY" + index.ToString();
                                    next = true;
                                }
                                else
                                {
                                    nextitem = (ParamCanal)j.Clone();
                                    myLISTdublicate2.Add(nextitem);
                                    myLISTdublicate1b[index].name = "deleteCOPY" + index.ToString();
                                }
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
            myLISTdublicate1a = null;
            myLISTdublicate1b = null;
            return myLISTdublicate2;
        }  
      

    }
}
    
