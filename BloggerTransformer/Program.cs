﻿using System;
using System.IO;
using System.Linq;
using BloggerTransformer.Models.Blogger;
using BloggerTransformer.Helpers;
using System.Xml.Serialization;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Console.WriteLine("Reading XML file");
            //var xml = File.ReadAllText("..\\data\\blog-11-15-2016.xml");
            //Console.WriteLine("Read XML file");

            //Console.WriteLine(String.Format("xml {0} characters long", xml.Length));

            Console.WriteLine("Converting XML file to object");
            //dynamic blogger = DynamicXml.Parse(xml);
            var fileStream = File.Open("c:\\tmp\\blogger.xml", FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Feed));
            var feed = (Feed)serializer.Deserialize(fileStream);
            //fileStream.Close();
            Console.WriteLine("Converted XML file to object");

            Console.WriteLine(feed.Id);
            Console.WriteLine(feed.Entries.Count);

            // Loop through each entry
            // Build up the Disqus comments as we go
            var comments = Exporter.NewRss();
            foreach (var entry in feed.Entries.Where(x => x.Kind == KindType.Post && !x.IsDraft))
            {
                //if (entry.Id == "tag:blogger.com,1999:blog-2744013729766746743.post-2815398180088438894" ||
                //    entry.Id == "tag:blogger.com,1999:blog-2744013729766746743.post-2941986872580699950" ||
                //    entry.Id == "tag:blogger.com,1999:blog-2744013729766746743.post-7607876749709462790")
                {

                    var graph = feed.Graph(entry);
                    Exporter.Export(graph, comments);
                }

            }
            Exporter.SaveDisqus(comments);

            Console.WriteLine("Finished");
        }

        

        private static void ReportPost(Entry post, EntryGraph graph)
        {
            Console.WriteLine("Id: " + post.Id);
            Console.WriteLine("Published: " + post.Published);
            Console.WriteLine("Updated: " + post.Updated);
            Console.WriteLine("Title: " + post.Title);
            Console.WriteLine("Url: " + post.Url);
            Console.WriteLine("Tags: " + String.Join(", ", post.Tags));
            Console.WriteLine("Author: " + post.Author.Name);
            Console.WriteLine("Content Length: " + post.Content.Length);

            ReportGraph(graph);
        } 

        private static void ReportGraph(EntryGraph graph, int indent = 0)
        {

            Console.WriteLine((new String('\t', indent)) + graph.Entry.Id);
            indent++;

            foreach (var child in graph.Children)
            {
                ReportGraph(child, indent);
            }
        }

        private static void IsNull(string name, object item)
        {
            if (item == null) 
            {
                Console.WriteLine(name + " is null");
            }
        }

        
    }
}
