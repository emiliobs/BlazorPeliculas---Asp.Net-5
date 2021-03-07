using AutoMapper;
using BlazorPeliculas.Server.Controllers.UtilitiesClasses;
using BlazorPeliculas.Server.Datos;
using BlazorPeliculas.Server.Helpars;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContex _contex;
        private readonly IAlmacenadorArchivosAzStorage _almacenadorArchivos;
        private readonly IMapper _mapper;

        public PeliculasController(ApplicationDbContex contex, IAlmacenadorArchivosAzStorage almacenadorArchivos,
                                   IMapper mapper)
        {
            _contex = contex;
            _almacenadorArchivos = almacenadorArchivos;
            _mapper = mapper;
        }

        [HttpGet]
       [AllowAnonymous]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            int limite = 5;

            List<Pelicula> peliculasEnCartelera = await _contex.Peliculas.Where(x => x.EnCartelera).Take(limite)
                                            .OrderByDescending(x => x.Lanzamiento).ToListAsync();

            DateTime fechaActual = DateTime.Today;

            List<Pelicula> proximosEstrenos = await _contex.Peliculas.Where(x => x.Lanzamiento > fechaActual).OrderBy(x => x.Lanzamiento)
                                         .Take(limite).ToListAsync();

            return new HomePageDTO
            {
                PeliculasEnCartelera = peliculasEnCartelera,
                ProximosEstrenos = proximosEstrenos,
            };

        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<PeliculaVisualizarDTO>> Get(int id)
        {
            Pelicula pelicula = await _contex.Peliculas.Where(x => x.Id == id)
                                                  .Include(x => x.GeneroPeliculas)
                                                  .ThenInclude(x => x.Genero)
                                                  .Include(x => x.PeliculaActors)
                                                  .ThenInclude(x => x.Persona).FirstOrDefaultAsync();

            if (pelicula == null)
            {
                return NotFound();
            }

            //todo: sistema de votación
            int promedioVotos = 4;
            int votoUsuario = 5;

            pelicula.PeliculaActors = pelicula.PeliculaActors.OrderBy(x => x.Orden).ToList();

            PeliculaVisualizarDTO model = new PeliculaVisualizarDTO
            {
                Pelicula = pelicula,
                Generos = pelicula.GeneroPeliculas.Select(x => x.Genero).ToList(),
                Actores = pelicula.PeliculaActors.Select(x => new Persona
                {
                    Nombre = x.Persona.Nombre,
                    Foto = x.Persona.Foto,
                    Personaje = x.Personaje,
                    Id = x.PersonaId,
                }).ToList(),

                PromedioVotos = promedioVotos,
                VotoUsuario = votoUsuario
            };

            return model;
        }

        [AllowAnonymous]
        [HttpGet("filtrar")]
        public async Task<ActionResult<List<Pelicula>>> Get([FromQuery] ParametrosBusquedaPeliculas parametrosBusquedaPeliculas)
        {
            IQueryable<Pelicula> peliculasQueryable = _contex.Peliculas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(parametrosBusquedaPeliculas.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.Titulo.ToLower().Contains(parametrosBusquedaPeliculas.Titulo.ToLower()));
            }

            if (parametrosBusquedaPeliculas.EnCartelera)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.EnCartelera);
            }

            if (parametrosBusquedaPeliculas.Estrenos)
            {
                DateTime hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(p => p.Lanzamiento >= hoy);
            }

            if (parametrosBusquedaPeliculas.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.GeneroPeliculas.Select(p => p.GeneroId)
                                                       .Contains(parametrosBusquedaPeliculas.GeneroId));
            }

            //TODO: Implementar votación

            await HttpContext.InsertarParametrosPaginacionEnRespuesta(peliculasQueryable, parametrosBusquedaPeliculas.CantidadRegistros);

            return await peliculasQueryable.Paginar(parametrosBusquedaPeliculas.PaginacionDTO).ToListAsync();
        }

        [HttpGet("actualizar/{id}")]
        public async Task<ActionResult<PeliculaActualizacionDTO>> PutGet(int id)
        {
            ActionResult<PeliculaVisualizarDTO> peliculaActionResult = await Get(id);
            if (peliculaActionResult.Result is NotFoundResult)
            {
                return NotFound();
            }

            PeliculaVisualizarDTO peliculaVisualizarDTO = peliculaActionResult.Value;
            List<int> generosSeleccionadosIds = peliculaVisualizarDTO.Generos.Select(p => p.Id).ToList();
            List<Genero> generosNoSeleccionados = await _contex.Generos.Where(g => !generosSeleccionadosIds.Contains(g.Id)).ToListAsync();

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
                byte[] fotoPoster = Convert.FromBase64String(pelicula.Poster);
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

        [HttpPut]
        public async Task<ActionResult> Put(Pelicula pelicula)
        {
            Pelicula peliculaDB = await _contex.Peliculas.FirstOrDefaultAsync(p => p.Id == pelicula.Id);
            if (peliculaDB == null)
            {
                return NotFound();
            }

            peliculaDB = _mapper.Map(pelicula, peliculaDB);

            if (!string.IsNullOrWhiteSpace(pelicula.Poster))
            {
                byte[] posterImage = Convert.FromBase64String(pelicula.Poster);
                peliculaDB.Poster = await _almacenadorArchivos.EditarArchivo(posterImage, "jpg", "peliculas", peliculaDB.Poster);
            }

            await _contex.Database.ExecuteSqlInterpolatedAsync($"delete from  GeneroPeliculas where PeliculaId = {pelicula.Id}; delete from PeliculaActors where PeliculaId = {pelicula.Id}");

            if (pelicula.PeliculaActors != null)
            {
                for (int i = 0; i < pelicula.PeliculaActors.Count; i++)
                {
                    pelicula.PeliculaActors[i].Orden = i + 1;
                }

            }

            peliculaDB.PeliculaActors = pelicula.PeliculaActors;
            peliculaDB.GeneroPeliculas = pelicula.GeneroPeliculas;

            await _contex.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool existe = await _contex.Peliculas.AnyAsync(g => g.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            _contex.Remove(new Pelicula { Id = id });

            await _contex.SaveChangesAsync();

            return NoContent();
        }

    }
}
