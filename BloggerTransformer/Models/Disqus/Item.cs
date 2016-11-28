using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Collections.Generic;

namespace BloggerTransformer.Models.Disqus
{
    [XmlRoot(ElementName = "item")]

    public class Item : IXmlSerializable
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        // absolute URI to article
        [XmlElement(ElementName = "link")]
        public string AbsoluteUrl { get; set; }

        // body of the page or post; use cdata; html allowed (though will be formatted to DISQUS specs)
        [XmlElement(ElementName = "encoded", Namespace = Rss.NS_CONTENT)]
        public string Content { get; set; }

        // value used within disqus_identifier; usually internal identifier of article
        [XmlElement(ElementName = "thread_identifier", Namespace = Rss.NS_DSQ)]
        public string ThreadIdentified { get; set; }

        // creation date of thread (article), in GMT. Must be YYYY-MM-DD HH:MM:SS 24-hour format.
        [XmlElement(ElementName = "post_date_gmt", Namespace = Rss.NS_WP)]
        public DateTime Published { get; set; }

        // open/closed values are acceptable
        [XmlElement(ElementName = "comment_status", Namespace = Rss.NS_WP)]
        public string Status { get; set; }

        [XmlElement(ElementName = "comment", Namespace = Rss.NS_WP)]
        public List<Comment> Comments { get; set; }

        #region IXmlSerializable
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("title", Title);
            writer.WriteElementString("link", AbsoluteUrl);
            writer.WriteElementString("encoded", Rss.NS_CONTENT, Content);
            writer.WriteElementString("thread_identified", Rss.NS_DSQ, ThreadIdentified);
            writer.WriteElementString("post_date_gmt", Rss.NS_WP, Published.ToString(Rss.DISQUS_DATE_FORMAT));
            writer.WriteElementString("comment_status", Rss.NS_WP, Status);
            if (Comments != null)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Comment));
                var xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");
                foreach (var comment in Comments)
                {
                    serializer.Serialize(writer, comment, xmlnsEmpty);
                }
            }
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