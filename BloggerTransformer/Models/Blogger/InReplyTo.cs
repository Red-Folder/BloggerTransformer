using System.Xml.Serialization;

namespace BloggerTransformer.Models.Blogger
{
    public class InReplyTo
    {
        [XmlAttribute("ref")]
        public string Ref;
    }
}