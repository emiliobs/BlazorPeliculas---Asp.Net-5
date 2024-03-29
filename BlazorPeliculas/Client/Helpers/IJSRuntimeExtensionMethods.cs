﻿using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BlazorPeliculas.Client.Helper
{
    public static class IJSRuntimeExtensionMethods
    {
        public static async ValueTask InicializarTimerInactivo<T>(this IJSRuntime js,
            DotNetObjectReference<T> dotNetObjectReference) where T : class
        {
            await js.InvokeVoidAsync("timerInactivo", dotNetObjectReference);
        }

        public static async ValueTask<bool> Confirm(this IJSRuntime jS, string mensaje)
        {
            await jS.InvokeVoidAsync("console.log", "Pruena de console log");

            return await jS.InvokeAsync<bool>("confirm", mensaje);
        }

        public static ValueTask<object> SetInLocalStorage(this IJSRuntime js, string key, string content)
        {
            return js.InvokeAsync<object>("localStorage.setItem", key, content);
        }

        public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js, string key)
        {
            return js.InvokeAsync<string>("localStorage.getItem", key);
        }

        public static ValueTask<object> RemoveItem(this IJSRuntime js, string key)
        {
            return js.InvokeAsync<object>("localStorage.removeItem", key);
        }
    }
}
