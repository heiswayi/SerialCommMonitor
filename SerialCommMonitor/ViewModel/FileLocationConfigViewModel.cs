using HeiswayiNrird.MVVM.Common;
using NLog;
using Ookii.Dialogs.Wpf;
using SerialCommMonitor.Service;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Media;

namespace SerialCommMonitor.ViewModel
{
    public class FileLocationConfigViewModel : ViewModelBase
    {
        private ILogger _logger;
        private static Brush OK = Brushes.Green;
        private static Brush ERROR = Brushes.Red;
        private static Brush LOADING = Brushes.DarkGray;

        public FileLocationConfigViewModel()
        {
            _logger = LogManager.GetCurrentClassLogger();
            ValidateFile(ContactIdXmlFilename);
        }

        public string OutputDataDirectory
        {
            get
            {
                return AppManager.Instance.OutputDataDirectory;
            }
            set
            {
                AppManager.Instance.OutputDataDirectory = value;
                RaisePropertyChanged(() => OutputDataDirectory);
            }
        }

        public string ContactIdXmlFilename { get { return AppManager.Instance.GetContactIdXmlFilename; } }
        public Brush FileStatusForeground { get; set; }
        public string FileStatus { get; set; }

        private ICommand _OpenContactIdXmlCommand;
        public ICommand OpenContactIdXmlCommand
        {
            get
            {
                if (_OpenContactIdXmlCommand == null)
                    _OpenContactIdXmlCommand = new RelayCommand(
                        param => OpenContactIdXml(param));
                return _OpenContactIdXmlCommand;
            }
        }

        private ICommand _ValidateFileCommand;
        public ICommand ValidateFileCommand
        {
            get
            {
                if (_ValidateFileCommand == null)
                    _ValidateFileCommand = new RelayCommand(
                        param => ValidateFile(param));
                return _ValidateFileCommand;
            }
        }

        private ICommand _ReloadFileCommand;
        public ICommand ReloadFileCommand
        {
            get
            {
                if (_ReloadFileCommand == null)
                    _ReloadFileCommand = new RelayCommand(
                        param => ReloadFile(param));
                return _ReloadFileCommand;
            }
        }

        private ICommand _SelectOutputDataDirectoryCommand;
        public ICommand SelectOutputDataDirectoryCommand
        {
            get
            {
                if (_SelectOutputDataDirectoryCommand == null)
                    _SelectOutputDataDirectoryCommand = new RelayCommand(
                        param => SelectOutputDataDirectory());
                return _SelectOutputDataDirectoryCommand;
            }
        }

        private void OpenContactIdXml(object param)
        {
            string _filePath = AppManager.Instance.GetAppDirectory + (string)param;
            try
            {
                System.Diagnostics.Process.Start("notepad.exe", _filePath);
            }
            catch (Exception ex)
            {
                _logger.Error("[Generic Exception] Opening ContactId.xml: " + ex.ToString());
                return;
            }
        }

        private void ValidateFile(object param)
        {
            bool _fileExist = false;
            bool _fileExistAndLoaded = false;

            BackgroundWorker _worker = new BackgroundWorker();
            _worker.DoWork += (obj, e) =>
            {
                Thread.Sleep(1000);

                if (!File.Exists(AppManager.Instance.GetAppDirectory + AppManager.Instance.GetContactIdXmlFilename))
                    _fileExist = false;
                else
                    _fileExist = true;

                if (AppManager.Instance.CurrentContactIdDataObject == null)
                    _fileExistAndLoaded = false;
                else
                    _fileExistAndLoaded = true;
            };
            _worker.RunWorkerCompleted += (obj, e) =>
            {
                if (_fileExistAndLoaded)
                    SetFileStatus(OK, "File is ready.");
                else if (_fileExist && !_fileExistAndLoaded)
                    SetFileStatus(ERROR, "File exists, but not loaded.");
                else
                    SetFileStatus(ERROR, "File not found!");
            };
            _worker.RunWorkerAsync();
            SetFileStatus(LOADING, "Validating...");
        }

        private void ReloadFile(object param)
        {
            BackgroundWorker _worker = new BackgroundWorker();
            _worker.DoWork += (obj, e) =>
            {
                Thread.Sleep(1000);
                AppManager.Instance.GetContactIdDataObjectFromXmlFile();
            };
            _worker.RunWorkerCompleted += (obj, e) =>
            {
                if (AppManager.Instance.CurrentContactIdDataObject == null)
                    SetFileStatus(ERROR, "File failed to load!");
                else
                    SetFileStatus(OK, "File is loaded.");
            };
            _worker.RunWorkerAsync();
            SetFileStatus(LOADING, "Reloading...");
        }

        private void SelectOutputDataDirectory()
        {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();

            if (!string.IsNullOrEmpty(OutputDataDirectory))
                fbd.SelectedPath = OutputDataDirectory;

            var result = fbd.ShowDialog();

            if (result == true)
                OutputDataDirectory = fbd.SelectedPath + @"\";
        }

        private void SetFileStatus(Brush textColor, string text)
        {
            FileStatusForeground = textColor;
            FileStatus = text;
            RaisePropertyChanged(() => FileStatusForeground);
            RaisePropertyChanged(() => FileStatus);
        }
    }
}