using BlazorPeliculas.Shared.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Repositorios
{
    public interface IRepositorioPelicula
    {
        Task<HttpResponseWrapper<T>> Get<T>(string url);

        List<Pelicula> ObtenerPelicula();

        Task<HttpResponseWrapper<object>> Post<T>(string url, T enviar);

        Task<HttpResponseWrapper<TResponse>> Post<T, TResponse>(string url, T enviar);
    }
}
