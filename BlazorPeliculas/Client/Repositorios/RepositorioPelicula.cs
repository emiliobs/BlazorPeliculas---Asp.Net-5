using BlazorPeliculas.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Repositorios
{
    public class RepositorioPelicula : IRepositorioPelicula
    {
        private readonly HttpClient _httpClient;

        public RepositorioPelicula(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private readonly JsonSerializerOptions OpcionesPorDefectoJSON = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public List<Pelicula> ObtenerPelicula()
        {
            return new List<Pelicula>
            {
                 new Pelicula { Titulo = "Spider-Man Far From Home", Lanzamiento = new DateTime(2020,5,5),
                 Poster = "https://m.media-amazon.com/images/M/MV5BZDEyN2NhMjgtMjdhNi00MmNlLWE5YTgtZGE4MzNjMTRlMGEwXkEyXkFqcGdeQXVyNDUyOTg3Njg@._V1_UX182_CR0,0,182,268_AL_.jpg"},
                 new Pelicula { Titulo = "Emiliana", Lanzamiento = new DateTime(2019,12,12),
                 Poster = "https://m.media-amazon.com/images/M/MV5BMjI4MzU5NTExNF5BMl5BanBnXkFtZTgwNzY1MTEwMDI@._V1_UX182_CR0,0,182,268_AL_.jpg"},
                 new Pelicula { Titulo = "Inception" ,Lanzamiento = new DateTime(2020,11,11),
                 Poster = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_UX182_CR0,0,182,268_AL_.jpg"},
             };
        }

        public async Task<HttpResponseWrapper<object>> Post<T>(string url, T enviar)
        {
            string enviarJSON = JsonSerializer.Serialize(enviar);
            StringContent enviarContent = new StringContent(enviarJSON, Encoding.UTF8, "application/json");
            HttpResponseMessage responseHttp = await _httpClient.PostAsync(url, enviarContent);
            return new HttpResponseWrapper<object>(!responseHttp.IsSuccessStatusCode, null, responseHttp);
        }

        public async Task<HttpResponseWrapper<object>> Put<T>(string url, T enviar)
        {
            string enviarJSON = JsonSerializer.Serialize(enviar);
            StringContent enviarContent = new StringContent(enviarJSON, Encoding.UTF8, "application/json");
            HttpResponseMessage responseHttp = await _httpClient.PutAsync(url, enviarContent);
            return new HttpResponseWrapper<object>(!responseHttp.IsSuccessStatusCode, null, responseHttp);
        }


        public async Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T enviar)
        {
            string enviarJSON = JsonSerializer.Serialize(enviar);
            StringContent enviarContent = new StringContent(enviarJSON, Encoding.UTF8, "application/json");
            HttpResponseMessage responseHttp = await _httpClient.PostAsync(url, enviarContent);
            if (responseHttp.IsSuccessStatusCode)
            {
                TResponse response = await DeserializarRespuesta<TResponse>(responseHttp, OpcionesPorDefectoJSON);
                return new HttpResponseWrapper<TResponse>(false, response, responseHttp);
            }
            else
            {
                return new HttpResponseWrapper<TResponse>(true, default, responseHttp);
            }
        }

        public async Task<HttpResponseWrapper<T>> Get<T>(string url)
        {
            HttpResponseMessage responseHTTP = await _httpClient.GetAsync(url);

            if (responseHTTP.IsSuccessStatusCode)
            {
                T response = await DeserializarRespuesta<T>(responseHTTP, OpcionesPorDefectoJSON);

                return new HttpResponseWrapper<T>(false, response, responseHTTP);
            }
            else
            {
                return new HttpResponseWrapper<T>(true, default, responseHTTP);
            }
        }

        private async Task<T> DeserializarRespuesta<T>(HttpResponseMessage httpResponseMessage, JsonSerializerOptions jsonSerializerOptions)
        {
            string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseString, jsonSerializerOptions);
        }

        public async Task<HttpResponseWrapper<object>> Delete(string url)
        {
            HttpResponseMessage responseHTTP = await _httpClient.DeleteAsync(url);

            return new HttpResponseWrapper<object>(!responseHTTP.IsSuccessStatusCode, null, responseHTTP);

        }
    }
}
