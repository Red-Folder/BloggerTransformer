using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace BloggerTransformer.Models.Blogger
{
    public class Entry
    {
        [XmlElement("id")]
        public string Id;

        [XmlElement("category")]
        public List<Category> Categories;


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

                var kind = KindType.Unknown;
                switch (kindList.First().Term)
                {
                    case "http://schemas.google.com/blogger/2008/kind#template":
                        return KindType.Template;
                    case "http://schemas.google.com/blogger/2008/kind#settings":
                        return KindType.Settings;
                    case "http://schemas.google.com/blogger/2008/kind#post":
                        return KindType.Post;
                    default:
                        Console.WriteLine("[ERROR] Unknown Kind");
                        Console.WriteLine(kindList.First().Term);
                        throw new Exception("Unknown kind");
                }

                return kind;
            }
        }

        public List<string> Tags
        {
            get
            {
                return new List<string>();
            }
        }
    }

    public enum KindType
    {
        Template,
        Settings,
        Post,
        Unknown
    }
}