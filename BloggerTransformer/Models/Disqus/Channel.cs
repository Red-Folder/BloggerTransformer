using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml;

namespace BloggerTransformer.Models.Disqus
{
    [XmlRoot(ElementName="channel")]
    public class Channel: IXmlSerializable
    {
        [XmlElement(ElementName = "item")]
        public List<Item> Items { get; set; }

        #region IXmlSerializable
        public void WriteXml (XmlWriter writer)
        {
            //XmlSerializer serializer = new XmlSerializer(typeof(Channel));
            //var xmlnsEmpty = new XmlSerializerNamespaces();
            //xmlnsEmpty.Add("", "");
            //serializer.Serialize(writer, Channel, xmlnsEmpty);

            writer.WriteString("Hello World");
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