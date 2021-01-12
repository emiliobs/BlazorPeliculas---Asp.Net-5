using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Pages
{
    public partial class Counter
    {


        [Inject]
        protected IJSRuntime Js { get; set; }

        private int currentCount = 0;
        static int currentCountStatic = 0;
        IJSObjectReference modulo;


        [JSInvokable]
        public async Task IncrementCount()
        {

            var arreglo = new double[] { 1, 2, 3, 4, 5 };

            var max = arreglo.Maximum();
            var min = arreglo.Minimum();

            modulo = await Js.InvokeAsync<IJSObjectReference>("import", "./js/Counter.js");
            await modulo.InvokeVoidAsync("mostrarAlerta", $"El max es {max} y el min es {min}");

            currentCount++;

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
