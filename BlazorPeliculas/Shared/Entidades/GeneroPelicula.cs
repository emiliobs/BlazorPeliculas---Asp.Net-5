namespace BlazorPeliculas.Shared.Entidades
{
    public class GeneroPelicula
    {
        public int PeliculaId { get; set; }

        public int GeneroId { get; set; }

        //propiedades de navegación
        public Genero Genero { get; set; }

        public Pelicula Pelicula { get; set; }
    }
}
