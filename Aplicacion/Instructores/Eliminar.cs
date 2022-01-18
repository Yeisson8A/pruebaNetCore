using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Eliminar
    {
        public class Ejecuta : IRequest
        {
            public Guid Id {get;set;}
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
                var resultado = await _instructorRepository.Eliminar(request.Id);

                //Validar respuesta obtenida de la ejecución del procedimiento almacenado
                if (resultado > 0)
                {
                    return Unit.Value;
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo eliminar el instructor");
            }
        }
    }
}