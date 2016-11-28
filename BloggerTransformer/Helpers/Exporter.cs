using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using BloggerTransformer.Models.Blogger;
using RedFolder.Website.Data;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using BloggerTransformer.Models.Disqus;
using System.Xml.Serialization;

namespace BloggerTransformer.Helpers
{
    public class Exporter
    {
        private const string DESTINATION_BLOG_PATH = "https://www.red-folder.com/blog/";

        private const string CONTENTBASEFOLDER = "c:\\tmp\\blog\\";
        private const string MEDIABASEFOLDER = "c:\\tmp\\blog\\";
        private const string MEDIABASEPATH = "/media/blog/";

        private const string DESCRIPTION_BREAK_TAG = "[DESCRIPTION BREAK]";

        private const bool DOWNLOAD_ENABLED = false;

        public static void Export(EntryGraph graph)
        {
            Blog meta = new Blog();
            meta.Id = Guid.NewGuid().ToString();
            meta.Url = graph.Entry.Url;
            meta.Author = graph.Entry.Author.Name;
            meta.Published = graph.Entry.Published;
            meta.Modified = graph.Entry.Updated;
            meta.Title = graph.Entry.Title;
            //meta.Image;
            //meta.Description;
            meta.Enabled = true;
            meta.Redirects = new List<Redirect>
            {
                new Redirect
                {
                    Url = graph.Entry.BloggerUrl,
                    RedirectType = HttpStatusCode.MovedPermanently,
                    RedirectByParameter = true,
                    RedirectByRoute = false
                }
            };

            string contentFolder = CreateContentFolder(graph.Entry.Url);
            string mediaFolder = CreateMediaFolder(graph.Entry.Url);
            string mediaPath = MEDIABASEPATH + graph.Entry.Url;

            var md = graph.Entry.Content;

            // Add Carriage Returns
            md = md.Replace("<br />", Environment.NewLine);

            // Map <hx>
            md = Regex.Replace(md, "<h1>(.*?)</h1>", x => {
                return "# " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
            });
            md = Regex.Replace(md, "<h2>(.*?)</h2>", x => {
                return "## " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
            });
            md = Regex.Replace(md, "<h3>(.*?)</h3>", x => {
                return "### " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
            });

            // Replace non-breaking spaces
            md = md.Replace("&nbsp;"," ");

            // Remove any meta Tags
            md = Regex.Replace(md, "<meta (.*?)/>" + Environment.NewLine, "");            

            // Map blogger image
            md = Regex.Replace(md, "<div class=\"separator\"(.*?)<a href=\"(.*?)\"(.*?)<img (.*?)src=\"(.*?)\"(.*?)</div>", x => {
               return "![Image](" + ProcessImage(mediaFolder, mediaPath, x.Groups[2].Value) + ")" + Environment.NewLine; 
            });

            // Remove summary breaks
            md = md.Replace("<a name='more'></a>" + Environment.NewLine + Environment.NewLine, DESCRIPTION_BREAK_TAG);
            
            // Remove RFC Weekly header
            Regex rfcWeeklyHeaderRegex = new Regex("<a href=\"(.*?)Hopefully someone else will also find useful.", RegexOptions.Singleline);
            md = rfcWeeklyHeaderRegex.Replace(md, "");

            // Map Anchors
            md = Regex.Replace(md, "<a (.*?)>(.*?)</a>", x => {
                var sb = new StringBuilder();
                sb.Append("[");
                sb.Append(x.Groups[2]);
                sb.Append("]");
                sb.Append("(");
                sb.Append(Regex.Matches(x.Groups[1].ToString(), "href=\"(.*?)\"")[0].Groups[1]);
                sb.Append(")");
                return sb.ToString();
            });

            // Map Lists
            md = md.Replace("<ul>", Environment.NewLine);
            md = md.Replace("<li>", "* ");
            md = md.Replace("</li>", Environment.NewLine);
            md = md.Replace("</ul>", Environment.NewLine);

            // Gists
            md = Regex.Replace(md, "<script src=\"(.*?)\">(.*?)</script>", x=> "%[" + x.Groups[1].Value + "]");

            // Remove Spread the love
            md = Regex.Replace(md, "<div id=\"SpreadTheLove\">(.*?)</div>", "");

            // Remove Divs
            md = Regex.Replace(md, "<div(.*?)>", "");
            md = md.Replace("</div>", "");

            // Replace html encoding
            md = md.Replace("&gt;", ">");

            // Remove empty lines at the start & end
            while (md.StartsWith(Environment.NewLine))
            {
                md = md.Substring(Environment.NewLine.Length);
            }
            while (md.EndsWith(Environment.NewLine + Environment.NewLine))
            {
                md = md.Substring(0, md.Length - Environment.NewLine.Length);
            }

            // Load the Description (then remove the tag)
            if (md.Contains(DESCRIPTION_BREAK_TAG))
            {
                meta.Description = md.Substring(0, md.IndexOf(DESCRIPTION_BREAK_TAG));
                md = md.Replace(DESCRIPTION_BREAK_TAG, "");
            }

            // Set up the Keywords - add RFC Weekly if necessary
            meta.KeyWords = graph.Entry.Tags;
            if (graph.Entry.Title.ToLower().Contains("rfc") && graph.Entry.Title.ToLower().Contains("weekly"))
            {
                if (!meta.KeyWords.Contains("RFCWeekly"))
                {
                    meta.KeyWords.Add("RFCWeekly");
                }
            }
                
            SaveMarkdown(contentFolder, graph.Entry.Url, md);
            SaveMeta(contentFolder, graph.Entry.Url, meta);

            var comments = BuildRss(meta, graph);
            SaveDisqus(contentFolder, graph.Entry.Url, comments);

            Console.WriteLine(graph.Entry.Url);
        }

