using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Helper
{
    public static class IJSRuntimeExtensionMethods
    {
        public static async ValueTask<bool> Confirm(this IJSRuntime jS, string mensaje)
        {
            await jS.InvokeVoidAsync("console.log","Pruena de console log");

            return await jS.InvokeAsync<bool>("confirm", mensaje);
        }
    }
}
