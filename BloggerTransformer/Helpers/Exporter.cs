using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using BloggerTransformer.Models.Blogger;
using System.Threading.Tasks;

namespace BloggerTransformer.Helpers
{
    public class Exporter
    {
        private const string CONTENTBASEFOLDER = "c:\\tmp\\blog\\";
        private const string MEDIABASEFOLDER = "c:\\tmp\\blog\\";
        private const string MEDIABASEPATH = "/media/blog/";

        public static void Export(EntryGraph graph)
        {
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

            // Remove summary breaks
            md = md.Replace("<a name='more'></a>" + Environment.NewLine + Environment.NewLine, "");

            // Map blogger image
            md = Regex.Replace(md, "<div class=\"separator\"(.*?)<a href=\"(.*?)\"(.*?)<img (.*?)src=\"(.*?)\"(.*?)</div>", x => {
               return "![Image](" + ProcessImage(mediaFolder, mediaPath, x.Groups[2].Value) + ")" + Environment.NewLine; 
            });

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
                
            SaveMarkdown(contentFolder, graph.Entry.Url, md);

            Console.WriteLine(graph.Entry.Url);
        }  
        private static void SaveMarkdown(string baseFolder, string exportFolder, string markdown)
        {
            File.WriteAllText(baseFolder + "\\" + exportFolder + ".md", markdown);
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

                Downloader.Download(imageUrl, exportFolder + "\\" +  filename).Wait();

                return newImageUrl;
            }
            return imageUrl;
        }

    }
}