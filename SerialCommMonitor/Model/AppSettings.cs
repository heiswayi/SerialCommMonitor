using System.IO.Ports;
using System.Xml.Serialization;

namespace SerialCommMonitor.Model
{
    [XmlRoot("AppSaveSettings")]
    public class AppSettings
    {
        [XmlElement]
        public bool IsSerialPortEnabled { get; set; }

        [XmlElement]
        public string ReceiverName { get; set; }

        [XmlElement]
        public string Description { get; set; }

        [XmlElement]
        public string PortName { get; set; }

        [XmlElement]
        public int BaudRate { get; set; }

        [XmlElement]
        public int DataBits { get; set; }

        [XmlElement]
        public StopBits StopBits { get; set; }

        [XmlElement]
        public Parity Parity { get; set; }

        [XmlElement]
        public Handshake Handshake { get; set; }

        [XmlElement]
        public string OutputDataDirectory { get; set; }
    }
}