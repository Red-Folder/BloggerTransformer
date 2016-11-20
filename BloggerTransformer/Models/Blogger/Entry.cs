using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var fullUrl = GetLink("alternate");
                return fullUrl.Replace("http://blog.red-folder.com", "");
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

        public string ContentAsMarkdown
        {
            get
            {
                var md = Content;

                // Add Carriage Returns
                md = md.Replace("<br />", Environment.NewLine);

                // Map <hx>
                md = Regex.Replace(md, "<h1>(.*)</h1>", x => {
                    return "# " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
                });
                md = Regex.Replace(md, "<h2>(.*)</h2>", x => {
                    return "## " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
                });
                md = Regex.Replace(md, "<h3>(.*)</h3>", x => {
                    return "### " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
                });

                // Replace non-breaking spaces
                md = md.Replace("&nbsp;"," ");

                // Remove summary breaks
                md = md.Replace("<a name='more'></a>" + Environment.NewLine + Environment.NewLine, "");

                // Map blogger image
                md = Regex.Replace(md, "<div class=\"separator\"(.*)<a href=\"(.*?)\"(.*)<img (.*)src=\"(.*)\"(.*)</div>", x => {
                   return "![Image](" + x.Groups[2] + ")" + Environment.NewLine; 
                });

                // Map Anchors
                md = Regex.Replace(md, "<a (.*)>(.*)</a>", x => {
                    var sb = new StringBuilder();
                    sb.Append("[");
                    sb.Append(x.Groups[2]);
                    sb.Append("]");
                    sb.Append("(");
                    sb.Append(Regex.Matches(x.Groups[1].ToString(), "href=\"(.*)\"")[0].Groups[1]);
                    sb.Append(")");
                    return sb.ToString();
                });

                // Map Lists
                md = md.Replace("<ul>", Environment.NewLine);
                md = md.Replace("<li>", "* ");
                md = md.Replace("</li>", Environment.NewLine);
                md = md.Replace("</ul>", Environment.NewLine);

                // Remove Divs
                md = md.Replace("<div>", "");
                md = md.Replace("</div>", "");

                // Replace html encoding
                md = md.Replace("&gt;", ">");
                

                return md;
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