        private static Rss BuildRss(Blog meta, EntryGraph graph)
        {
            if (graph.Children != null && graph.Children.Count > 0)
            {
                var comments = new Rss();
                comments.Channel = new Channel();
                comments.Channel.Items = new List<Item>();
                comments.Channel.Items.Add(new Item {
                    Title = meta.Title,
                    AbsoluteUrl = DESTINATION_BLOG_PATH + meta.Url,
                    Content = "",
                    ThreadIdentified = "",
                    Published = meta.Published,
                    Status = "open",
                    Comments = BuildComments(graph.Children, "")
                });


                return comments;
            }
            else 
            {
                return null;
            }
        }

        private static List<Comment> BuildComments(List<EntryGraph> bloggerEntries, string parentId)
        {
            var comments = new List<Comment>();

            foreach (var entry in bloggerEntries)
            {
                var id = Guid.NewGuid().ToString();
                comments.Add(new Comment
                {
                    Id = id,
                    Author = entry.Entry.Author.Name,
                    AuthorEmail = "",
                    AuthorUrl = "",
                    AuthorIP = "",
                    Published = entry.Entry.Published,
                    Content = entry.Entry.Content,
                    Approved = true,
                    ParentId = parentId
                });

                if (entry.Children != null && entry.Children.Count > 0)
                {
                    comments.AddRange(BuildComments(entry.Children, parentId));
                }
            }

            return comments;
        }  

        private static void SaveMarkdown(string baseFolder, string exportFolder, string markdown)
        {
            File.WriteAllText(baseFolder + "\\" + exportFolder + ".md", markdown);
        }

        private static void SaveMeta(string baseFolder, string exportFolder, Blog meta)
        {
            File.WriteAllText(baseFolder + "\\" + exportFolder + ".json", JsonConvert.SerializeObject(meta));
        }

        private static void SaveDisqus(string baseFolder, string exportFolder, Rss comments)
        {
            using (var fileStream = File.Open(baseFolder + "\\" + exportFolder + ".xml", FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Rss));
                serializer.Serialize(fileStream, comments);
            }
        }

        private static string CreateContentFolder(string folderName)
        {
            return CreateFolder(CONTENTBASEFOLDER + folderName);
        }

        private static string CreateMediaFolder(string folderName)
        {
            return CreateFolder(MEDIABASEFOLDER + folderName);
        }
        
        private static string CreateFolder(string folderName)
        {
            if (Directory.Exists(folderName))
            {
                Directory.Delete(folderName, true);
            }

            Directory.CreateDirectory(folderName);

            return folderName;
        }

        private static string ProcessImage(string exportFolder, string relativePath, string imageUrl)
        {
            if (imageUrl.Contains("blogspot.com"))
            {
                var filename = imageUrl.Substring(imageUrl.LastIndexOf("/") + 1);
                var newImageUrl = relativePath + filename;

                if (DOWNLOAD_ENABLED)
                {
                    Downloader.Download(imageUrl, exportFolder + "\\" +  filename).Wait();
                }

                return newImageUrl;
            }
            return imageUrl;
        }
    }
}