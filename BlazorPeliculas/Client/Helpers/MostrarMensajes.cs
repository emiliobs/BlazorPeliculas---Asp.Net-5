﻿using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Helpers
{
    public class MostrarMensajes : IMostrarMensajes
    {
        public async Task MostrarMensajeExitoso(string mensaje)
        {
            await Task.FromResult(0);
        }

        public async Task MostrarMensajesError(string mensaje)
        {
            await Task.FromResult(0);
        }
    }
}
