namespace SerialCommMonitor.Model
{
    public class OutputData
    {
        public string PortNumber { get; set; }
        public string Timestamp { get; set; }
        public string ReceiverName { get; set; }
        public string RawData { get; set; }
        public string Account { get; set; }
        public string EventCode { get; set; }
        public string EventDescription { get; set; }
        public string EventQualifier { get; set; }
        public string Partition { get; set; }
        public PortSettings PortSettings { get; set; }
    }
}