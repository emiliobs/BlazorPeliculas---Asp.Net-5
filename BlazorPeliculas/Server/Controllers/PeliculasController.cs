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

        [HttpGet("{id}")]
        public async Task<ActionResult<PeliculaVisualizarDTO>> Get(int id)
        {
            var pelicula = await _contex.Peliculas.Where(x => x.Id == id)
                                                  .Include(x => x.GeneroPeliculas)
                                                  .ThenInclude(x => x.Genero)
                                                  .Include(x => x.PeliculaActors)
                                                  .ThenInclude(x => x.Persona).FirstOrDefaultAsync();

            if (pelicula == null)
            {
                return NotFound();
            }

            //todo: sistema de votación
            var promedioVotos = 4;
            var votoUsuario = 5;

            pelicula.PeliculaActors = pelicula.PeliculaActors.OrderBy(x => x.Orden).ToList();

            var model = new PeliculaVisualizarDTO();
            model.Pelicula = pelicula;
            model.Generos = pelicula.GeneroPeliculas.Select(x => x.Genero).ToList();
            model.Actores = pelicula.PeliculaActors.Select(x => new Persona
            {
                 Nombre = x.Persona.Nombre,
                 Foto = x.Persona.Foto,
                 Personaje = x.Personaje,
                 Id = x.PersonaId,
            }).ToList();

            model.PromedioVotos = promedioVotos;
            model.VotoUsuario = votoUsuario;

            return model;
        }

        [HttpGet("actualizar/{id}")]
        public async Task<ActionResult<PeliculaActualizacionDTO>> PutGet(int id)
        {
            var peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult)
            {
                return NotFound();
            }

            var peliculaVisualizarDTO = peliculaActionResult.Value;
            var generosSeleccionadosIds = peliculaVisualizarDTO.Generos.Select(p => p.Id).ToList();
            var generosNoSeleccionados = await _contex.Generos.Where(g => !generosSeleccionadosIds.Contains(g.Id)).ToListAsync();

            return new PeliculaActualizacionDTO
            {
                Pelicula = peliculaVisualizarDTO.Pelicula,
                GenerosNoSeleccionados = generosNoSeleccionados,
                GenerosSeleccionados = peliculaVisualizarDTO.Generos,
                Actores = peliculaVisualizarDTO.Actores,
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

            if (pelicula.PeliculaActors != null)
            {
                for (int i = 0; i < pelicula.PeliculaActors.Count; i++)
                {
                    pelicula.PeliculaActors[i].Orden = i + 1;
                }

            }
            _contex.Add(pelicula);
            await _contex.SaveChangesAsync();
            return pelicula.Id;
        }

       
    }
}
