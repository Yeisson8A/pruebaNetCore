using System.Collections;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionRepositorio : IPaginacion
    {
        private readonly IFactoryConnection _factoryConnection;

        public PaginacionRepositorio(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }

        public async Task<PaginacionModel> devolverPaginacion(string storeProcedure, int numeroPagina, int cantidadElementos, IDictionary<string, object> parametrosFiltro, string ordenamientoColumna)
        {
            //Crear objeto PaginacionModel a devolver
            PaginacionModel paginacionModel = new PaginacionModel();
            //Crear objeto que contendrá los datos obtenidos del procedimiento
            List<IDictionary<string, object>> listaReporte = null;
            //Crear variable para obtener cantidad registros tras ejecución procedimiento
            int totalRecords = 0;
            //Crear variable para obtener cantidad de páginas tras ejecución procedimiento
            int totalPaginas = 0;

            try
            {
                //Crear objeto Connection
                var connection = _factoryConnection.GetConnection();
                //Crear objeto que contendra los parámetros a enviar al procedimiento
                DynamicParameters parametros = new DynamicParameters();

                //Recorrer la lista de parámetros para filtro
                foreach (var param in parametrosFiltro)
                {
                    //Adicionar cada parámetro a usar como filtro de los datos
                    parametros.Add("@" + param.Key, param.Value);
                }

                //Adicionar cada parámetro de entrada del procedimiento
                parametros.Add("@NumeroPagina", numeroPagina);
                parametros.Add("@CantidadElementos", cantidadElementos);
                parametros.Add("@Ordenamiento", ordenamientoColumna);
                //Adicionar cada parámetro de salida del procedimiento
                parametros.Add("@TotalRecords", totalRecords, DbType.Int32, ParameterDirection.Output);
                parametros.Add("@TotalPaginas", totalPaginas, DbType.Int32, ParameterDirection.Output);

                //Se usa QueryAsync para ejecutar el procedimiento almacenado
                var result = await connection.QueryAsync(storeProcedure, parametros, commandType : CommandType.StoredProcedure);
                //Convertir los registros obtenidos de la ejecución del procedimiento en tipo IDictionary
                listaReporte = result.Select(x => (IDictionary<string, object>)x).ToList();
                //Asignar objeto IDictionary a objeto PaginacionModel a devolver
                paginacionModel.ListaRecords = listaReporte;
                //Asignar parámetro TotalRecords obtenido del procedimiento a objeto PaginacionModel a devolver
                paginacionModel.TotalRecords = parametros.Get<int>("@TotalRecords");
                //Asignar parámetro TotalPaginas obtenido del procedimiento a objeto PaginacionModel a devolver
                paginacionModel.NumeroPaginas = parametros.Get<int>("@TotalPaginas");
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo ejecutar el procedimiento almacenado", e);
            }
            finally
            {
                //Cerrar la conexión
                _factoryConnection.CloseConnection();
            }

            return paginacionModel;
        }
    }
}