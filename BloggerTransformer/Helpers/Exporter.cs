using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using BloggerTransformer.Models.Blogger;


namespace BloggerTransformer.Helpers
{
    public class Exporter
    {
        public static void Export(EntryGraph graph)
        {

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
               return "![Image](" + x.Groups[2] + ")" + Environment.NewLine; 
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
                
            SaveMarkdown(md);
        }  
        private static void SaveMarkdown(string markdown)
        {
            File.WriteAllText("c:\\tmp\\output.md", markdown);
        }
    }
}