using BlazorPeliculas.Client.Helper;
using BlazorPeliculas.Client.Repositorios;
using BlazorPeliculas.Shared.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;




namespace BlazorPeliculas.Client.Auth
{
    public class ProveedorAutenticationJWT : AuthenticationStateProvider, ILoginService
    {
        public static readonly string TOKENKEY = "TOKENKEY";
        public static readonly string EXPIRATIONTOKENKEY = "EXPIRATIONTOKENKEY";
        private readonly IJSRuntime _jS;
        private readonly HttpClient _httpClient;
        private readonly IRepositorioPelicula _repositorio;

        private AuthenticationState Anonimo => new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity()));

        public ProveedorAutenticationJWT(IJSRuntime jS, HttpClient httpClient, IRepositorioPelicula repositorio)
        {
            _jS = jS;
            _httpClient = httpClient;
            this._repositorio = repositorio;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            string token = await _jS.GetFromLocalStorage(TOKENKEY);

            if (string.IsNullOrEmpty(token))
            {
                return Anonimo;
            }

            var tiempoExpiracionString = await _jS.GetFromLocalStorage(EXPIRATIONTOKENKEY);
            DateTime tiempoExpiracion;
            if (DateTime.TryParse(tiempoExpiracionString, out tiempoExpiracion))
            {
                if (TokenExpirado(tiempoExpiracion))
                {
                    await Limpiar();
                    return Anonimo;
                }

                if (DebeRenovarToken(tiempoExpiracion))
                {
                    token = await RenovarToken(token);
                }
            }

            return ConstruirAuthenticationState(token);

        }

        public async Task ManejadorRenovacionToken()
        {
            var tiempoExpiracionString = await _jS.GetFromLocalStorage(EXPIRATIONTOKENKEY);
            DateTime tiempoExpiracion;

            if (DateTime.TryParse(tiempoExpiracionString, out tiempoExpiracion))
            {
                if (TokenExpirado(tiempoExpiracion))
                {
                    await Logout();
                }

                if (DebeRenovarToken(tiempoExpiracion))
                {
                    var token = await _jS.GetFromLocalStorage(TOKENKEY);
                    var nuevoToken = await RenovarToken(token);
                    var authState = ConstruirAuthenticationState(nuevoToken);
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }
            }
        }

        private async Task<string> RenovarToken(string token)
        {
            Console.WriteLine("Renovando el token...");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var nuevoTokenResponse = await _repositorio.Get<UserToken>("api/cuentas/RenovarToken");
            var nuevoToken = nuevoTokenResponse.Response;
            await _jS.SetInLocalStorage(TOKENKEY, nuevoToken.Token);
            await _jS.SetInLocalStorage(EXPIRATIONTOKENKEY, nuevoToken.Expiration.ToString());
            return nuevoToken.Token;
        }

        private bool DebeRenovarToken(DateTime tiempoExpiration)
        {
            return tiempoExpiration.Subtract((DateTime.UtcNow)) < TimeSpan.FromMinutes(5);
        }

        private bool TokenExpirado(DateTime tiempoExpiracion)
        {
            return tiempoExpiracion <= DateTime.UtcNow;
        }

        private AuthenticationState ConstruirAuthenticationState(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            List<Claim> claims = new List<Claim>();
            string payload = jwt.Split('.')[1];
            byte[] jsonBytes = ParseBase64WithoutPadding(payload);
            Dictionary<string, object> keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    string[] parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (string parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }

        public async Task Login(UserToken userToken)
        {
            await _jS.SetInLocalStorage(TOKENKEY, userToken.Token);
            await _jS.SetInLocalStorage(EXPIRATIONTOKENKEY, userToken.Token);
            AuthenticationState autState = ConstruirAuthenticationState(userToken.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(autState));
        }

        public async Task Logout()
        {
            await Limpiar();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }

        private async Task Limpiar()
        {
            await _jS.RemoveItem(TOKENKEY);
            await _jS.RemoveItem(EXPIRATIONTOKENKEY);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
