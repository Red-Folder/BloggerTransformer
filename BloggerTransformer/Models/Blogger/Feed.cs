using System.Xml.Serialization;
using System.Collections.Generic;

namespace BloggerTransformer.Models.Blogger
{
    // Ref: https://www.nuget.org/packages/System.Xml.XmlSerializer/

    [XmlRoot(Namespace = "http://www.w3.org/2005/Atom", ElementName = "feed")]
    public class Feed
    {
        [XmlElement(ElementName = "id")]
        public string Id;

        [XmlElement(ElementName = "entry")]
        public List<Entry> Entries;
    }
}