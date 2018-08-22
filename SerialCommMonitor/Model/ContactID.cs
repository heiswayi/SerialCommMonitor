using System.Collections.Generic;
using System.Xml.Serialization;

namespace SerialCommMonitor.Model
{
    [XmlRoot("ContactIdDataMap")]
    public class ContactIdDataMap
    {
        public List<ContactId> ContactIdList { get; set; }
    }

    public class ContactId
    {
        [XmlAttribute("Category")]
        public string CategoryName { get; set; }

        [XmlAttribute("Code")]
        public int EventCode { get; set; }

        [XmlAttribute("Description")]
        public string EventDescription { get; set; }

        [XmlAttribute("Zone")]
        public string Zone { get; set; }
    }
}