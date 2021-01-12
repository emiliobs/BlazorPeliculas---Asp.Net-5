using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorPeliculas.Shared.Entidades
{
     public class Genero
    {
        public int  Id { get; set; }

        [Required(ErrorMessage = "El Campo {0} es Requerido!")]
        public string Nombre { get; set; }
    }
}
