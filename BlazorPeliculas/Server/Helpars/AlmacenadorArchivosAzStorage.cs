using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Helpars
{
    public class AlmacenadorArchivosAzStorage : IAlmacenadorArchivosAzStorage
    {
        private readonly string _connectionString;

        public AlmacenadorArchivosAzStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, string nombreContenedor, string ruta)
        {
            await EliminarArchivos(ruta, nombreContenedor);

            return await GuardarArchivo(contenido, extension, nombreContenedor);
        }

        public async Task EliminarArchivos(string ruta, string nombreContenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }

            BlobContainerClient cliente = new BlobContainerClient(_connectionString, nombreContenedor);
            await cliente.CreateIfNotExistsAsync();
            string nombreArchivo = Path.GetFileName(ruta);
            BlobClient blob = cliente.GetBlobClient(nombreArchivo);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, string nombreContenedor)
        {
            BlobContainerClient cliente = new BlobContainerClient(_connectionString, nombreContenedor);
            await cliente.CreateIfNotExistsAsync();
            cliente.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            string archivoNombre = $"{Guid.NewGuid()}.{extension}";
            BlobClient blob = cliente.GetBlobClient(archivoNombre);
            using (MemoryStream ms = new MemoryStream(contenido))
            {
                await blob.UploadAsync(ms);
            }

            return blob.Uri.ToString();
        }
    }
}
