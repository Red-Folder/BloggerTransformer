using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace BloggerTransformer.Models.Disqus
{
    [XmlRoot(ElementName = "remote", Namespace = Rss.NS_DSQ)]

    public class Remote : IXmlSerializable
    {
        public string Title { get; set; }

        // unique internal identifier; username, user id, etc. 
        [XmlElement(ElementName = "id", Namespace = Rss.NS_DSQ)]
        public string Id { get; set; }

        // Avatar
        [XmlElement(ElementName = "avatar", Namespace = Rss.NS_DSQ)]
        public string AvatarUrl { get; set; }

        public static Remote RedFolder()
        {
            return new Remote
            {
                Id = "TODO",
                AvatarUrl = "TODO"
            };
        }

        #region IXmlSerializable
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("id", Rss.NS_DSQ, Id);
            writer.WriteElementString("avatar", Rss.NS_DSQ, AvatarUrl);
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema()
        {
            return (null);
        }
        #endregion    
    }
}