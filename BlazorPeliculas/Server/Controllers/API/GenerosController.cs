﻿using BlazorPeliculas.Server.Datos;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContex _contex;

        public GenerosController(ApplicationDbContex contex)
        {
            this._contex = contex;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Genero genero)
        {
            _contex.Add(genero);
            await _contex.SaveChangesAsync();

            return Ok(genero);
        }

        [HttpGet]
        public async Task<ActionResult<List<Genero>>> Get()
        {
            return await _contex.Generos.ToListAsync();
        }
    }
}