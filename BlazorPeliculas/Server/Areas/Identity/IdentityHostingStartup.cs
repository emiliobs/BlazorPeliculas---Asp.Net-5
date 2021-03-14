using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(BlazorPeliculas.Server.Areas.Identity.IdentityHostingStartup))]
namespace BlazorPeliculas.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}