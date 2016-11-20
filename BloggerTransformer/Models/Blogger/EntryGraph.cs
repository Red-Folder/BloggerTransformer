using System.Collections.Generic;

namespace BloggerTransformer.Models.Blogger
{
    public class EntryGraph
    {
        public Entry Entry;

        public List<EntryGraph> Children;
    }
}