using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace BloggerTransformer.Models.Blogger
{
    // Ref: https://www.nuget.org/packages/System.Xml.XmlSerializer/

    [XmlRoot(Namespace = "http://www.w3.org/2005/Atom", ElementName = "feed")]
    public class Feed
    {
        [XmlElement(ElementName = "id")]
        public string Id;

        [XmlElement(ElementName = "entry")]
        public List<Entry> Entries;

        public EntryGraph Graph(Entry entry)
        {
            var childrenGraph = new List<EntryGraph>();
            var children = Entries.Where(x => x.InReplyTo !=null && x.InReplyTo.Ref == entry.Id && x.IsRootComment).ToList(); 

            foreach (var child in children)
            {
                childrenGraph.Add(GraphByRelated(child));
            }

            return new EntryGraph
            {
                Entry = entry,
                Children = childrenGraph
            };
        }

        public EntryGraph GraphByRelated(Entry entry)
        {
            var childrenGraph = new List<EntryGraph>();
            var children = Entries.Where(x => x.RelatedLink == entry.SelfLink).ToList();

            foreach (var child in children)
            {
                childrenGraph.Add(GraphByRelated(child));
            }

            return new EntryGraph
            {
                Entry = entry,
                Children = childrenGraph
            };
        }
    }
}