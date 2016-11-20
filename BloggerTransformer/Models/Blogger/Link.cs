using System.Xml.Serialization;

namespace BloggerTransformer.Models.Blogger
{
    public class Link
    {
        [XmlAttribute("rel")]
        public string Rel;

        [XmlAttribute("href")]
        public string HRef;
    }
}