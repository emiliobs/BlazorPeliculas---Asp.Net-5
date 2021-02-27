using BlazorPeliculas.Server.Datos;
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

        [HttpGet]
        public async Task<ActionResult<List<Genero>>> Get()
        {
            return await _contex.Generos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Genero>> Get(int id)
        {
            return await _contex.Generos.FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Genero genero)
        {
            _contex.Add(genero);
            await _contex.SaveChangesAsync();

            return Ok(genero);
        }

        [HttpPut]
        public async Task<ActionResult> Put(Genero genero)
        {
            _contex.Attach(genero).State = EntityState.Modified;
            await _contex.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _contex.Generos.AnyAsync(g => g.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            _contex.Remove(new Genero { Id = id });

            await _contex.SaveChangesAsync();

            return NoContent();
        }
       
    }
}
