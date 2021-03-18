using System.Net.Http;

namespace BlazorPeliculas.Client.Helpers
{
    public class HttpClientConToken
    {

        public HttpClientConToken(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }
    }
}
