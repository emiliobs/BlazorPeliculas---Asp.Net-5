using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Helpars
{
    public interface IAlmacenadorArchivosAzStorage
    {
        Task<string> GuardarArchivo(byte[] contenido, string extension, string nombreContenedor);

        Task EliminarArchivos(string ruta, string nombreContenedor);

        Task<string> EditarArchivo(byte[] contenido, string extension, string nombreContenedor, string ruta);
    }
}
