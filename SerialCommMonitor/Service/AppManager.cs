using HeiswayiNrird.Utility.Common;
using NLog;
using Ookii.Dialogs.Wpf;
using SerialCommMonitor.Model;
using System;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Windows;

namespace SerialCommMonitor.Service
{
    public class AppManager
    {
        private static readonly Lazy<AppManager> lazy = new Lazy<AppManager>(() => new AppManager());
        public static AppManager Instance { get { return lazy.Value; } }

        private ILogger _logger;

        private AppManager()
        {
            Init();
        }

        public void Init()
        {
            _logger = LogManager.GetCurrentClassLogger();
            ExportToFileEnabled = true;
            //TerminationChar = "¶";

            CurrentAppSettings = LoadAppSettings();
            if (CurrentAppSettings != null)
            {
                OutputDataDirectory = CurrentAppSettings.OutputDataDirectory;
                var portSettings = new PortSettings();
                var defaultList = portSettings.GetDefaultValues();

                portSettings.IsSerialPortEnabled = CurrentAppSettings.IsSerialPortEnabled;
                portSettings.ReceiverName = CurrentAppSettings.ReceiverName;
                portSettings.Description = CurrentAppSettings.Description;
                portSettings.PortName = CurrentAppSettings.PortName;
                portSettings.BaudRate = CurrentAppSettings.BaudRate;
                portSettings.Parity = CurrentAppSettings.Parity;
                portSettings.DataBits = CurrentAppSettings.DataBits;
                portSettings.StopBits = CurrentAppSettings.StopBits;
                portSettings.Handshake = CurrentAppSettings.Handshake;
                portSettings.PortNameList = defaultList.PortNameList;
                portSettings.BaudRateList = defaultList.BaudRateList;
                portSettings.ParityList = defaultList.ParityList;
                portSettings.DataBitsList = defaultList.DataBitsList;
                portSettings.StopBitsList = defaultList.StopBitsList;
                portSettings.HandshakeList = defaultList.HandshakeList;

                CurrentPortSettings = portSettings;
            }
            else
            {
                OutputDataDirectory = GetAppDirectory;
                var portSettings = new PortSettings();
                CurrentPortSettings = portSettings.GetDefaultValues();
            }
        }

        public string AppTitle
        {
            get
            {
                return string.Format("{0} alpha {1}",
                Assembly.GetExecutingAssembly().GetName().Name,
                Assembly.GetExecutingAssembly().GetName().Version);
            }
        }
        public string GetAppDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }
        public string GetContactIdXmlFilename = "ContactId.xml";

        public PortSettings CurrentPortSettings { get; set; }
        public ContactIdDataMap CurrentContactIdDataObject { get; private set; }
        public AppSettings CurrentAppSettings { get; set; }
        public string OutputDataDirectory { get; set; }
        //public string TerminationChar { get; set; }

        public bool ExportToFileEnabled { get; set; }
        public void ExportToFile(OutputData outputData)
        {
            if (outputData == null)
                return;

            if (string.IsNullOrEmpty(OutputDataDirectory))
                OutputDataDirectory = GetAppDirectory;

            string outputDirectory = string.Format(
                "{0}{1}",
                OutputDataDirectory,
                "output");

            string outputFilePath = string.Format(
                @"{0}\{1}.{2}",
                outputDirectory,
                DateTime.Now.ToString("yyyy-MM-dd"),
                "csv");

            string sb = StopBits.None.ToString().Substring(0, 1);
            switch (outputData.PortSettings.StopBits)
            {
                case StopBits.One:
                    sb = "1"; break;
                case StopBits.OnePointFive:
                    sb = "1.5"; break;
                case StopBits.Two:
                    sb = "2"; break;
                default:
                    break;
            }

            string p = outputData.PortSettings.Parity.ToString().Substring(0, 1);
            string hs = outputData.PortSettings.Handshake == Handshake.None ? "No Handshake" : outputData.PortSettings.Handshake.ToString();

            string portSettings = string.Format(
                "[{0}|{1}|{2}{3}{4}|{5}]",
                outputData.PortSettings.PortName,
                outputData.PortSettings.BaudRate,
                outputData.PortSettings.DataBits,
                p,
                sb,
                hs);

            string appendData = string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                outputData.PortNumber,
                outputData.Timestamp,
                outputData.ReceiverName,
                outputData.RawData,
                outputData.Account,
                outputData.EventCode,
                outputData.EventDescription,
                outputData.EventQualifier,
                outputData.Partition,
                portSettings);

