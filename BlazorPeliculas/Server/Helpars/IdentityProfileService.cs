using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Helpars
{
    public class IdentityProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<IdentityUser> _claimsFactory;
        private readonly UserManager<IdentityUser> _userManager;

        public IdentityProfileService(IUserClaimsPrincipalFactory<IdentityUser> claimsFactory,
            UserManager<IdentityUser> userManager)
        {
            this._claimsFactory = claimsFactory;
            this._userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var usaurioId = context.Subject.GetSubjectId();
            var usuario = await _userManager.FindByIdAsync(usaurioId);
            var claimsPrincipal = await _claimsFactory.CreateAsync(usuario);
            var claims = claimsPrincipal.Claims.ToList();

            var claimsMapeados = new List<Claim>();

            foreach (var claim in claims)
            {
                if (claim.Type == JwtClaimTypes.Role)
                {
                    claimsMapeados.Add(new Claim(ClaimTypes.Role, claim.Value));
                }
            }

            claims.AddRange(claimsMapeados);
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var usaurioId = context.Subject.GetSubjectId();
            var usuario = await _userManager.FindByIdAsync(usaurioId);
            context.IsActive = usuario != null;
        }
    }
}
