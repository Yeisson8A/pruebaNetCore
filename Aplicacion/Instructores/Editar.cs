using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Editar
    {
        public class Ejecuta : IRequest
        {
            public Guid InstructorId {get;set;}
            public string Nombre {get;set;}
            public string Apellidos {get;set;}
            public string Grado {get;set;}
        }

        //Clase para las validaciones con Fluent
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            //Constructor donde se definen las validaciones a aplicar
            public EjecutaValidacion()
            {
                //Regla a aplicar a campo Nombre
                RuleFor(x => x.Nombre).NotEmpty();
                //Regla a aplicar a campo Apellidos
                RuleFor(x => x.Apellidos).NotEmpty();
                //Regla a aplicar a campo Grado
                RuleFor(x => x.Grado).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly IInstructor _instructorRepository;

            public Manejador(IInstructor instructorRepository)
            {
                _instructorRepository = instructorRepository;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Ejecutar procedimiento mediante el Dapper y obtener cantidad de registros afectadas
                var resultado = await _instructorRepository.Actualizar(request.InstructorId, request.Nombre, request.Apellidos, request.Grado);

                //Validar respuesta obtenida de la ejecución del procedimiento almacenado
                if (resultado > 0)
                {
                    return Unit.Value;
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo actualizar los datos del instructor");
            }
        }
    }
}