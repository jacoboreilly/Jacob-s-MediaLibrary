using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MediaLib.Model
{
    [XmlRoot(Namespace="http://xspf.org/ns/0/")]
    public class playlist
    {
        public playlist()
        {
            version = 1;
        }

        [XmlAttribute]
        public int version { get; set; }

        [XmlArray(ElementName="trackList")]
        [XmlArrayItem(ElementName = "track")]
        public List<track> trackList
        {
            get;
            set; 
        }
    }
}
