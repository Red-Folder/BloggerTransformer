using System;
using System.Xml.Serialization;

namespace BloggerTransformer.Models.Disqus
{
    public class Comment
    {
        // internal id of comment
        [XmlElement(ElementName = "comment_id", Namespace = "wp")]
        public string Id { get; set; }

        // author display name 
        [XmlElement(ElementName = "comment_author", Namespace = "wp" )]
        public string Author { get; set; }

        // author email address 
        [XmlElement(ElementName = "comment_author_email", Namespace = "wp")]
        public string AuthorEmail { get; set; }

        // author url, optional
        [XmlElement(ElementName = "comment_author_url", Namespace = "wp")]
        public string AuthorUrl { get; set; }

        // author ip address
        [XmlElement(ElementName = "comment_author_IP", Namespace = "wp")]
        public string AuthorIP { get; set; }

        // comment datetime, in GMT. Must be YYYY-MM-DD HH:MM:SS 24-hour format. 
        [XmlElement(ElementName = "comment_date_gmt", Namespace = "wp")]
        public DateTime Published { get; set; }

        // comment body; use cdata; html allowed (though will be formatted to DISQUS specs)
        [XmlElement(ElementName = "comment_content", Namespace = "wp")]
        public string Content { get; set; }

        // is this comment approved? 0/1
        [XmlElement(ElementName = "comment_approved", Namespace = "wp")]
        public bool Approved { get; set; }

        // parent id (match up with wp:comment_id)
        [XmlElement(ElementName = "comment_parent", Namespace = "wp")]
        public string ParentId { get; set; }
    }
}
