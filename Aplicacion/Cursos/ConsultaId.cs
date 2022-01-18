using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class ConsultaId
    {
        public class CursoUnico : IRequest<CursoDto>
        {
            public Guid Id {get;set;}
        }

        public class Manejador : IRequestHandler<CursoUnico, CursoDto>
        {
            private readonly CursosOnlineContext _context;
            private readonly IMapper _mapper;

            //Inyectar context a través del constructor de la clase,
            //de forma que se pueda acceder a la base de datos
            public Manejador(CursosOnlineContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            //Con async y await se indica que la ejecución va a ser asincrona
            public async Task<CursoDto> Handle(CursoUnico request, CancellationToken cancellationToken)
            {
                //Se busca en la entidad Curso por Id en la base datos
                var curso = await _context.Curso
                //Con Include se incluye a la entidad Comentario y se obtiene su información
                .Include(x => x.ComentarioLista)
                //Con Include se incluye a la entidad Precio y se obtiene su información
                .Include(x => x.PrecioPromocion)
                //Con Include y ThenInclude se hace el enlace con la entidad Intructor y se obtiene su información
                .Include(x => x.InstructoresLink).ThenInclude(y => y.Instructor).FirstOrDefaultAsync(a => a.CursoId == request.Id);

                //Validar si el curso buscado existe
                if (curso == null)
                {
                    //Se lanza excepción usando la clase creada para el manejo de errores,
                    //la cual se utiliza en WebAPI como Middleware
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "No se encontro el curso"});
                }

                //Mapear de la entidad Curso a Curso DTO
                var cursoDto = _mapper.Map<Curso, CursoDto>(curso);
                //Devolver curso DTO
                return cursoDto;
            }
        }
    }
}