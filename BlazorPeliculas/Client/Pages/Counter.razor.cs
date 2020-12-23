using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Pages
{
    public partial class Counter
    {
        [Inject]
        private ServiciosSingleton ServiciosSingleton { get; set; }

        [Inject]
        private ServiciosTransient ServiciosTransient { get; set; }

        [Inject]
        protected IJSRuntime Js { get; set; }

        private int currentCount = 0;
        static int currentCountStatic = 0;
        


        private async Task  IncrementCount()
        {
            currentCount++;
            ServiciosSingleton.Valor = currentCount;
            ServiciosTransient.Valor = currentCount;
            currentCountStatic++;

            await Js.InvokeVoidAsync("prubaPuntoNetStaric");

        }

        [JSInvokable]
        public static Task<int> ObtenerCurrentCount()
        {
            return Task.FromResult(currentCountStatic);
        }
    }
}
