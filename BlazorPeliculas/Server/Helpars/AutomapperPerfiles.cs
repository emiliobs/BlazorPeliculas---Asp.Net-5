using AutoMapper;
using BlazorPeliculas.Shared.Entidades;

namespace BlazorPeliculas.Server.Helpars
{
    public class AutomapperPerfiles : Profile
    {
        public AutomapperPerfiles()
        {
            CreateMap<Persona, Persona>().ForMember(p => p.Foto, option => option.Ignore());

            CreateMap<Pelicula, Pelicula>().ForMember(p => p.Poster, option => option.Ignore());
        }
    }
}
