using System.Net;
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Instructor;
using Aplicacion.ManejadorError;

namespace Aplicacion.Instructores
{
    public class ConsultaId
    {
        //Clase que representa lo que va a devolver
        public class Ejecuta : IRequest<InstructorModel>
        {
            public Guid Id {get;set;}
        }

        //Clase que va a manejar la operación,
        //recibe lo que va a devolver y el formato
        public class Manejador : IRequestHandler<Ejecuta, InstructorModel>
        {
            private readonly IInstructor _instructorRepository;

            public Manejador(IInstructor instructorRepository)
            {
                _instructorRepository = instructorRepository;
            }

            public async Task<InstructorModel> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Obtener datos de la ejecución del procedimiento mediante el Dapper
                var instructor = await _instructorRepository.ObtenerPorId(request.Id);

                //Validar si se obtuvo información del procedimiento
                if (instructor == null)
                {
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "No se encontró el instructor"});
                }

                //Devolver información del instructor obtenida del procedimiento
                return instructor;
            }
        }
    }
}