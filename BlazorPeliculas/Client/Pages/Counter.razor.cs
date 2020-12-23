using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Drawing;
using System.Threading.Tasks;
using static BlazorPeliculas.Client.Shared.MainLayout;

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

        [CascadingParameter] public AppState appState  { get; set; }
      

        private int currentCount = 0;
        static int currentCountStatic = 0;
        IJSObjectReference modulo;


        [JSInvokable]
        public async Task IncrementCount()
        {
            modulo = await Js.InvokeAsync<IJSObjectReference>("import", "./js/Counter.js");
            await modulo.InvokeVoidAsync("mostrarAlerta", "Emilio Please styding too mach");

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

        protected async Task IncrementoCountJavascript()
        {
            await Js.InvokeVoidAsync("pruebaPuntoNETInstancia", DotNetObjectReference.Create(this));
        }
    }
}
