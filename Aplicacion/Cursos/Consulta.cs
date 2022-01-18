using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Consulta
    {
        //Clase que representa lo que va a devolver
        public class ListaCursos : IRequest<List<CursoDto>>{}
        //Clase que va a manejar la operación,
        //recibe lo que va a devolver y el formato
        public class Manejador : IRequestHandler<ListaCursos, List<CursoDto>>
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
            public async Task<List<CursoDto>> Handle(ListaCursos request, CancellationToken cancellationToken)
            {
                //Obtener información de los cursos de la base datos
                var cursos = await _context.Curso
                //Con Include se incluye a la entidad Comentario y se obtiene su información
                .Include(x => x.ComentarioLista)
                //Con Include se incluye a la entidad Precio y se obtiene su información
                .Include(x => x.PrecioPromocion)
                //Con Include y ThenInclude se hace el enlace con la entidad Intructor y se obtiene su información
                .Include(x => x.InstructoresLink).ThenInclude(x => x.Instructor).ToListAsync();
                //Se llama al mapper para mapear la lista entidad curso obtenida a una lista curso DTO a devolver
                var cursosDto = _mapper.Map<List<Curso>, List<CursoDto>>(cursos);
                //Devolver listado curso DTO
                return cursosDto;
            }
        }
    }
}