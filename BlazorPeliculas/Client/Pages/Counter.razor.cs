using Microsoft.AspNetCore.Components;

namespace BlazorPeliculas.Client.Pages
{
    public partial class Counter
    {
        [Inject]
        private ServiciosSingleton ServiciosSingleton { get; set; }

        [Inject]
        private ServiciosTransient ServiciosTransient { get; set; }

        private int currentCount = 0;

        private void IncrementCount()
        {
            currentCount++;
            ServiciosSingleton.Valor = currentCount;
            ServiciosTransient.Valor = currentCount;
        }
    }
}
