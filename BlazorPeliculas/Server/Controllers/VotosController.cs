using BlazorPeliculas.Server.Datos;
using BlazorPeliculas.Shared.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class VotosController : ControllerBase
    {
        private readonly ApplicationDbContex context;
        private readonly UserManager<IdentityUser> userManager;

        public VotosController(ApplicationDbContex context,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Votar(VotoPelicula votoPelicula)        
        {
            IdentityUser user = await userManager.FindByEmailAsync(HttpContext.User.Identity.Name);
            string userId = user.Id;
            VotoPelicula votoActual = await context.VotoPeliculas
                .FirstOrDefaultAsync(x => x.PeliculaId == votoPelicula.PeliculaId && x.UserId == userId);

            if (votoActual == null)
            {
                votoPelicula.UserId = userId;
                votoPelicula.FechaVoto = DateTime.Today;
                context.Add(votoPelicula);
                await context.SaveChangesAsync();
            }
            else
            {
                votoActual.Voto = votoPelicula.Voto;
                await context.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}
