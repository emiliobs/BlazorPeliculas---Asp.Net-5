using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Auth
{
    public class ProveedorAutenticacionPrueba : AuthenticationStateProvider
    {
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //await Task.Delay(3000);

            var anonimo = new ClaimsIdentity(new List<Claim>()
            {
                new Claim("llave1", "valor1"),
                new Claim(ClaimTypes.Name, "Emilio"),
                //new Claim(ClaimTypes.Role, "Admin")
            });

            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonimo)));
        }
    }
}
