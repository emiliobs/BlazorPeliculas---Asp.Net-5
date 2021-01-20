using BlazorPeliculas.Shared.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Datos
{
    public class ApplicationDbContex : DbContext
    {
        public ApplicationDbContex(DbContextOptions<ApplicationDbContex> options) : base(options)
        {

        }

        public DbSet<GeneroPelicula> GeneroPeliculas { get; set; }

        public DbSet<Pelicula> Peliculas { get; set; }

        public DbSet<Genero> Generos { get; set; }

        public DbSet<Persona> Personas { get; set; }

        public DbSet<PeliculaActor> PeliculaActors { get; set; }

        public DbSet<VotoPelicula> VotoPeliculas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Generar llave primaria compuesta:
            modelBuilder.Entity<GeneroPelicula>().HasKey(gp => new { gp.GeneroId, gp.PeliculaId });

            modelBuilder.Entity<PeliculaActor>().HasKey(pa => new { pa.PeliculaId, pa.PersonaId });

            base.OnModelCreating(modelBuilder);
        }

    }
}
