using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Helpers
{
    public class MostrarMensajes : IMostrarMensajes
    {
        private readonly IJSRuntime _jS;

        public MostrarMensajes(IJSRuntime jS)
        {
            this._jS = jS;
        }

        public async Task MostrarMensajeExitoso(string mensaje)
        {
            await MostrarMensaje("Exitoso", mensaje, "success");
        }



        public async Task MostrarMensajesError(string mensaje)
        {
            await MostrarMensaje("Error", mensaje, "error");
        }

        private async ValueTask MostrarMensaje(string titulo, string mensaje, string tipoMensaja)
        {
            await _jS.InvokeVoidAsync("Swal.fire",titulo, mensaje, tipoMensaja);
        }
    }
}
