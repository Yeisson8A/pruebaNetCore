using System.IO.Compression;
using System.Linq;
using Aplicacion.Cursos;
using AutoMapper;
using Dominio;

namespace Aplicacion
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Crear mapeo de las clases EntityFramework y las clases DTO
            CreateMap<Curso, CursoDto>()
            //Se usa ForMember para hacer mapeo manual de la propiedad Instructores
            //Se usa MapFrom para hacer el mapeo manual de la entidad por medio del enlace hacia CursoInstructor
            //Se usa Select para seleccionar la información de la entidad Instructor
            .ForMember(x => x.Instructores, y => y.MapFrom(z => z.InstructoresLink.Select(a => a.Instructor).ToList()))
            //Se usa ForMember para hacer mapeo manual de la propiedad Comentarios
            //Se usa MapFrom para hacer el mapeo manual de la entidad Comentario
            .ForMember(x => x.Comentarios, y => y.MapFrom(z => z.ComentarioLista))
            //Se usa ForMember para hacer mapeo manual de la propiedad Precio
            //Se usa MapFrom para hacer el mapeo manual de la entidad Precio
            .ForMember(x => x.Precio, y => y.MapFrom(z => z.PrecioPromocion));
            CreateMap<CursoInstructor, CursoInstructorDto>();
            CreateMap<Instructor, InstructorDto>();
            CreateMap<Comentario, ComentarioDto>();
            CreateMap<Precio, PrecioDto>();
        }
    }
}