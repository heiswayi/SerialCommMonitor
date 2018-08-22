using NLog;
using System.Windows;
using System.Windows.Threading;

namespace SerialCommMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ILogger _logger;

        public App()
        {
            _logger = LogManager.GetCurrentClassLogger();
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorMessage = string.Format("An exception occurred: {0}", e.Exception.Message);
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "\\log.txt", e.Exception.StackTrace);
            _logger.Fatal("[Global Exception] " + e.Exception.StackTrace);
            e.Handled = true;
        }
    }
}