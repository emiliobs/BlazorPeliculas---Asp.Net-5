using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Helpers
{
    public class HttpClientSinToken
    {
        public HttpClientSinToken(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }
    }
}
