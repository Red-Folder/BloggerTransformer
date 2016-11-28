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
        [XmlElement(ElementName="channel")]
        public Channel Channel { get; set; }

        #region IXmlSerializable
        public void WriteXml (XmlWriter writer)
        {
            //writer.WriteStartElement("rss");
            writer.WriteAttributeString("version", null, "2.0");
            writer.WriteAttributeString("xmlns", "content", null, "http://purl.org/rss/1.0/modules/content/");
            writer.WriteAttributeString("xmlns", "dsq", null, "http://www.disqus.com/");
            writer.WriteAttributeString("xmlns", "dc", null, "http://purl.org/dc/elements/1.1/");
            writer.WriteAttributeString("xmlns", "wp", null, "http://wordpress.org/export/1.0/");

            XmlSerializer serializer = new XmlSerializer(typeof(Channel));
            //var xmlnsEmpty = new XmlSerializerNamespaces();
            //xmlnsEmpty.Add("", "");
            //serializer.Serialize(writer, Channel, xmlnsEmpty);
            serializer.Serialize(writer, Channel);

            //writer.WriteElementString("dm:form_namespace", null, FormNamespace);
            //writer.WriteElementString("Days", Days.ToString());
            //writer.WriteElementString("Leave_Type", LeaveType);
            //writer.WriteEndElement();
        }

        public void ReadXml (XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema()
        {
            return(null);
        }
        #endregion
    }
}