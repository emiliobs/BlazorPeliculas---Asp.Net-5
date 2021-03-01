using BlazorPeliculas.Shared.DTOs;
using System.Linq;

namespace BlazorPeliculas.Server.Helpars
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDTO paginacionDTO)
        {
            return queryable.Skip((paginacionDTO.Pagina - 1) * paginacionDTO.CantidadRegistros).Take(paginacionDTO.CantidadRegistros);
        }
    }
}
