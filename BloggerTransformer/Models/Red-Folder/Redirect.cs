using System.Net;
using System.Runtime.Serialization;

namespace RedFolder.Website.Data
{
    [DataContract]
    public class Redirect
    {
        [DataMember(Name="redirectType")]
        public HttpStatusCode RedirectType { get; set; }
        [DataMember(Name="url")]
        public string Url { get; set; }
        [DataMember(Name="redirectByRoute")]
        public bool RedirectByRoute { get; set; }
        [DataMember(Name="redirectByParameter")]
        public bool RedirectByParameter { get; set; }
    }
}
