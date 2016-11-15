using System;
using BloggerTransformer.Helpers;
using System.IO;
using System.Dynamic;

namespace ConsoleApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Reading XML file");
            var xml = File.ReadAllText("..\\data\\blog-11-15-2016.xml");
            Console.WriteLine("Read XML file");

            Console.WriteLine(String.Format("xml {0} characters long", xml.Length));

            Console.WriteLine("Converting XML file to object");
            dynamic blogger = DynamicXml.Parse(xml);
            Console.WriteLine("Converted XML file to object");

            IsNull("blogger", blogger);
            IsNull("blogger.Feed", blogger.Feed);
            //Console.WriteLine(String.Format("{0} entries found", blogger.@entry.Length));
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
