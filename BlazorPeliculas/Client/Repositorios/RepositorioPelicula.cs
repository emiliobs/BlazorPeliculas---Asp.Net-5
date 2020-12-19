using BlazorPeliculas.Shared.Entidades;
using System;
using System.Collections.Generic;

namespace BlazorPeliculas.Client.Repositorios
{
    public class RepositorioPelicula : IRepositorioPelicula
    {
        public List<Pelicula> ObtenerPelicula()
        {
            return new List<Pelicula>
            {
                 new Pelicula { Titulo = "Spider-Man Far From Home", Lanzamiento = new DateTime(2020,5,5) },
                 new Pelicula { Titulo = "Maona", Lanzamiento = new DateTime(2019,12,12) },
                 new Pelicula { Titulo = "Inception" ,Lanzamiento = new DateTime(2020,11,11) },
             };
        }
    }
}
