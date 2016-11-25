using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace RedFolder.Website.Data
{
    [DataContract]
    public class Blog
    {
        [DataMember(Name="id")]
        public string Id { get; set; }
        [DataMember(Name="url")]
        public string Url { get; set; }
        [DataMember(Name="author")]
        public string Author { get; set; }
        [DataMember(Name="published")]
        public DateTime Published { get; set; }
        [DataMember(Name="modified")]
        public DateTime Modified { get; set; }
        [DataMember(Name="title")]
        public string Title { get; set; }
        [DataMember(Name="image")]
        public string Image { get; set; }
        [DataMember(Name="description")]
        public string Description { get; set; }
        [DataMember(Name="enabled")]
        public bool Enabled { get; set; }
        [DataMember(Name="redirects")]
        public List<Redirect> Redirects { get; set; }
    }
}
