﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Helpers
{
    public  class MostrarMensajes : IMostrarMensajes
    {
        public async Task MostrarMensajesError(string mensaje)
        {
            await Task.FromResult(0);
        }
    }
}
