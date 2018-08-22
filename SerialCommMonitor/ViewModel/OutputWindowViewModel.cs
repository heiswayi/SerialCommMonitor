using HeiswayiNrird.MVVM.Common;
using NLog;
using SerialCommMonitor.Model;
using SerialCommMonitor.Service;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SerialCommMonitor.ViewModel
{
    public class OutputWindowViewModel : ViewModelBase
    {
        private ILogger _logger;
        private OutputData _outputData;
        private ContactIdDataMap _contactIdDataMap;
        private PortSettings _currentPortSettings;
        private StringBuilder _serialDataBuffer;

        private bool _isPaused;

        /// <summary>
        /// Constructor
        /// </summary>
        public OutputWindowViewModel()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _contactIdDataMap = AppManager.Instance.CurrentContactIdDataObject;
            _serialDataBuffer = new StringBuilder();

            AppManager.Instance.OnContactIdXmlLoaded += (obj, e) =>
            {
                _isPaused = true;
                _contactIdDataMap = e;
                _isPaused = false;
            };

            SerialPortManager.Instance.OnSerialPortOpened += (obj, e) =>
            {
                if (e == true)
                    _currentPortSettings = AppManager.Instance.CurrentPortSettings;
            };
            SerialPortManager.Instance.OnDataReceived += Handler_OnDataReceived;
        }

        private ICommand _ClearAllCommand;
        public ICommand ClearAllCommand
        {
            get
            {
                if (_ClearAllCommand == null)
                    _ClearAllCommand = new RelayCommand((param) =>
                    {
                        ClearOutputDataList();
                    });
                return _ClearAllCommand;
            }
        }

        /// <summary>
        /// Event handler for receiving data from SerialPortManager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dataBuffer"></param>
        private void Handler_OnDataReceived(object sender, string dataBuffer)
        {
            if (_isPaused)
                return;

            //Debug.WriteLine("[DEBUG] dataBuffer: " + dataBuffer);

            // Buffer data until termination sequence is found
            //_serialDataBuffer.Append(dataBuffer);
            //string bufferString = _serialDataBuffer.ToString();
            //int index = -1;
            //do
            //{
            //    index = bufferString.IndexOf(AppManager.Instance.TerminationChar);
            //    if (index > -1)
            //    {
            //        string message = bufferString.Substring(0, index);
            //        bufferString = bufferString.Remove(0, index + AppManager.Instance.TerminationChar.Length);

            //        Debug.WriteLine("[DEBUG] message: " + message);

            //        // Interpret buffered message after termination sequence is found
            //        InterpretData(message);
            //    }
            //}
            //while (index > -1);
            //_serialDataBuffer = new StringBuilder(bufferString);

            //App.Current.Dispatcher.Invoke(delegate
            //{
            //    UpdateToConsoleView(string.Format(
            //    "[{0}] {1}",
            //    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
            //    dataBuffer));

            //    InterpretData2(dataBuffer);
            //});

            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                InterpretData(dataBuffer);
            }));
        }

        public ObservableCollection<RawDataObject> RawDataList { get; set; }
        public class RawDataObject
        {
            public string Timestamp { get; set; }
            public string RawData { get; set; }
            public RawDataObject(string _timestamp, string _rawdata)
            {
                Timestamp = _timestamp;
                RawData = _rawdata;
            }
        }

        public ObservableCollection<OutputData> OutputDataList { get; set; }
        public string OperationStatus { get; set; }

        // Clear output list
        public void ClearOutputDataList()
        {
            if (OutputDataList.Count > 0)
                OutputDataList.Clear();
        }

        /// <summary>
        /// Interpret data message
        /// </summary>
        /// <param name="data">Unprocessed string data</param>
        private void InterpretData(string data)
        {
            try
            {
                DateTime timestamp = DateTime.Now;
                //string incomingData = data.Trim().Replace(" ", string.Empty);

                ////AAAABBCCCCDEEEFFGGG = 19 chars
                //if (incomingData.Length < 19)
                //    return;

                ////Get Q
                //string d = "D";
                //if (incomingData.IndexOf("E") != -1)
                //    d = "E";
                //else if (incomingData.IndexOf("R") != -1)
                //    d = "R";

                //int qualifierIndex = incomingData.IndexOf(d);
                //string beforeQualifier = incomingData.Substring(0, qualifierIndex);

                ////Get AAAABBCCCC = 10 chars
                //string aaaabbcccc = "AAAABBCCCC";
                //if (beforeQualifier.Length >= 10)
                //    aaaabbcccc = beforeQualifier.Substring(beforeQualifier.Length - 10);

                ////Get AAAA
                //string aaaa = "AAAA";
                //if (aaaabbcccc.Length >= 4)
                //    aaaa = aaaabbcccc.Substring(0, 4);

                ////Get BB
                //string bb = "BB";
                //if (aaaabbcccc.Length == 10)
                //    bb = aaaabbcccc.Substring(aaaabbcccc.Length - 6, 2);

                ////Get CCCC
                //string cccc = "CCCC";
                //if (aaaabbcccc.Length == 10)
                //    cccc = aaaabbcccc.Substring(aaaabbcccc.Length - 4);

                ////Get EEEFFGGG = 8 chars
                //string afterQualifier = incomingData.Substring(qualifierIndex + 1);

                ////Get EEE
                //string eee = "EEE";
                //if (afterQualifier.Length >= 3)
                //    eee = afterQualifier.Substring(0, 3);

                ////Get FF
                //string ff = "FF";
                //if (afterQualifier.Length >= 5)
                //    ff = afterQualifier.Substring(3, 2);

                ////Get GGG
                //string ggg = "GGG";
                //if (afterQualifier.Length >= 8)
                //    ggg = afterQualifier.Substring(5, 3);

                ////AAAABBCCCCDEEEFFGGG
                //string rawdata = string.Format(
                //        "{0}{1}{2}{3}{4}{5}{6}",
                //        aaaa,
                //        bb,
                //        cccc,
                //        d,
                //        eee,
                //        ff,
                //        ggg);

                //Debug.WriteLine("[DEBUG] rawdata: " + rawdata);

                //// CCCC = Account | D = Qualifier | EEE = Event Code | FF = Partition | GGG = Zone
                //string account = cccc; // CCCC

                //string qualifier = "Q";
                //if (d == "E")
                //    qualifier = "Opening";
                //else if (d == "R")
                //    qualifier = "Closing";

                //string eventCode = eee; // EEE
                //string partition = ff; // FF
                //string zone = ggg; // GGG

                //// Create output data object
                //_outputData = new OutputData();
                //_outputData.Account = account;
                //_outputData.EventCode = eventCode;
                //_outputData.EventDescription = GetEventDescriptionFromContactIdXml(eventCode);
                //_outputData.EventQualifier = qualifier;
                //_outputData.Partition = partition;
                //_outputData.PortSettings = _currentPortSettings;
                //_outputData.PortNumber = (_currentPortSettings.Id + 1).ToString();
                //_outputData.RawData = rawdata;
                //_outputData.ReceiverName = _currentPortSettings.ReceiverName;
                //_outputData.Timestamp = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

                //UpdateToTableView(_outputData);
                UpdateToConsoleView(timestamp.ToString("HH:mm:ss.fff"), data);

                // Log interpreted data to a file
                if (AppManager.Instance.ExportToFileEnabled)
                    AppManager.Instance.ExportToFile(_outputData);
            }
            catch (Exception ex)
            {
                _logger.Error("[Generic Exception] Interpret data: " + ex.ToString());
                return;
            }
        }

        /// <summary>
        /// Display interpreted data to Table View
        /// </summary>
        /// <param name="outputData">Processed data</param>
        private void UpdateToTableView(OutputData outputData)
        {
            if (outputData == null)
                return;

            if (OutputDataList == null)
                OutputDataList = new ObservableCollection<OutputData>();

            OutputDataList.Add(
                new OutputData()
                {
                    PortNumber = outputData.PortNumber,
                    Timestamp = outputData.Timestamp,
                    ReceiverName = outputData.ReceiverName,
                    RawData = outputData.RawData,
                    Account = outputData.Account,
                    EventCode = outputData.EventCode,
                    EventDescription = outputData.EventDescription,
                    EventQualifier = outputData.EventQualifier,
                    Partition = outputData.Partition
                });

            RaisePropertyChanged(() => OutputDataList);
        }

        /// <summary>
        /// Display raw data to Console View
        /// </summary>
        /// <param name="timestamp">Timestamp of data retrieved</param>
        /// <param name="data">Unprocessed string data</param>
        private void UpdateToConsoleView(string timestamp, string data)
        {
            if (RawDataList == null)
                RawDataList = new ObservableCollection<RawDataObject>();

            RawDataList.Add(new RawDataObject(timestamp, data));
            RaisePropertyChanged(() => RawDataList);
        }

        private string GetEventDescriptionFromContactIdXml(string eventCode)
        {
            if (_contactIdDataMap != null)
            {
                int _eventCode = Convert.ToInt16(eventCode);
                bool _exist = _contactIdDataMap.ContactIdList.Any(x => x.EventCode == _eventCode);
                if (_exist)
                    return _contactIdDataMap.ContactIdList.First(x => x.EventCode == _eventCode).EventDescription;
                else
                    return "Undefined";
            }
            else
            {
                return "N/A";
            }
        }
    }
}