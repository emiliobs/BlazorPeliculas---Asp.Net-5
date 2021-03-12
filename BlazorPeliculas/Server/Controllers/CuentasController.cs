
using BlazorPeliculas.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
                                IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("Crear")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] UserInfo userInfo)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userInfo.Email,
                Email = userInfo.Email,
            };

            IdentityResult result = await _userManager.CreateAsync(user, userInfo.Password);

            if (result.Succeeded)
            {
                return BuildToken(userInfo, new List<string>());
            }
            else
            {
                return BadRequest("Sorry!, UserName or Password invalid!!");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserToken>> Login([FromBody] UserInfo userInfo)
        {
            Microsoft.AspNetCore.Identity.SignInResult resutl = await _signInManager.PasswordSignInAsync(userInfo.Email, userInfo.Password, isPersistent: false,
                                                                  lockoutOnFailure: false);
            if (resutl.Succeeded)
            {
                IdentityUser usurios = await _userManager.FindByEmailAsync(userInfo.Email);
                var roles = await _userManager.GetRolesAsync(usurios);

                return BuildToken(userInfo, roles);
            }
            else
            {
                return BadRequest("Sorry!, Invalid Login Attemp.");
            }
        }

        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UserToken>> Renovar()
        {
            var userInfo = new UserInfo()
            {
                Email = HttpContext.User.Identity.Name
            };

            var usuario = await _userManager.FindByEmailAsync(userInfo.Email);
            var roles = await _userManager.GetRolesAsync(usuario);

            return BuildToken(userInfo, roles);
        }

        private UserToken BuildToken(UserInfo userInfo, IList<string> roles)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim("MiValor","Lo que yo quiera"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));

            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            SigningCredentials credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime experation = DateTime.UtcNow.AddMinutes(5);
            JwtSecurityToken token = new JwtSecurityToken(
                  issuer: null,
                  audience: null,
                  claims: claims,
                   expires: experation,
                   signingCredentials: credential
                );

            return new UserToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = experation,
            };
        }
    }
}
