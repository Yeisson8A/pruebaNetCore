using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Paginacion;

namespace Aplicacion.Cursos
{
    public class PaginacionCurso
    {
        public class Ejecuta : IRequest<PaginacionModel>
        {
            public string Titulo {get;set;}
            public int NumeroPagina {get;set;}
            public int CantidadElementos {get;set;}
        }

        public class Manejador : IRequestHandler<Ejecuta, PaginacionModel>
        {
            private readonly IPaginacion _paginacion;

            public Manejador(IPaginacion paginacion)
            {
                _paginacion = paginacion;
            }

            public async Task<PaginacionModel> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Variable con el nombre del procedimiento almacenado a llamar para obtener los cursos con paginación
                var StoredProcedure = "usp_Obtener_Curso_Paginacion";
                //Variable con el nombre de la columna a utilizar para el ordenamiento de los datos
                var ordenamiento = "Titulo";
                //Crear Dictionary para los parámetros de filtro del procedimiento
                var parametros = new Dictionary<string, object>();
                //Agregar parámetros de filtro
                parametros.Add("NombreCurso", request.Titulo);
                //Llamar al Repositorio Paginacion para ejecutar el procedimiento almacenado
                return await _paginacion.devolverPaginacion(StoredProcedure, request.NumeroPagina, 
                                                            request.CantidadElementos, parametros, ordenamiento);
            }
        }
    }
}