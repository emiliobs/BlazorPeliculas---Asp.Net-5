using BlazorPeliculas.Server.Datos;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonasController : ControllerBase
    {
        private readonly ApplicationDbContex _contex;

        public PersonasController(ApplicationDbContex contex)
        {
            this._contex = contex;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Persona  persona)
        {
            _contex.Add(persona);
            await _contex.SaveChangesAsync();

            return Ok(persona);
        }
    }
}
