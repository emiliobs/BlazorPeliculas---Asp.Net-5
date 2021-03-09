
using BlazorPeliculas.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
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
                return BuildToken(userInfo);
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
                return BuildToken(userInfo);
            }
            else
            {
                return BadRequest("Sorry!, Invalid Login Attemp.");
            }
        }

        private UserToken BuildToken(UserInfo userInfo)
        {
            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userInfo.Email),
                new Claim(ClaimTypes.Name, userInfo.Email),
                new Claim("MiValor","Lo que yo quiera"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            SigningCredentials credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime experation = DateTime.UtcNow.AddHours(1);
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
