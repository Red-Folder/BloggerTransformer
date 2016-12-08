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
        private static int commentId = 1;

        private static int NextCommentId
        {
            get
            {
                return commentId++;
            }
        }

        private const string DESTINATION_BLOG_PATH = "https://www.red-folder.com/blog/";

        public const string CONTENTBASEFOLDER = "c:\\tmp\\blog\\";
        private const string MEDIABASEFOLDER = "c:\\tmp\\blog\\";
        private const string MEDIABASEPATH = "/media/blog/";

        private const string DESCRIPTION_BREAK_TAG = "[DESCRIPTION BREAK]";

        private const bool DOWNLOAD_ENABLED = false;

        public static void Export(EntryGraph graph, Rss comments)
        {
            var debug = false;

            Console.WriteLine(graph.Entry.Url);

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
            if (debug) Console.WriteLine("Add Carriage Returns");
            md = md.Replace("<br />", Environment.NewLine);

            // Map <hx>
            if (debug) Console.WriteLine("Map <hx>");
            md = Regex.Replace(md, "<h1>(.*?)</h1>", x =>
            {
                return "# " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
            });
            md = Regex.Replace(md, "<h2>(.*?)</h2>", x =>
            {
                return "## " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
            });
            md = Regex.Replace(md, "<h3>(.*?)</h3>", x =>
            {
                return "### " + x.Groups[1] + Environment.NewLine; // + new String('-', x.Groups[1].Length) + Environment.NewLine;
            });

            // Replace non-breaking spaces
            if (debug) Console.WriteLine("Replace non-breaking spaces");
            md = md.Replace("&nbsp;", " ");

            // Remove any meta Tags
            if (debug) Console.WriteLine("Remove any meta Tags");
            md = Regex.Replace(md, "<meta (.*?)/>" + Environment.NewLine, "");

            // Map blogger image
            if (debug) Console.WriteLine("Map blogger image");
            md = Regex.Replace(md, "<div class=\"separator\"(.*?)<a href=\"(.*?)\"(.*?)<img (.*?)src=\"(.*?)\"(.*?)</div>", x =>
            {
                return "![Image](" + ProcessImage(mediaFolder, mediaPath, x.Groups[2].Value) + ")" + Environment.NewLine;
            });

            // Remove summary breaks
            if (debug) Console.WriteLine("Remove summary breaks");
            md = md.Replace("<a name='more'></a>" + Environment.NewLine + Environment.NewLine, DESCRIPTION_BREAK_TAG);
            md = md.Replace("<a name='more'></a>" + Environment.NewLine, DESCRIPTION_BREAK_TAG);
            md = md.Replace("<a name='more'></a>", DESCRIPTION_BREAK_TAG);

            // Remove RFC Weekly header
            if (debug) Console.WriteLine("Remove RFC Weekly header");
            Regex rfcWeeklyHeaderRegex = new Regex("<a href=\"(.*?)Hopefully someone else will also find useful.", RegexOptions.Singleline);
            md = rfcWeeklyHeaderRegex.Replace(md, "");

            // Map Anchors
            if (debug) Console.WriteLine("Map Anchors");
            md = Regex.Replace(md, "<a (.*?)>(.*?)</a>", x =>
            {
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
            if (debug) Console.WriteLine("Map Lists");
            md = md.Replace("<ul>", Environment.NewLine);
            md = md.Replace("<li>", "* ");
            md = md.Replace("</li>", Environment.NewLine);
            md = md.Replace("</ul>", Environment.NewLine);

            // Gists
            if (debug) Console.WriteLine("Gists");
            md = Regex.Replace(md, "<script src=\"(.*?)\">(.*?)</script>", x => "%[" + x.Groups[1].Value + "]");

            // Remove Spread the love
            if (debug) Console.WriteLine("Remove Spread the love");
            md = Regex.Replace(md, "<div id=\"SpreadTheLove\">(.*?)</div>", "");

            // Remove Divs
            if (debug) Console.WriteLine("Remove Divs");
            md = Regex.Replace(md, "<div(.*?)>", "");
            md = md.Replace("</div>", "");

            // Replace html encoding
            if (debug) Console.WriteLine("Replace html encoding");
            md = md.Replace("&gt;", ">");

            // Remove empty lines at the start & end
            if (debug) Console.WriteLine("Remove empty lines at the start & end");
            while (md.StartsWith(Environment.NewLine))
            {
                md = md.Substring(Environment.NewLine.Length);
            }
            while (md.EndsWith(Environment.NewLine + Environment.NewLine))
            {
                md = md.Substring(0, md.Length - Environment.NewLine.Length);
            }

            // Load the Description (then remove the tag)
            if (debug) Console.WriteLine("Load the Description (then remove the tag)");
            if (md.Contains(DESCRIPTION_BREAK_TAG))
            {
                meta.Description = md.Substring(0, md.IndexOf(DESCRIPTION_BREAK_TAG));
                md = md.Replace(DESCRIPTION_BREAK_TAG, "");
            }

            // Set up the Keywords
            if (debug) Console.WriteLine("Setup the keywords");
            meta.KeyWords = graph.Entry.Tags;

            // RFC Weekly specific actions
            if (debug) Console.WriteLine("RFC Weekly specific actions");
            if (graph.Entry.Title.ToLower().Contains("rfc") && graph.Entry.Title.ToLower().Contains("weekly"))
            {
                // Add the image & description
                meta.Image = "/media/blog/rfc-weekly/RFCWeeklyTwitterCard.png";
                meta.Description = "RFC Weekly - a summary of things that I find interesting.  It is an indulgence; its the weekly update that I would like to receive.  Unfortunately no-one else is producing it so I figured I best get on with it.  Hopefully someone else will also find useful.";

                // Add as a key word
                if (!meta.KeyWords.Contains("RFCWeekly"))
                {
                    meta.KeyWords.Add("RFCWeekly");
                }
            }

            if (debug) Console.WriteLine("Save to file");
            SaveMarkdown(contentFolder, graph.Entry.Url, md);
            SaveMeta(contentFolder, graph.Entry.Url, meta);

            BuildRss(comments, meta, graph);
        }

        public static Rss NewRss()
        {
            var comments = new Rss();
            comments.Channel = new Channel();
            comments.Channel.Items = new List<Item>();
            return comments;
        }

        private static void BuildRss(Rss comments, Blog meta, EntryGraph graph)
        {
            if (graph.Children != null && graph.Children.Count > 0)
            {
                comments.Channel.Items.Add(new Item
                {
                    Title = meta.Title,
                    AbsoluteUrl = DESTINATION_BLOG_PATH + meta.Url,
                    Content = "",
                    ThreadIdentified = meta.Id,
                    Published = meta.Published,
                    Status = "open",
                    Comments = BuildComments(graph.Children, "")
                });
            }
        }

        private static List<Comment> BuildComments(List<EntryGraph> bloggerEntries, string parentId)
        {
            var comments = new List<Comment>();

            foreach (var entry in bloggerEntries)
            {
                var id = NextCommentId;
                comments.Add(new Comment
                {
                    Id = id.ToString(),
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
                    comments.AddRange(BuildComments(entry.Children, id.ToString()));
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

        public static void SaveDisqus(Rss comments)
        {
            using (var fileStream = File.Open(CONTENTBASEFOLDER + "\\disqus.xml", FileMode.Create))
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
                    Downloader.Download(imageUrl, exportFolder + "\\" + filename).Wait();
                }

                return newImageUrl;
            }
            return imageUrl;
        }
    }
}