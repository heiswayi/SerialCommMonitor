using HeiswayiNrird.MVVM.Common;
using SerialCommMonitor.Model;
using SerialCommMonitor.Service;
using System.Collections.Generic;
using System.IO.Ports;

namespace SerialCommMonitor.ViewModel
{
    public class SerialPortConfigViewModel : ViewModelBase
    {
        public SerialPortConfigViewModel()
        {
            CurrentSettings = AppManager.Instance.CurrentPortSettings;
        }

        public PortSettings CurrentSettings { get; private set; }

        //public string TerminationChar
        //{
        //    get { return AppManager.Instance.TerminationChar; }
        //    set
        //    {
        //        AppManager.Instance.TerminationChar = value;
        //        RaisePropertyChanged(() => TerminationChar);
        //    }
        //}

        public bool IsSerialPortEnabled
        {
            get
            {
                return CurrentSettings.IsSerialPortEnabled;
            }
            set
            {
                CurrentSettings.IsSerialPortEnabled = value;
                RaisePropertyChanged(() => IsSerialPortEnabled);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }

        public string ReceiverName
        {
            get
            {
                return CurrentSettings.ReceiverName;
            }
            set
            {
                CurrentSettings.ReceiverName = value;
                RaisePropertyChanged(() => ReceiverName);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }

        public string Description
        {
            get
            {
                return CurrentSettings.Description;
            }
            set
            {
                CurrentSettings.Description = value;
                RaisePropertyChanged(() => Description);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }

        public List<string> PortNameList
        {
            get { return CurrentSettings.PortNameList; }
        }
        public List<int> BaudRateList
        {
            get { return CurrentSettings.BaudRateList; }
        }
        public List<StopBits> StopBitsList
        {
            get { return CurrentSettings.StopBitsList; }
        }
        public List<Parity> ParityList
        {
            get { return CurrentSettings.ParityList; }
        }
        public List<int> DataBitsList
        {
            get { return CurrentSettings.DataBitsList; }
        }
        public List<Handshake> HandshakeList
        {
            get { return CurrentSettings.HandshakeList; }
        }

        public string SelectedPortName
        {
            get
            {
                return CurrentSettings.PortName;
            }
            set
            {
                CurrentSettings.PortName = value;
                RaisePropertyChanged(() => SelectedPortName);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }

        public int SelectedBaudRate
        {
            get
            {
                return CurrentSettings.BaudRate;
            }
            set
            {
                CurrentSettings.BaudRate = value;
                RaisePropertyChanged(() => SelectedBaudRate);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }

        public StopBits SelectedStopBits
        {
            get
            {
                return CurrentSettings.StopBits;
            }
            set
            {
                CurrentSettings.StopBits = value;
                RaisePropertyChanged(() => SelectedStopBits);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }

        public Parity SelectedParity
        {
            get
            {
                return CurrentSettings.Parity;
            }
            set
            {
                CurrentSettings.Parity = value;
                RaisePropertyChanged(() => SelectedParity);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }

        public int SelectedDataBits
        {
            get
            {
                return CurrentSettings.DataBits;
            }
            set
            {
                CurrentSettings.DataBits = value;
                RaisePropertyChanged(() => SelectedDataBits);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }

        public Handshake SelectedHandshake
        {
            get
            {
                return CurrentSettings.Handshake;
            }
            set
            {
                CurrentSettings.Handshake = value;
                RaisePropertyChanged(() => SelectedHandshake);
                AppManager.Instance.CurrentPortSettings = CurrentSettings;
            }
        }
    }
}