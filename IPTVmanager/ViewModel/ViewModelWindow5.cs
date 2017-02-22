using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPTVman.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using IPTVman.Helpers;
using System.Windows.Data;

namespace IPTVman.ViewModel
{
    class ViewModelWindow5 : ViewModelBase
    {
        ParamCanalnelBusinessObject ParamCanalnel; // The sealed business object (database layer, web service, etc)

        ObservableCollection<NEWParamCanal> _Canal;
        public ObservableCollection<NEWParamCanal> Canal
        {
            get
            {
                _Canal = new ObservableCollection<NEWParamCanal>(ParamCanalnel.GetEmployees());
                return _Canal;
            }
        }

        public string ReportTitle
        {
            get
            {
                return ParamCanalnel.ReportTitle;
            }
            set
            {
                if (ParamCanalnel.ReportTitle != value)
                {
                    ParamCanalnel.ReportTitle = value;
                    RaisePropertyChanged("ReportTitle");
                }
            }
        }

        IPTVman.Model.ParamCanalnelBusinessObject.StatusType _BoStatus;
        public IPTVman.Model.ParamCanalnelBusinessObject.StatusType BoStatus
        {
            get
            {
                return _BoStatus;
            }
            set
            {
                if (_BoStatus != value)
                {
                    _BoStatus = value;
                    RaisePropertyChanged("BoStatus");
                }
            }
        }

        object _SelectedParamCanal;
        public object SelectedParamCanal
        {
            get
            {
                return _SelectedParamCanal;
            }
            set
            {
                if (_SelectedParamCanal != value)
                {
                    _SelectedParamCanal = value;
                    RaisePropertyChanged("SelectedParamCanal");
                }
            }
        }

        public int SelectedIndex { get; set; }

        BindingGroup _UpdateBindingGroup;
        public BindingGroup UpdateBindingGroup
        {
            get
            {
                return _UpdateBindingGroup;
            }
            set
            {
                if (_UpdateBindingGroup != value)
                {
                    _UpdateBindingGroup = value;
                    RaisePropertyChanged("UpdateBindingGroup");
                }
            }
        }

        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand key_ADDCommand { get; set; }
        public RelayCommand DeleteUserCommand { get; set; }

        DispatcherTimer checkStatusTimer;

        public ViewModelWindow5()
        {
            ParamCanalnel = new ParamCanalnelBusinessObject();
            ParamCanalnel.CanalChanged += new EventHandler(ParamCanalnel_CanalChanged);

            CancelCommand = new RelayCommand(DoCancel);
            SaveCommand = new RelayCommand(DoSave);
            key_ADDCommand = new RelayCommand(key_ADD);
            DeleteUserCommand = new RelayCommand(DeleteUser);

            UpdateBindingGroup = new BindingGroup { Name = "Group1" };

            checkStatusTimer = new DispatcherTimer();
            checkStatusTimer.Interval = TimeSpan.FromMilliseconds(500);
            checkStatusTimer.Tick += new EventHandler(CheckStatus);
            checkStatusTimer.Start();

            CheckStatus(null, null);
        }

        void CheckStatus(object sender, EventArgs e)
        {
            //Periodically checks if the property has changed
            if (_BoStatus != ParamCanalnel.Status)
                BoStatus = ParamCanalnel.Status;
        }

        void ParamCanalnel_CanalChanged(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    RaisePropertyChanged("Canal");
                    

                }));
        }

        void DoCancel(object param)
        {
            UpdateBindingGroup.CancelEdit();
            if (SelectedIndex == -1)    //This only closes if new - just to show you how CancelEdit returns old values to bindings
                SelectedParamCanal = null;
        }

        void DoSave(object param)
        {
            UpdateBindingGroup.CommitEdit();
            var ParamCanal = SelectedParamCanal as NEWParamCanal;
            if (SelectedIndex == -1)
            {
                ParamCanalnel.AddParamCanal(ParamCanal);
                RaisePropertyChanged("Canal"); // Update the list from the data source
            }
            else
                ParamCanalnel.UpdateParamCanal(ParamCanal);

            SelectedParamCanal = null;
        }

        void key_ADD(object parameter)
        {
            SelectedParamCanal = null; // Unselects last selection. Essential, as assignment below won't clear other control's SelectedItems
            var ParamCanal = new NEWParamCanal();
            SelectedParamCanal = ParamCanal;
        }

        void DeleteUser(object parameter)
        {
            var ParamCanal = SelectedParamCanal as NEWParamCanal;
            if (SelectedIndex != -1)
            {
                ParamCanalnel.DeleteParamCanal(ParamCanal);
                RaisePropertyChanged("Canal"); // Update the list from the data source
            }
            else
                SelectedParamCanal = null; // Simply discard the new object
        }

    }
}
