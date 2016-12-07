using System.Xml.Serialization;

namespace BloggerTransformer.Models.Blogger
{
    public class Control
    {
        [XmlElement(ElementName = "draft", Namespace = "http://purl.org/atom/app#")]
        public string Draft;
    }
}