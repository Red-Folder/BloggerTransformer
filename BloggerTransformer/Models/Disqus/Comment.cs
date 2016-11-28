using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;


namespace BloggerTransformer.Models.Disqus
{
    [XmlRoot(ElementName = "comment", Namespace = Rss.NS_WP)]

    public class Comment : IXmlSerializable
    {
        // internal id of comment
        [XmlElement(ElementName = "comment_id", Namespace = Rss.NS_WP)]
        public string Id { get; set; }

        // author display name 
        [XmlElement(ElementName = "comment_author", Namespace = Rss.NS_WP)]
        public string Author { get; set; }

        // author email address 
        [XmlElement(ElementName = "comment_author_email", Namespace = Rss.NS_WP)]
        public string AuthorEmail { get; set; }

        // author url, optional
        [XmlElement(ElementName = "comment_author_url", Namespace = Rss.NS_WP)]
        public string AuthorUrl { get; set; }

        // author ip address
        [XmlElement(ElementName = "comment_author_IP", Namespace = Rss.NS_WP)]
        public string AuthorIP { get; set; }

        // comment datetime, in GMT. Must be YYYY-MM-DD HH:MM:SS 24-hour format. 
        [XmlElement(ElementName = "comment_date_gmt", Namespace = Rss.NS_WP)]
        public DateTime Published { get; set; }

        // comment body; use cdata; html allowed (though will be formatted to DISQUS specs)
        [XmlElement(ElementName = "comment_content", Namespace = Rss.NS_WP)]
        public string Content { get; set; }

        // is this comment approved? 0/1
        [XmlElement(ElementName = "comment_approved", Namespace = Rss.NS_WP)]
        public bool Approved { get; set; }

        // parent id (match up with wp:comment_id)
        [XmlElement(ElementName = "comment_parent", Namespace = Rss.NS_WP)]
        public string ParentId { get; set; }

        #region IXmlSerializable
        public void WriteXml(XmlWriter writer)
        {
            if (Author.Equals("Red Folder"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Remote));
                var xmlnsEmpty = new XmlSerializerNamespaces();
                xmlnsEmpty.Add("", "");
                serializer.Serialize(writer, Remote.RedFolder(), xmlnsEmpty);
            }
            writer.WriteElementString("comment_id", Rss.NS_WP, Id);
            writer.WriteElementString("comment_author", Rss.NS_WP, Author);
            writer.WriteElementString("comment_author_email", Rss.NS_WP, AuthorEmail);
            writer.WriteElementString("comment_author_url", Rss.NS_WP, AuthorUrl);
            writer.WriteElementString("comment_author_IP", Rss.NS_WP, AuthorIP);
            writer.WriteElementString("comment_date_gmt", Rss.NS_WP, Published.ToString(Rss.DISQUS_DATE_FORMAT));
            writer.WriteStartElement("comment_content", Rss.NS_WP);
            writer.WriteCData(Content);
            writer.WriteEndElement();
            writer.WriteElementString("comment_approved", Rss.NS_WP, Approved.ToString());
            writer.WriteElementString("comment_parent", Rss.NS_WP, ParentId);
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
