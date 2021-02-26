using AutoMapper;
using BlazorPeliculas.Shared.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPeliculas.Server.Helpars
{
    public class AutomapperPerfiles : Profile
    {
        public AutomapperPerfiles()
        {
            CreateMap<Persona, Persona>().ForMember(p => p.Foto, option => option.Ignore()); 
        }
    }
}
