using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Comentarios
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

            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Buscar comentario a eliminar
                var comentario = await _context.Comentario.FindAsync(request.Id);

                //Validar si se encontró el comentario
                if (comentario == null)
                {
                    //Se lanza excepción usando la clase creada para el manejo de errores,
                    //la cual se utiliza en WebAPI como Middleware
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "No se encontro el comentario"});
                }

                //Remover entidad
                _context.Remove(comentario);

                //Guardar en la base de datos
                var resultado = await _context.SaveChangesAsync();

                //Validar respuesta obtenida de la eliminación
                if (resultado > 0)
                {
                    return Unit.Value;
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo eliminar el comentario");
            }
        }
    }
}