using BlazorPeliculas.Shared.DTOs;

namespace BlazorPeliculas.Server.Controllers.UtilitiesClasses
{
    public class ParametrosBusquedaPeliculas
    {
        public int Pagina { get; set; } = 1;

        public int CantidadRegistros { get; set; } = 10;

        public PaginacionDTO PaginacionDTO => new PaginacionDTO
        {
            Pagina = Pagina,
            CantidadRegistros = CantidadRegistros
        };

        public string Titulo { get; set; }

        public int GeneroId { get; set; }

        public bool EnCartelera { get; set; }

        public bool Estrenos { get; set; }

        public bool MasVotadas { get; set; }
    }
}
