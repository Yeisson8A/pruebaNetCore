using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Consulta
    {
        //Clase que representa lo que va a devolver
        public class Lista : IRequest<List<InstructorModel>>{}
        //Clase que va a manejar la operación,
        //recibe lo que va a devolver y el formato
        public class Manejador : IRequestHandler<Lista, List<InstructorModel>>
        {
            private readonly IInstructor _instructorRepository;

            public Manejador(IInstructor instructorRepository)
            {
                _instructorRepository = instructorRepository;
            }

            public async Task<List<InstructorModel>> Handle(Lista request, CancellationToken cancellationToken)
            {
                //Obtener datos de la ejecución del procedimiento mediante el Dapper
                var resultado = await _instructorRepository.ObtenerLista();
                //Devolver datos obtenidos como un tipo List
                return resultado.ToList();
            }
        }
    }
}