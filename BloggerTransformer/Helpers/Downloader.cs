using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BloggerTransformer.Helpers
{
    public class Downloader
    {
        private static HttpClient client = new HttpClient();
        public static Task Download(string url, string localFilename)
        {
            //Console.WriteLine("Download " + url + " to " + localFilename);
            
            // https://blogs.msdn.microsoft.com/henrikn/2012/02/17/httpclient-downloading-to-a-local-file/

            // Send asynchronous request
            return client.GetAsync(url).ContinueWith(
                (requestTask) =>
                    {
                        // Get HTTP response from completed task.
                        HttpResponseMessage response = requestTask.Result;
    
                        // Check that response was successful or throw exception
                        response.EnsureSuccessStatusCode();
    
                        // Read content into buffer
                        //response.Content.LoadIntoBufferAsync();

                        // The content can now be read multiple times using any ReadAs* extension method
                        response.Content.ReadAsFileAsync(localFilename, true).ContinueWith(
                       (readTask) =>
                       {
                           Process process = new Process();
                           process.StartInfo.FileName = localFilename;
                           process.Start();
                       });

                    });
        }
    }
}