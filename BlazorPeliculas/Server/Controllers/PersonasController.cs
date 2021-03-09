using AutoMapper;
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
    public class PersonasController : ControllerBase
    {
        private readonly ApplicationDbContex _contex;
        private readonly IAlmacenadorArchivosAzStorage _almacenadorArchivos;
        private readonly IMapper _mapper;
        //private readonly string contendor = "personas";

        public PersonasController(ApplicationDbContex contex, IAlmacenadorArchivosAzStorage almacenadorArchivos, IMapper mapper)
        {
            _contex = contex;
            _almacenadorArchivos = almacenadorArchivos;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Persona>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            IQueryable<Persona> queryable = _contex.Personas.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacionDTO.CantidadRegistros);


            return await queryable.Paginar(paginacionDTO).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Persona>> Get(int id)
        {
            Persona persona = await _contex.Personas.FirstOrDefaultAsync(x => x.Id == id);

            if (persona == null)
            {
                return NotFound();
            }

            return persona;
        }

        [HttpGet("buscar/{textoBusqueda}")]
        public async Task<ActionResult<List<Persona>>> Get(string textoBusqueda)
        {
            if (string.IsNullOrWhiteSpace(textoBusqueda))
            {
                return new List<Persona>();
            }

            textoBusqueda = textoBusqueda.ToLower();
            return await _contex.Personas.Where(x => x.Nombre.ToLower().Contains(textoBusqueda)).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Persona persona)
        {
            if (!string.IsNullOrEmpty(persona.Foto))
            {
                byte[] fotoPersonas = Convert.FromBase64String(persona.Foto);
                persona.Foto = await _almacenadorArchivos.GuardarArchivo(fotoPersonas, "jpg", "personas");
            }

            _contex.Add(persona);
            await _contex.SaveChangesAsync();

            return Ok(persona);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Persona persona)
        {
            Persona personaDB = await _contex.Personas.FirstOrDefaultAsync(p => p.Id == persona.Id);

            if (personaDB == null)
            {
                return NotFound();
            }

            //aqui mappeo las dos clases:
            personaDB = _mapper.Map(persona, personaDB);

            if (!string.IsNullOrWhiteSpace(persona.Foto))
            {
                byte[] fotoImagen = Convert.FromBase64String(persona.Foto);
                personaDB.Foto = await _almacenadorArchivos.EditarArchivo(fotoImagen,
                    "jpg", "personas", personaDB.Foto);
            }

            await _contex.SaveChangesAsync();

            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool existe = await _contex.Personas.AnyAsync(g => g.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            _contex.Remove(new Persona { Id = id });

            await _contex.SaveChangesAsync();

            return NoContent();
        }
    }
}
