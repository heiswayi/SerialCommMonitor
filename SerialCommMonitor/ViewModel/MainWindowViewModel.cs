using HeiswayiNrird.MVVM.Common;
using NLog;
using SerialCommMonitor.Service;
using System.Windows;

namespace SerialCommMonitor.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly string _appTitle;
        private readonly TopMenuViewModel _topMenuVM;
        private readonly FileLocationConfigViewModel _fileLocationConfigVM;
        private readonly SerialPortConfigViewModel _serialPortConfigVM;
        private readonly OutputWindowViewModel _outputWindowVM;

        private ILogger _logger;

        public MainWindowViewModel()
        {
            _logger = LogManager.GetCurrentClassLogger();

            // App name and version
            _appTitle = AppManager.Instance.AppTitle;

            // Initialize all viewmodels
            _topMenuVM = new TopMenuViewModel();
            _fileLocationConfigVM = new FileLocationConfigViewModel();
            _serialPortConfigVM = new SerialPortConfigViewModel();
            _outputWindowVM = new OutputWindowViewModel();

            IsEnabledFileConfVM = true;
            IsEnabledSerialPortVM = true;

            Application.Current.MainWindow.Closing += MainWindow_Closing;
            SerialPortManager.Instance.OnSerialPortOpened += (obj, e) =>
            {
                if (e == true)
                {
                    IsEnabledFileConfVM = false;
                    IsEnabledSerialPortVM = false;
                }
                else
                {
                    IsEnabledFileConfVM = true;
                    IsEnabledSerialPortVM = true;
                }
            };
            SerialPortManager.Instance.OnStatusChanged += (obj, e) =>
            {
                BottomStatusText = e;
            };

            BottomStatusText = "Application started.";

            _logger.Trace("Application started.");
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (SerialPortManager.Instance.IsOpen)
                SerialPortManager.Instance.Close();

            AppManager.Instance.SaveAppSettings();

            BottomStatusText = "Application closing...";

            _logger.Trace("Application closing...");
        }

        public string AppTitle { get { return _appTitle; } }
        public TopMenuViewModel TopMenuVM { get { return _topMenuVM; } }
        public FileLocationConfigViewModel FileLocationConfigVM { get { return _fileLocationConfigVM; } }
        public SerialPortConfigViewModel SerialPortConfigVM { get { return _serialPortConfigVM; } }
        public OutputWindowViewModel OutputWindowVM { get { return _outputWindowVM; } }

        private string _BottomStatusText;
        public string BottomStatusText
        {
            get { return _BottomStatusText; }
            set
            {
                _BottomStatusText = value;
                RaisePropertyChanged(() => BottomStatusText);
            }
        }

        private bool _IsEnabledFileConfVM;
        public bool IsEnabledFileConfVM
        {
            get { return _IsEnabledFileConfVM; }
            set
            {
                _IsEnabledFileConfVM = value;
                RaisePropertyChanged(() => IsEnabledFileConfVM);
            }
        }

        private bool _IsEnabledSerialPortVM;
        public bool IsEnabledSerialPortVM
        {
            get { return _IsEnabledSerialPortVM; }
            set
            {
                _IsEnabledSerialPortVM = value;
                RaisePropertyChanged(() => IsEnabledSerialPortVM);
            }
        }
    }
}