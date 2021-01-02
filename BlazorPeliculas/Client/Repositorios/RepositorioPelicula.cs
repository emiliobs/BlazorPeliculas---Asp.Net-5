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
                 new Pelicula { Titulo = "Spider-Man Far From Home", Lanzamiento = new DateTime(2020,5,5),
                 Poster = "https://m.media-amazon.com/images/M/MV5BZDEyN2NhMjgtMjdhNi00MmNlLWE5YTgtZGE4MzNjMTRlMGEwXkEyXkFqcGdeQXVyNDUyOTg3Njg@._V1_UX182_CR0,0,182,268_AL_.jpg"},
                 new Pelicula { Titulo = "Maona", Lanzamiento = new DateTime(2019,12,12),
                 Poster = "https://m.media-amazon.com/images/M/MV5BMjI4MzU5NTExNF5BMl5BanBnXkFtZTgwNzY1MTEwMDI@._V1_UX182_CR0,0,182,268_AL_.jpg"},
                 new Pelicula { Titulo = "Inception" ,Lanzamiento = new DateTime(2020,11,11),
                 Poster = "https://m.media-amazon.com/images/M/MV5BMjAxMzY3NjcxNF5BMl5BanBnXkFtZTcwNTI5OTM0Mw@@._V1_UX182_CR0,0,182,268_AL_.jpg"},
             };
        }
    }
}
