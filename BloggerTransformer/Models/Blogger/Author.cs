using System.Xml.Serialization;

namespace BloggerTransformer.Models.Blogger
{
    public class Author
    {
        [XmlElement("name")]
        public string Name;
    }
}