using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Helpars
{
    public static class HttpContextExtensions
    {
        public static async Task InsertarParametrosPaginacionEnRespuesta<T>(this HttpContext httpContext,
                                 IQueryable<T> queryable, int cantidadRegistroAMostrar)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            double conteo = await queryable.CountAsync();
            double totalPaginas = Math.Ceiling(conteo / cantidadRegistroAMostrar);
            httpContext.Response.Headers.Add("conteo", conteo.ToString());
            httpContext.Response.Headers.Add("totalPaginas", totalPaginas.ToString());
        }
    }
}
