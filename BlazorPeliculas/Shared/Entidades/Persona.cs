using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entidades
{
    public class Persona
    {
        public int  Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public string Biografia { get; set; }

        public string Foto { get; set; }

        [Required]
        public DateTime? FechaDeNacimiento { get; set; }
    }
}
