using BlazorPeliculas.Client.Auth;
using BlazorPeliculas.Client.Helpers;
using BlazorPeliculas.Client.Repositorios;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            ConfigureServices(builder.Services);

            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {

            services.AddScoped<IRepositorioPelicula, RepositorioPelicula>();
            services.AddScoped<IMostrarMensajes, MostrarMensajes>();
            services.AddAuthorizationCore();
            services.AddScoped<ProveedorAutenticationJWT>();
            services.AddScoped<AuthenticationStateProvider, ProveedorAutenticationJWT>(                
                provider =>  provider.GetRequiredService<ProveedorAutenticationJWT>()   
                );
            services.AddScoped<ILoginService, ProveedorAutenticationJWT>(
                provider => provider.GetRequiredService<ProveedorAutenticationJWT>()
                );
        }
    }
}
