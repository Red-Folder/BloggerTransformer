using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Collections.Generic;
using System.Linq;

namespace BloggerTransformer.Models.Disqus
{
    // Ref: https://help.disqus.com/customer/portal/articles/472150-custom-xml-import-format

    [XmlRoot(ElementName = "rss")]
    public class Rss : IXmlSerializable
    {
        public const string NS_CONTENT = "http://purl.org/rss/1.0/modules/content/";
        public const string NS_DSQ = "http://www.disqus.com/";
        public const string NS_DC = "http://purl.org/dc/elements/1.1/";
        public const string NS_WP = "http://wordpress.org/export/1.0/";

        public const string DISQUS_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";

        [XmlElement(ElementName = "channel")]
        public Channel Channel { get; set; }

        #region IXmlSerializable
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("version", null, "2.0");
            writer.WriteAttributeString("xmlns", "content", null, NS_CONTENT);
            writer.WriteAttributeString("xmlns", "dsq", null, NS_DSQ);
            writer.WriteAttributeString("xmlns", "dc", null, NS_DC);
            writer.WriteAttributeString("xmlns", "wp", null, NS_WP);

            XmlSerializer serializer = new XmlSerializer(typeof(Channel));
            serializer.Serialize(writer, Channel);
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