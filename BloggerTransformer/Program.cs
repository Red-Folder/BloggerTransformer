using System;
using System.IO;
using System.Linq;
using BloggerTransformer.Models.Blogger;
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

            //foreach (var entry in feed.Entries)
            //{
            //    Console.WriteLine(entry.Category.Term);
            //}

            foreach (var entry in feed.Entries)
            {
                Console.WriteLine(entry.Kind);
            }

            //IsNull("blogger", blogger);
            //Console.WriteLine(String.Format("{0} entries found", blogger.entry.Count));
            Console.WriteLine("Finished");
            Console.ReadKey();
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
