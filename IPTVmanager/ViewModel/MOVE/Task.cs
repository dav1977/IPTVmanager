using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ListViewDragDropManager
{
	enum TaskDuration
	{
		Unknown,
		VeryShort,
		Short,
		Medium,
		Long,
		VeryLong
	}

    static class dataDD
    {
       public static ObservableCollection<Task> list;
       public static ObservableCollection<Task> tasks;
    }

	class Task
	{
        TaskDuration duration;
        string name;
        string playing;
        string extfilter;
        string group_title;
        string http;
        string ping;
        string logo;
        string tvg;
        bool finished;

        public Task(TaskDuration duration, string name, string playing, string ExtFilter, string group_title, string http, string _ping, string _logo, string _tvg, bool finished)
        {
            this.duration = duration;
            this.playing = playing;
            this.name = name;
            this.extfilter = ExtFilter;
            this.group_title = group_title;
            this.http = http;
            this.finished = finished;
            this.ping = _ping;
            this.logo = _logo;
            this.tvg = _tvg;
        }


        public static ObservableCollection<Task> CreateTasks()
        {
            dataDD.list = new ObservableCollection<Task>();

            if (IPTVman.Model.data.mode_radio_from_select)
            {
                foreach (var s in IPTVman.ViewModel.ViewModelMain.myLISTselect)
                {
                    dataDD.list.Add(new Task(TaskDuration.VeryShort, s.name, "", s.ExtFilter, s.group_title, s.http, s.ping, s.logo, s.tvg_name, false));
                }
            }
            else
            foreach (var s in IPTVman.ViewModel.ViewModelMain.myLISTbase)
            {
                dataDD.list.Add(new Task(TaskDuration.VeryShort,  s.name, "", s.ExtFilter,s.group_title,s.http , s.ping, s.logo, s.tvg_name , false));
            }
            return dataDD.list;
		}


        public static ObservableCollection<Task> CreateTasks2(List<IPTVman.Model.ParamCanal> LST)
        {;

            dataDD.list = new ObservableCollection<Task>();

            if (LST == null) return dataDD.list;
                foreach (var s in LST)
                {
                    dataDD.list.Add(new Task(TaskDuration.VeryShort, s.name, "", s.ExtFilter, s.group_title, s.http, s.ping, s.logo, s.tvg_name, false));
                }
            return dataDD.list;
        }

        public bool Finished
		{
			get { return this.finished; }
			set { this.finished = value; }
		}

		public TaskDuration Duration
		{
			get { return this.duration; }
		}

		public string Name
		{
			get { return this.name; }
		}

        public string Playing
        {
            get { return this.playing; }
            set { this.playing = value; }
        }

        public string ExtFilter
        {
            get { return this.extfilter; }
        }
        public string Group_title
        {
			get { return this.group_title; }
		}
         public string Http
        {
			get { return this.http; }
		}
        public string Ping
        {
            get { return this.ping; }
        }
        public string Logo
        {
            get { return this.logo; }
        }
        public string Tvg
        {
            get { return this.tvg; }
        }
    }
}
