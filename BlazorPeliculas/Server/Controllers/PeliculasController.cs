using BlazorPeliculas.Server.Datos;
using BlazorPeliculas.Server.Helpars;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContex _contex;
        private readonly IAlmacenadorArchivosAzStorage _almacenadorArchivos;

        public PeliculasController(ApplicationDbContex contex, IAlmacenadorArchivosAzStorage almacenadorArchivos)
        {
            this._contex = contex;
            this._almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var limite = 5;

            var peliculasEnCartelera = await _contex.Peliculas.Where(x => x.EnCartelera).Take(limite)
                                            .OrderByDescending(x => x.Lanzamiento).ToListAsync();

            var fechaActual = DateTime.Today;

            var proximosEstrenos = await _contex.Peliculas.Where(x => x.Lanzamiento > fechaActual).OrderBy(x => x.Lanzamiento)
                                         .Take(limite).ToListAsync();

            return new HomePageDTO 
            {
               PeliculasEnCartelera = peliculasEnCartelera,
               ProximosEstrenos = proximosEstrenos,
            };

        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(Pelicula pelicula)
        {
            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                var fotoPoster = Convert.FromBase64String(pelicula.Poster);
                pelicula.Poster = await _almacenadorArchivos.GuardarArchivo(fotoPoster, "jpg", "peliculas");
            }

            _contex.Add(pelicula);
            await _contex.SaveChangesAsync();
            return pelicula.Id;
        }

       
    }
}