            try
            {
                Directory.CreateDirectory(outputDirectory);

                if (!File.Exists(outputFilePath))
                {
                    string header = "Port Number,Timestamp,Receiver Name,Raw Data,Account,Event Code,Event Description,Event Qualifier,Partition,Port Settings";
                    File.WriteAllText(outputFilePath, header + Environment.NewLine);

                    //_logger.Trace("File.WriteAllText(outputFilePath, header + Environment.NewLine);");
                }

                File.AppendAllText(outputFilePath, appendData + Environment.NewLine);

                //_logger.Trace("File.AppendAllText(outputFilePath, appendData + Environment.NewLine);");
            }
            catch (Exception ex)
            {
                _logger.Error("[Generic Exception] Exporting to CSV file: " + ex.ToString());
                return;
            }
        }

        public event EventHandler<ContactIdDataMap> OnContactIdXmlLoaded;
        public void GetContactIdDataObjectFromXmlFile()
        {
            if (!File.Exists(GetAppDirectory + GetContactIdXmlFilename))
                CurrentContactIdDataObject = null;

            try
            {
                CurrentContactIdDataObject = XmlHelper.DeserializeFromXmlFile<ContactIdDataMap>(GetAppDirectory + GetContactIdXmlFilename);
                _logger.Trace("CurrentContactIdDataObject = XmlHelper.DeserializeFromXmlFile<ContactIdDataMap>(GetAppDirectory + GetContactIdXmlFilename);");
            }
            catch
            {
                CurrentContactIdDataObject = null;
                _logger.Error("CurrentContactIdDataObject = null;");
            }

            if (OnContactIdXmlLoaded != null)
                OnContactIdXmlLoaded(this, CurrentContactIdDataObject);
        }

        public AppSettings LoadAppSettings()
        {
            AppSettings _appConfig = null;

            if (!File.Exists(GetAppDirectory + @"\AppConfig.xml"))
            {
                if (CurrentPortSettings == null)
                    CurrentPortSettings = new PortSettings();

                CurrentPortSettings = CurrentPortSettings.GetDefaultValues();

                _appConfig = new AppSettings()
                {
                    IsSerialPortEnabled = CurrentPortSettings.IsSerialPortEnabled,
                    ReceiverName = CurrentPortSettings.ReceiverName,
                    Description = CurrentPortSettings.Description,
                    PortName = CurrentPortSettings.PortName,
                    BaudRate = CurrentPortSettings.BaudRate,
                    Parity = CurrentPortSettings.Parity,
                    DataBits = CurrentPortSettings.DataBits,
                    StopBits = CurrentPortSettings.StopBits,
                    Handshake = CurrentPortSettings.Handshake,
                    OutputDataDirectory = GetAppDirectory
                };

                try
                {
                    XmlHelper.SerializeToXmlFile<AppSettings>(_appConfig, "AppConfig.xml");
                }
                catch (Exception ex)
                {
                    _logger.Error("[Generic Exception] LoadAppSettings->SerializeToXmlFile: " + ex.ToString());
                    return null;
                }
            }

            try
            {
                var obj = XmlHelper.DeserializeFromXmlFile<AppSettings>(GetAppDirectory + @"\AppConfig.xml");
                _appConfig = new AppSettings();
                _appConfig.IsSerialPortEnabled = obj.IsSerialPortEnabled;
                _appConfig.ReceiverName = obj.ReceiverName;
                _appConfig.Description = obj.Description;
                _appConfig.PortName = obj.PortName;
                _appConfig.BaudRate = obj.BaudRate;
                _appConfig.Parity = obj.Parity;
                _appConfig.DataBits = obj.DataBits;
                _appConfig.StopBits = obj.StopBits;
                _appConfig.Handshake = obj.Handshake;
                _appConfig.OutputDataDirectory = obj.OutputDataDirectory;

                return _appConfig;
            }
            catch (Exception ex)
            {
                _logger.Error("[Generic Exception] LoadAppSettings->DeserializeFromXmlFile: " + ex.ToString());
                return null;
            }
        }

        public void SaveAppSettings()
        {
            try
            {
                var _appConfig = new AppSettings()
                {
                    IsSerialPortEnabled = CurrentPortSettings.IsSerialPortEnabled,
                    ReceiverName = CurrentPortSettings.ReceiverName,
                    Description = CurrentPortSettings.Description,
                    PortName = CurrentPortSettings.PortName,
                    BaudRate = CurrentPortSettings.BaudRate,
                    Parity = CurrentPortSettings.Parity,
                    DataBits = CurrentPortSettings.DataBits,
                    StopBits = CurrentPortSettings.StopBits,
                    Handshake = CurrentPortSettings.Handshake,
                    OutputDataDirectory = OutputDataDirectory
                };

                XmlHelper.SerializeToXmlFile<AppSettings>(_appConfig, "AppConfig.xml");
            }
            catch (Exception ex)
            {
                _logger.Error("[Generic Exception] SaveAppSettings->SerializeToXmlFile: " + ex.ToString());
            }
        }

        public void ShowAboutDialog()
        {
            TaskDialog td = new TaskDialog();
            td.WindowTitle = "About";
            td.MainIcon = TaskDialogIcon.Information;
            td.MainInstruction = AppTitle;
            td.Content = "Developed by Heiswayi Nrird, 2016.\nWebsite: <a href=\"https://heiswayi.github.io\">https://heiswayi.github.io</a>";
            td.EnableHyperlinks = true;
            td.HyperlinkClicked += (obj, e) =>
            {
                System.Diagnostics.Process.Start(e.Href);
            };
            TaskDialogButton okButton = new TaskDialogButton(ButtonType.Ok);
            td.Buttons.Add(okButton);
            TaskDialogButton button = td.ShowDialog(Application.Current.MainWindow);
        }
    }
}