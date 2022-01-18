using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta : IRequest
        {
            public Guid Id {get;set;}
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;

            //Inyectar context a través del constructor de la clase,
            //de forma que se pueda acceder a la base de datos
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Obtener los instructores del curso a eliminar
                var instructoresDB = _context.CursoInstructor.Where(x => x.CursoId == request.Id).ToList();

                //Recorrer listado de instructores de dicho curso
                foreach (var instructor in instructoresDB)
                {
                    //Eliminar instructor de la entidad CursoInstructor
                    _context.CursoInstructor.Remove(instructor);
                }

                //Obtener los comentarios del curso a eliminar
                var comentariosDB = _context.Comentario.Where(x => x.CursoId == request.Id).ToList();

                //Recorrer listado de comentarios de dicho curso
                foreach (var comentario in comentariosDB)
                {
                    //Eliminar comentario de la entidad Comentario
                    _context.Comentario.Remove(comentario);
                }

                //Obtener el precio del curso a eliminar
                var precioDB = _context.Precio.Where(x => x.CursoId == request.Id).FirstOrDefault();
                
                //Validar si el curso tiene precio asociado
                if (precioDB != null)
                {
                    //Eliminar precio de la entidad Precio
                    _context.Precio.Remove(precioDB);
                }

                //Buscar curso que se va a eliminar
                var curso = await _context.Curso.FindAsync(request.Id);

                //Validar si el curso a eliminar existe
                if (curso == null)
                {
                    //throw new Exception("No se puede eliminar el curso");
                    //Se lanza excepción usando la clase creada para el manejo de errores,
                    //la cual se utiliza en WebAPI como Middleware
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "No se encontro el curso"});
                }

                //Remover entidad
                _context.Remove(curso);
                //Guardar en la base de datos
                var resultado = await _context.SaveChangesAsync();

                //Validar respuesta obtenida de la eliminación
                if (resultado > 0)
                {
                    return Unit.Value;
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudieron guardar los cambios");
            }
        }
    }
}