using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorPeliculas.Shared.Entidades
{
    public class Persona
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Biografia { get; set; }

        public string Foto { get; set; }

        [Required]
        public DateTime? FechaDeNacimiento { get; set; }

        public List<PeliculaActor> PeliculaActors { get; set; }

        [NotMapped]
        public string Personaje { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Persona p2)
            {
                return Id == p2.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
