using System.Xml.Serialization;

namespace BloggerTransformer.Models.Blogger
{
    public class Category
    {
        [XmlAttribute("scheme")]
        public string Scheme;

        [XmlAttribute("term")]
        public string Term;
    }
}