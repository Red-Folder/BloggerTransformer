using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace BloggerTransformer.Models.Disqus
{
    public class Item
    {
      [XmlElement(ElementName="title")]
      public string Title { get; set; }

      // absolute URI to article
      [XmlElement(ElementName="link")]
      public string AbsoluteUrl { get; set; }
      
      // body of the page or post; use cdata; html allowed (though will be formatted to DISQUS specs)
      [XmlElement(ElementName="encoded", Namespace = "content")]
      public string Content { get; set; }

      // value used within disqus_identifier; usually internal identifier of article
      [XmlElement(ElementName="thread_identifier", Namespace = "dsq")]
      public string ThreadIdentified { get; set; }
      
      // creation date of thread (article), in GMT. Must be YYYY-MM-DD HH:MM:SS 24-hour format.
      [XmlElement(ElementName="post_date_gmt", Namespace = "wp")]
      public DateTime Published { get; set; }
      
      // open/closed values are acceptable
      [XmlElement(ElementName="comment_status", Namespace = "wp")]
      public string Status { get; set; }

      [XmlElement(ElementName = "comment", Namespace = "wp")]
      public List<Comment> Comments { get; set; }     
    }
}