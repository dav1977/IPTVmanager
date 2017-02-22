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
                //new ParamCanal { FirstName="Tom", LastName="Jones", Age=80 },
                //new ParamCanal { FirstName="Dick", LastName="Tracey", Age=40 },
                //new ParamCanal { FirstName="Harry", LastName="Hill", Age=60 },
           
            };
        }

        public static List<NEWParamCanal> GetPocoCanalFromDatabase()
        {
            //Simulate legacy database extaction of POCO classes
            //For example from ADO DataSets or EF
            return new List<NEWParamCanal>
            {
                //new NEWParamCanal { FirstName="Tom", LastName="Jones", Age=80 },
                //new NEWParamCanal { FirstName="Dick", LastName="Tracey", Age=40 },
                //new NEWParamCanal { FirstName="Harry", LastName="Hill", Age=60 },
            };
        }
    }
}
