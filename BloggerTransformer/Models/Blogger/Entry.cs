using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BloggerTransformer.Models.Blogger
{
    public class Entry
    {
        [XmlElement("id")]
        public string Id;

        [XmlElement("category")]
        public List<Category> Categories;

        [XmlElement("published")]
        public DateTime Published;

        [XmlElement("updated")]
        public DateTime Updated;

        [XmlElement("title")]
        public string Title;

        [XmlElement("content")]
        public string Content;

        [XmlElement("author")]
        public Author Author;

        [XmlElement(Namespace = "http://purl.org/syndication/thread/1.0", ElementName = "in-reply-to")]
        public InReplyTo InReplyTo;

        [XmlElement("link")]
        public List<Link> Links;

        [XmlElement(ElementName = "control", Namespace = "http://purl.org/atom/app#")]
        public Control Control;

        public bool IsDraft
        {
            get
            {
                return (Control != null && Control.Draft.ToLower().Equals("yes"));
            }
        }
        
        public string RelatedLink
        {
            get
            {
                return GetLink("related");
            }
        }

        public string SelfLink
        {
            get
            {
                return GetLink("self");
            }
        }

        public string Url
        {
            get
            {
                //Console.WriteLine(Title);
                //Console.WriteLine(IsDraft);
                var fullUrl = BloggerUrl;
                //Console.WriteLine(fullUrl);
                return fullUrl.Substring(fullUrl.LastIndexOf('/')).Replace("/", "").Replace(".html", ""); 
            }
        }

        public string BloggerUrl
        {
            get
            {
                return GetLink("alternate");
            }
        }

        private string GetLink(string rel)
        {
            var link = Links.Where(x => x.Rel == rel);
            if (link.Count() == 1)
            {
                return link.First().HRef;
            }
            else
            {
                return "";
            }
        }

        public bool IsRootComment
        {
            get
            {
                return (Kind == KindType.Comment && RelatedLink.Length == 0);
            }
        }

        public KindType Kind
        {
            get
            {
                var kindList = Categories.Where(x => x.Scheme == "http://schemas.google.com/g/2005#kind");
                if (kindList.Count() > 1)
                {
                    Console.WriteLine("[ERROR] Found multiple kind records - unexpected");
                    Console.WriteLine("Found");
                    foreach (var kindItem in kindList)
                    {
                        Console.WriteLine("\t" + kindItem.Term);
                    }
                    throw new Exception("Multiple kind records");
                }

                switch (kindList.First().Term)
                {
                    case "http://schemas.google.com/blogger/2008/kind#template":
                        return KindType.Template;
                    case "http://schemas.google.com/blogger/2008/kind#settings":
                        return KindType.Settings;
                    case "http://schemas.google.com/blogger/2008/kind#post":
                        return KindType.Post;
                    case "http://schemas.google.com/blogger/2008/kind#comment":
                        return KindType.Comment;
                    default:
                        Console.WriteLine("[ERROR] Unknown Kind");
                        Console.WriteLine(kindList.First().Term);
                        throw new Exception("Unknown kind");
                }
            }
        }

        public List<string> Tags
        {
            get
            {
                return Categories.Where(x => x.Scheme == "http://www.blogger.com/atom/ns#").Select(x => x.Term).ToList();
            }
        }
    }

    public enum KindType
    {
        Template,
        Settings,
        Post,
        Comment,
        Unknown
    }
}