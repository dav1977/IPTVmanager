using System.Collections.Generic;
using IPTVman.Data;
using System;
using System.Threading;

namespace IPTVman.Model
{
    class ParamCanalnelBusinessObject
    {
        public enum StatusType
        {
            Offline = 0,
            Online = 1
        }
        
        public event EventHandler CanalChanged;

        List<NEWParamCanal> Canal { get; set; }
        public StatusType Status { get; set; }
        public string ReportTitle { get; set; }

        Timer StatusTimer;

        public ParamCanalnelBusinessObject()
        {
            Canal = FakeDatabaseLayer.GetPocoCanalFromDatabase();
            StatusTimer = new Timer(StatusChangeTick, null, 1000, 1000);
        }

        void StatusChangeTick(object state)
        {
            Status = Status == StatusType.Offline ? StatusType.Online : StatusType.Offline;
        }

        public List<NEWParamCanal> GetEmployees()
        {
            return Canal;
        }

        public void AddParamCanal(NEWParamCanal ParamCanal)
        {
            Canal.Add(ParamCanal);
            OnCanalChanged();
        }

        public void DeleteParamCanal(NEWParamCanal ParamCanal)
        {
            Canal.Remove(ParamCanal);
            OnCanalChanged();
        }

        public void UpdateParamCanal(NEWParamCanal ParamCanal)
        {

        }

        void OnCanalChanged()
        {
            if (CanalChanged != null)
                CanalChanged(this, null);
            
           
        }
    }
}
