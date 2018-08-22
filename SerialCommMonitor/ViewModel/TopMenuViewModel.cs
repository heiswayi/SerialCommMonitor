using HeiswayiNrird.MVVM.Common;
using NLog;
using SerialCommMonitor.Service;
using System.Windows;
using System.Windows.Input;

namespace SerialCommMonitor.ViewModel
{
    public class TopMenuViewModel : ViewModelBase
    {
        private ILogger _logger;

        public TopMenuViewModel()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        private ICommand _ExitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_ExitCommand == null)
                    _ExitCommand = new RelayCommand(
                        param => Exit());
                return _ExitCommand;
            }
        }

        private ICommand _StartCommand;
        public ICommand StartCommand
        {
            get
            {
                if (_StartCommand == null)
                    _StartCommand = new RelayCommand(
                        param => Start());
                return _StartCommand;
            }
        }

        private ICommand _StopCommand;
        public ICommand StopCommand
        {
            get
            {
                if (_StopCommand == null)
                    _StopCommand = new RelayCommand(
                        param => Stop());
                return _StopCommand;
            }
        }

        private ICommand _AboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (_AboutCommand == null)
                    _AboutCommand = new RelayCommand(
                        param => About());
                return _AboutCommand;
            }
        }

        private void Exit()
        {
            if (SerialPortManager.Instance.IsOpen)
                SerialPortManager.Instance.Close();

            AppManager.Instance.SaveAppSettings();

            _logger.Trace("Application exiting...");

            Application.Current.Shutdown();
        }

        private void About()
        {
            AppManager.Instance.ShowAboutDialog();
        }

        private void Stop()
        {
            SerialPortManager.Instance.Close();
        }

        private void Start()
        {
            SerialPortManager.Instance.Open(AppManager.Instance.CurrentPortSettings);
        }
    }
}