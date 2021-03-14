using BlazorPeliculas.Server.Datos;
using BlazorPeliculas.Server.Helpars;
using BlazorPeliculas.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContex _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UsuariosController(ApplicationDbContex context, UserManager<IdentityUser> userManager,
                                  RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<UsuariosDTO>>> Get([FromQuery] PaginacionDTO paginacion)
        {
            IQueryable<IdentityUser> queryable = _context.Users.AsQueryable();
            await HttpContext.InsertarParametrosPaginacionEnRespuesta(queryable, paginacion.CantidadRegistros);
            return await queryable.Paginar(paginacion)
                .Select(x => new UsuariosDTO { Email = x.Email, UserId = x.Id }).ToListAsync();
        }

        [HttpGet("roles")]
        public async Task<ActionResult<List<RolDTO>>> Get()
        {
            return await _context.Roles.Select(r => new RolDTO { RolName = r.Name, RolId = r.Id }).ToListAsync();
        }

        [HttpPost("asignarRol")]
        public async Task<ActionResult> AsignarRolUsuario(EditarRolDTO editarRolDTO)
        {
            var usuarios = await _userManager.FindByIdAsync(editarRolDTO.UserId);
            await _userManager.AddToRoleAsync(usuarios, editarRolDTO.RoleId);

            return NoContent();
        }
               

        [HttpPost("removeRol")]
        public async Task<ActionResult> RemoveRolUsuario(EditarRolDTO editarRolDTO)
        {
            var usuarios = await _userManager.FindByIdAsync(editarRolDTO.UserId);
            await _userManager.RemoveFromRoleAsync(usuarios, editarRolDTO.RoleId);

            return NoContent();
        }
    }
}
