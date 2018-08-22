using System.Collections.Generic;
using System.IO.Ports;

namespace SerialCommMonitor.Model
{
    public class PortSettings
    {
        public PortSettings()
        {
        }

        public int Id { get; set; }
        public bool IsSerialPortEnabled { get; set; }
        public string ReceiverName { get; set; }
        public string Description { get; set; }
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public StopBits StopBits { get; set; }
        public Parity Parity { get; set; }
        public Handshake Handshake { get; set; }

        public List<string> PortNameList { get; set; }
        public List<int> BaudRateList { get; set; }
        public List<int> DataBitsList { get; set; }
        public List<StopBits> StopBitsList { get; set; }
        public List<Parity> ParityList { get; set; }
        public List<Handshake> HandshakeList { get; set; }

        public string[] GetAvailablePorts()
        {
            return SerialPort.GetPortNames();
        }

        public int[] GetPredefinedBaudrates()
        {
            return new int[]
            {
                100,300,600,1200,2400,4800,9600,14400,19200,
                38400,56000,57600,115200,128000,256000,0
            };
        }

        public PortSettings GetDefaultValues()
        {
            var obj = new PortSettings();
            obj.Id = 0;
            obj.IsSerialPortEnabled = false;
            obj.ReceiverName = "Receiver1";
            obj.Description = "";

            obj.PortNameList = new List<string>();
            foreach (var port in GetAvailablePorts())
            {
                obj.PortNameList.Add(port);
            }
            if (obj.PortNameList.Count > 0 && obj.PortNameList.Exists(x => x == "COM1"))
                obj.PortName = "COM1";
            else
                obj.PortName = "";

            obj.BaudRateList = new List<int>();
            foreach (var baudrate in GetPredefinedBaudrates())
            {
                obj.BaudRateList.Add(baudrate);
            }
            obj.BaudRate = 1200;

            obj.StopBitsList = new List<StopBits>();
            obj.StopBitsList.Add(StopBits.None);
            obj.StopBitsList.Add(StopBits.One);
            obj.StopBitsList.Add(StopBits.OnePointFive);
            obj.StopBitsList.Add(StopBits.Two);
            obj.StopBits = StopBits.One;

            obj.ParityList = new List<Parity>();
            obj.ParityList.Add(Parity.Even);
            obj.ParityList.Add(Parity.Mark);
            obj.ParityList.Add(Parity.None);
            obj.ParityList.Add(Parity.Odd);
            obj.ParityList.Add(Parity.Space);
            obj.Parity = Parity.None;

            obj.DataBitsList = new List<int>();
            obj.DataBitsList.Add(5);
            obj.DataBitsList.Add(6);
            obj.DataBitsList.Add(7);
            obj.DataBitsList.Add(8);
            obj.DataBits = 8;

            obj.HandshakeList = new List<Handshake>();
            obj.HandshakeList.Add(Handshake.None);
            obj.HandshakeList.Add(Handshake.RequestToSend);
            obj.HandshakeList.Add(Handshake.RequestToSendXOnXOff);
            obj.HandshakeList.Add(Handshake.XOnXOff);
            obj.Handshake = Handshake.None;

            return obj;
        }
    }
}