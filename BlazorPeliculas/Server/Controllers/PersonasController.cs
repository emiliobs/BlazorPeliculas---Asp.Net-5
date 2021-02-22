﻿using BlazorPeliculas.Server.Datos;
using BlazorPeliculas.Server.Helpars;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly ApplicationDbContex _contex;
        private readonly IAlmacenadorArchivosAzStorage _almacenadorArchivos;
        private readonly string contendor = "personas";

        public PersonasController(ApplicationDbContex contex, IAlmacenadorArchivosAzStorage almacenadorArchivos)
        {
            _contex = contex;
            _almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<Persona>>> Get()
        {
            return await _contex.Personas.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(Persona persona)
        {
            if (!string.IsNullOrEmpty(persona.Foto))
            {
                var fotoPersonas = Convert.FromBase64String(persona.Foto);
                persona.Foto = await _almacenadorArchivos.GuardarArchivo(fotoPersonas, "jpg", contendor);
            }

            _contex.Add(persona);
            await _contex.SaveChangesAsync();

            return Ok(persona);
        }

    }
}
