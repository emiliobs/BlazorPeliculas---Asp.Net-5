using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Helpars
{
    public class AlmacenadorArchivoLocal
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AlmacenadorArchivoLocal(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string nombreContenedor, string ruta)
        {
            await EliminarArchivos(ruta, nombreContenedor);

            return await GuardarArchivo(contenido, extension, nombreContenedor);
        }

        public Task EliminarArchivos(string ruta, string nombreContenedor)
        {
            var fileName = Path.GetFileName(ruta);
            var directorioArchivo = Path.Combine(_webHostEnvironment.WebRootPath, nombreContenedor, fileName);
            if (File.Exists(directorioArchivo))
            {
                File.Delete(directorioArchivo);
            }

            return Task.FromResult(0);
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string nombreContenedor)
        {
            var fileName = $"{Guid.NewGuid()}.{extension}";
            var folder = Path.Combine(_webHostEnvironment.WebRootPath, nombreContenedor);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var rutaGuardado = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(rutaGuardado, contenido);

            var urlActual = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var rutaParaBD = Path.Combine(urlActual, nombreContenedor, fileName);

            return rutaParaBD;

        }

    }
}
