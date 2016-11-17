using System.Xml.Serialization;
using System.Collections.Generic;

namespace BloggerTransformer.Models.Blogger
{
    public class Entry
    {
        [XmlElement("id")]
        public string Id;

        [XmlElement("category")]
        public List<Category> Categories;
    }
}