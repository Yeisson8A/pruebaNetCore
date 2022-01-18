using System;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Comentarios
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public string Alumno {get;set;}
            public int Puntaje {get;set;}
            public string Comentario {get;set;}
            public Guid CursoId {get;set;}
        }

        //Clase para las validaciones con Fluent
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            //Constructor donde se definen las validaciones a aplicar
            public EjecutaValidacion()
            {
                //Regla a aplicar a campo Alumno
                RuleFor(x => x.Alumno).NotEmpty();
                //Regla a aplicar a campo Puntaje
                RuleFor(x => x.Puntaje).NotEmpty();
                //Regla a aplicar a campo Comentario
                RuleFor(x => x.Comentario).NotEmpty();
                //Regla a aplicar a campo CursoId
                RuleFor(x => x.CursoId).NotEmpty();
            }
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
                //Crear objeto para el nuevo comentario a crear en la base datos
                var comentario = new Comentario{
                    ComentarioId = Guid.NewGuid(),
                    Alumno = request.Alumno,
                    Puntaje = request.Puntaje,
                    ComentarioTexto = request.Comentario,
                    CursoId = request.CursoId,
                    FechaCreacion = DateTime.UtcNow
                };

                //Agregar objeto a entidad
                _context.Comentario.Add(comentario);

                //Guardar en la base de datos
                var valor = await _context.SaveChangesAsync();

                //Validar respuesta obtenida de la inserción
                if (valor > 0)
                {
                    return Unit.Value;
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo insertar el comentario");
            }
        }
    }
}