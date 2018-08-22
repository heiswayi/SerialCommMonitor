using NLog;
using SerialCommMonitor.Model;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace SerialCommMonitor.Service
{
    public sealed class SerialPortManager
    {
        private static readonly Lazy<SerialPortManager> lazy = new Lazy<SerialPortManager>(() => new SerialPortManager());
        public static SerialPortManager Instance { get { return lazy.Value; } }

        private ILogger _logger;
        private SerialPort _serialPort;
        private Thread _readThread;
        private volatile bool _keepReading;

        private SerialPortManager()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _serialPort = new SerialPort();
            _readThread = null;
            _keepReading = false;
        }

        public event EventHandler<string> OnStatusChanged;
        public event EventHandler<string> OnDataReceived;
        public event EventHandler<bool> OnSerialPortOpened;
        public PortSettings CurrentPortSettings { get; private set; }

        public bool IsOpen { get { return _serialPort.IsOpen; } }

        public void Open(PortSettings portSettings)
        {
            if (portSettings == null)
                return;

            if (!portSettings.IsSerialPortEnabled)
            {
                if (OnStatusChanged != null)
                    OnStatusChanged(this, "Unable to start, port is not enabled.");
                return;
            }

            Close();

            try
            {
                CurrentPortSettings = portSettings;
                _serialPort.PortName = CurrentPortSettings.PortName;
                _serialPort.BaudRate = CurrentPortSettings.BaudRate;
                _serialPort.Parity = CurrentPortSettings.Parity;
                _serialPort.DataBits = CurrentPortSettings.DataBits;
                _serialPort.StopBits = CurrentPortSettings.StopBits;
                _serialPort.Handshake = CurrentPortSettings.Handshake;

                _serialPort.ReadTimeout = 500;
                _serialPort.WriteTimeout = 500;

                _logger.Info(string.Format(
                    "SerialPort settings: PortName={0}, BaudRate={1}, DataBits={2}, Parity={3}, StopBits={4}, Handshake={5}, ReadTimeout={6}, WriteTimeout={7}",
                    _serialPort.PortName,
                    _serialPort.BaudRate,
                    _serialPort.DataBits,
                    _serialPort.Parity,
                    _serialPort.StopBits,
                    _serialPort.Handshake,
                    _serialPort.ReadTimeout,
                    _serialPort.WriteTimeout));

                _serialPort.Open();
                StartReading();
            }
            catch (IOException)
            {
                if (OnStatusChanged != null)
                    OnStatusChanged(this, string.Format("{0} does not exist.", CurrentPortSettings.PortName));
                _logger.Error(string.Format("SerialPort: {0} does not exist.", CurrentPortSettings.PortName));
            }
            catch (UnauthorizedAccessException)
            {
                if (OnStatusChanged != null)
                    OnStatusChanged(this, string.Format("{0} already in use.", CurrentPortSettings.PortName));
                _logger.Error(string.Format("SerialPort: {0} already in use.", CurrentPortSettings.PortName));
            }
            catch (Exception ex)
            {
                if (OnStatusChanged != null)
                    OnStatusChanged(this, "Exception: " + ex.Message);
                _logger.Error("[Generic Exception] SerialPort: " + ex.ToString());
            }

            if (_serialPort.IsOpen)
            {
                string sb = StopBits.None.ToString().Substring(0, 1);
                switch (_serialPort.StopBits)
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

                string p = _serialPort.Parity.ToString().Substring(0, 1);
                string hs = _serialPort.Handshake == Handshake.None ? "No Handshake" : _serialPort.Handshake.ToString();

                if (OnStatusChanged != null)
                    OnStatusChanged(this, string.Format(
                    "Connected to {0}: {1} bps, {2}{3}{4}, {5}.",
                    _serialPort.PortName,
                    _serialPort.BaudRate,
                    _serialPort.DataBits,
                    p,
                    sb,
                    hs));

                if (OnSerialPortOpened != null)
                    OnSerialPortOpened(this, true);

                _logger.Info(string.Format(
                    "SerialPort: Connected to {0}: {1} bps, {2}{3}{4}, {5}.",
                    _serialPort.PortName,
                    _serialPort.BaudRate,
                    _serialPort.DataBits,
                    p,
                    sb,
                    hs));
            }
            else
            {
                if (OnStatusChanged != null)
                    OnStatusChanged(this, string.Format(
                    "{0} already in use.",
                    CurrentPortSettings.PortName));

                if (OnSerialPortOpened != null)
                    OnSerialPortOpened(this, false);

                _logger.Error(string.Format(
                    "SerialPort: {0} already in use.",
                    CurrentPortSettings.PortName));
            }
        }

        public void Close()
        {
            StopReading();
            _serialPort.Close();

            if (OnStatusChanged != null)
                OnStatusChanged(this, "Connection closed.");

            if (OnSerialPortOpened != null)
                OnSerialPortOpened(this, false);

            _logger.Info("SerialPort: Connection closed.");
        }

        public void SendString(string message)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Write(message);

                if (OnStatusChanged != null)
                    OnStatusChanged(this, string.Format(
                    "Message sent: {0}",
                    message));

                _logger.Info(string.Format(
                    "SerialPort: Message sent: {0}",
                    message));
            }
        }

        private void StartReading()
        {
            if (!_keepReading)
            {
                _keepReading = true;
                _readThread = new Thread(ReadPort);
                _readThread.Start();
            }
        }

        private void StopReading()
        {
            if (_keepReading)
            {
                _keepReading = false;
                _readThread.Join();
                _readThread = null;
            }
        }

        private void ReadPort()
        {
            while (_keepReading)
            {
                if (_serialPort.IsOpen)
                {
                    //byte[] readBuffer = new byte[_serialPort.ReadBufferSize + 1];
                    try
                    {
                        //int count = _serialPort.Read(readBuffer, 0, _serialPort.ReadBufferSize);
                        //string data = Encoding.ASCII.GetString(readBuffer, 0, count);
                        string data = _serialPort.ReadLine();

                        if (OnDataReceived != null)
                            OnDataReceived(this, data);

                        //_logger.Debug("SerialPort: Receiving string data: " + data);
                        //Debug.WriteLine("[DEBUG] string data: " + data);
                    }
                    catch (TimeoutException) { }
                }
                else
                {
                    TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 500);
                    Thread.Sleep(waitTime);
                }
            }
        }
    }
}