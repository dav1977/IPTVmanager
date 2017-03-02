using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using IPTVman.Model;

namespace IPTVman.Data
{
    class FakeDatabaseLayer
    {
        public static ObservableCollection<ParamCanal> GetCanalFromDatabase()
        {
            //Simulate database extaction
            //For example from ADO DataSets or EF
            return new ObservableCollection<ParamCanal>
            {
                //new ParamCanal { Title="Tom", ExtFilter="Jones", group_title=80 },
                //new ParamCanal { Title="Dick", ExtFilter="Tracey", group_title=40 },
                //new ParamCanal { Title="Harry", ExtFilter="Hill", group_title=60 },
           
            };
        }

        public static List<NEWParamCanal> GetPocoCanalFromDatabase()
        {
            //Simulate legacy database extaction of POCO classes
            //For example from ADO DataSets or EF
            return new List<NEWParamCanal>
            {
                //new NEWParamCanal { Title="Tom", ExtFilter="Jones", group_title=80 },
                //new NEWParamCanal { Title="Dick", ExtFilter="Tracey", group_title=40 },
                //new NEWParamCanal { Title="Harry", ExtFilter="Hill", group_title=60 },
            };
        }
    }
}
