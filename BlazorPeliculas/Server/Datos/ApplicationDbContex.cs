using BlazorPeliculas.Shared.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

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

            //SeedData:

            var personas = new List<Persona>();
            for (int i = 11; i < 1000; i++)
            {
                personas.Add(new Persona 
                {
                      Id = i, 
                      Nombre = $"Persona {i}",
                      FechaDeNacimiento = DateTime.Today,
                });
            }

            modelBuilder.Entity<Persona>().HasData(personas);

            base.OnModelCreating(modelBuilder);
        }

    }
}
