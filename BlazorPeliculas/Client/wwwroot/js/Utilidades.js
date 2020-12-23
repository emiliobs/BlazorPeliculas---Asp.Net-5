function prubaPuntoNetStaric() {
    DotNet.invokeMethodAsync("BlazorPeliculas.Client", "ObtenerCurrentCount").then(resultado =>
    {
        console.log("Conteo desde javascript " + resultado);
    });
}