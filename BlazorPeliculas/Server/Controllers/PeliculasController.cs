﻿using AutoMapper;
using BlazorPeliculas.Server.Controllers.UtilitiesClasses;
using BlazorPeliculas.Server.Datos;
using BlazorPeliculas.Server.Helpars;
using BlazorPeliculas.Shared.DTOs;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize(Roles = "Admin")]
    public class PeliculasController : ControllerBase
    {
        private readonly ApplicationDbContex _context;
        private readonly IAlmacenadorArchivosAzStorage _almacenadorArchivos;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public PeliculasController(ApplicationDbContex context, IAlmacenadorArchivosAzStorage almacenadorArchivos,
                                   IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _almacenadorArchivos = almacenadorArchivos;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            int limite = 5;

            List<Pelicula> peliculasEnCartelera = await _context.Peliculas.Where(x => x.EnCartelera).Take(limite)
                                            .OrderByDescending(x => x.Lanzamiento).ToListAsync();

            DateTime fechaActual = DateTime.Today;

            List<Pelicula> proximosEstrenos = await _context.Peliculas.Where(x => x.Lanzamiento > fechaActual).OrderBy(x => x.Lanzamiento)
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
            Pelicula pelicula = await _context.Peliculas.Where(x => x.Id == id)
                                                  .Include(x => x.GeneroPeliculas)
                                                  .ThenInclude(x => x.Genero)
                                                  .Include(x => x.PeliculaActors)
                                                  .ThenInclude(x => x.Persona).FirstOrDefaultAsync();

            if (pelicula == null)
            {
                return NotFound();
            }

            // todo: sistema de votacion

            double promedioVotos = 0.0;
            int votoUsuario = 0;

            if (await _context.VotoPeliculas.AnyAsync(x => x.PeliculaId == id))
            {
                promedioVotos = await _context.VotoPeliculas.Where(x => x.PeliculaId == id)
                    .AverageAsync(x => x.Voto);

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    IdentityUser user = await _userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
                    //string userId = user.Id;

                    VotoPelicula votoUsuarioDB = await _context.VotoPeliculas
                        .FirstOrDefaultAsync(x => x.PeliculaId == id && x.UserId == user.Id);

                    if (votoUsuarioDB != null)
                    {
                        votoUsuario = votoUsuarioDB.Voto;
                    }
                }
            }

            pelicula.PeliculaActors = pelicula.PeliculaActors.OrderBy(x => x.Orden).ToList();

            var model = new PeliculaVisualizarDTO();
            model.Pelicula = pelicula;
            model.Generos = pelicula.GeneroPeliculas.Select(x => x.Genero).ToList();
            model.Actores = pelicula.PeliculaActors.Select(x =>
            new Persona
            {
                Nombre = x.Persona.Nombre,
                Foto = x.Persona.Foto,
                Personaje = x.Personaje,
                Id = x.PersonaId
            }).ToList();

            model.PromedioVotos = promedioVotos;
            model.VotoUsuario = votoUsuario;

            return model;

            // pelicula.PeliculaActors = pelicula.PeliculaActors.OrderBy(x => x.Orden).ToList();

            //PeliculaVisualizarDTO model = new PeliculaVisualizarDTO
            //{
            //    Pelicula = pelicula,
            //    Generos = pelicula.GeneroPeliculas.Select(x => x.Genero).ToList(),
            //    Actores = pelicula.PeliculaActors.Select(x => new Persona
            //    {
            //        Nombre = x.Persona.Nombre,
            //        Foto = x.Persona.Foto,
            //        Personaje = x.Personaje,
            //        Id = x.PersonaId,
            //    }).ToList(),

            //    PromedioVotos = promedioVotos,
            //    VotoUsuario = votoUsuario
            //};

            //return model;
        }

        [AllowAnonymous]
        [HttpGet("filtrar")]
        public async Task<ActionResult<List<Pelicula>>> Get([FromQuery] ParametrosBusquedaPeliculas parametrosBusquedaPeliculas)
        {
            IQueryable<Pelicula> peliculasQueryable = _context.Peliculas.AsQueryable();

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
            List<Genero> generosNoSeleccionados = await _context.Generos.Where(g => !generosSeleccionadosIds.Contains(g.Id)).ToListAsync();

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
            _context.Add(pelicula);
            await _context.SaveChangesAsync();
            return pelicula.Id;
        }

        [HttpPut]
        public async Task<ActionResult> Put(Pelicula pelicula)
        {
            Pelicula peliculaDB = await _context.Peliculas.FirstOrDefaultAsync(p => p.Id == pelicula.Id);
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

            await _context.Database.ExecuteSqlInterpolatedAsync($"delete from  GeneroPeliculas where PeliculaId = {pelicula.Id}; delete from PeliculaActors where PeliculaId = {pelicula.Id}");

            if (pelicula.PeliculaActors != null)
            {
                for (int i = 0; i < pelicula.PeliculaActors.Count; i++)
                {
                    pelicula.PeliculaActors[i].Orden = i + 1;
                }

            }

            peliculaDB.PeliculaActors = pelicula.PeliculaActors;
            peliculaDB.GeneroPeliculas = pelicula.GeneroPeliculas;

            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool existe = await _context.Peliculas.AnyAsync(g => g.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            _context.Remove(new Pelicula { Id = id });

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
