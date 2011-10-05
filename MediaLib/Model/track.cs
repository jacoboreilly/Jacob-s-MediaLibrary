using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MediaLib.Model
{
    //[XmlElement(Namespace="http://xspf.org/ns/0/")]
    public class track
    {
        public string location { get; set; }
        public string creator { get; set; }
        public string title { get; set; }
        public string annotation { get; set; }
        public string image { get; set; }
        public string info { get; set; }
    }
}
