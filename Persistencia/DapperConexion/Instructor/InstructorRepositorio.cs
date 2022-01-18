using System.Runtime.InteropServices;
using System.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorRepositorio : IInstructor
    {
        private readonly IFactoryConnection _factoryConnection;

        public InstructorRepositorio(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }

        public async Task<int> Actualizar(Guid instructorId, string nombre, string apellidos, string grado)
        {
            //Variable con el nombre del procedimiento almacenado a llamar
            var storeProcedure = "usp_Editar_Instructor";

            try
            {
                //Obtener objeto Connection
                var connection = _factoryConnection.GetConnection();
                //Se usa ExecuteAsync para ejecutar el procedimiento almacenado y obtener la cantidad de registros afectados
                var resultado = await connection.ExecuteAsync(storeProcedure, new {
                                    InstructorId = instructorId,
                                    Nombre = nombre,
                                    Apellidos = apellidos,
                                    Grado = grado
                                }, commandType : CommandType.StoredProcedure);

                //Cerrar la conexión
                _factoryConnection.CloseConnection();
                //Devolver resultado obtenido tras ejecución del procedimiento
                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo editar los datos del instructor", e);
            }
        }

        public async Task<int> Crear(string nombre, string apellidos, string grado)
        {
            //Variable con el nombre del procedimiento almacenado a llamar
            var storeProcedure = "usp_Crear_Instructor";

            try
            {
                //Obtener objeto Connection
                var connection = _factoryConnection.GetConnection();
                //Se usa ExecuteAsync para ejecutar el procedimiento almacenado y obtener la cantidad de registros afectados
                var resultado = await connection.ExecuteAsync(storeProcedure, new {
                                    InstructorId = Guid.NewGuid(),
                                    Nombre = nombre,
                                    Apellidos = apellidos,
                                    Grado = grado
                                }, commandType : CommandType.StoredProcedure);
                //Cerrar la conexión
                _factoryConnection.CloseConnection();
                //Devolver resultado obtenido tras ejecución del procedimiento                
                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo guardar el nuevo instructor", e);
            }
        }

        public async Task<int> Eliminar(Guid id)
        {
            //Variable con el nombre del procedimiento almacenado a llamar
            var storeProcedure = "usp_Eliminar_Instructor";

            try
            {
                //Obtener objeto Connection
                var connection = _factoryConnection.GetConnection();
                //Se usa ExecuteAsync para ejecutar el procedimiento almacenado y obtener la cantidad de registros afectados
                var resultado = await connection.ExecuteAsync(storeProcedure, new {
                                    InstructorId = id
                                }, commandType : CommandType.StoredProcedure);
                //Cerrar la conexión
                _factoryConnection.CloseConnection();
                //Devolver resultado obtenido tras ejecución del procedimiento
                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo eliminar el instructor", e);
            }
        }

        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            //Variable lista a devolver con los instructores
            IEnumerable<InstructorModel> instructorList = null;
            //Variable con el nombre del procedimiento almacenado a llamar
            var storeProcedure = "usp_Obtener_Instructores";

            try
            {
                //Obtener objeto connection
                 var connection = _factoryConnection.GetConnection();
                 //Se usa QueryAsync para ejecutar el procedimiento almacenado
                 instructorList = await connection.QueryAsync<InstructorModel>(storeProcedure, null, commandType : CommandType.StoredProcedure);
            }
            catch (Exception e)
            {
                throw new Exception("Error en la consulta de datos", e);
            }
            finally
            {
                //Cerrar la conexión
                _factoryConnection.CloseConnection();
            }
            //Devolver listado de instructores obtenidos
            return instructorList;
        }

        public async Task<InstructorModel> ObtenerPorId(Guid id)
        {
            //Variable con el nombre del procedimiento almacenado a llamar
            var storeProcedure = "usp_Obtener_Instructor_Id";
            InstructorModel instructor = null;

            try
            {
                //Obtener objeto connection
                var connection = _factoryConnection.GetConnection();
                //Se usa QueryFirstAsync para ejecutar el procedimiento almacenado
                instructor = await connection.QueryFirstAsync<InstructorModel>(storeProcedure, new{
                                    InstructorId = id
                                }, commandType : CommandType.StoredProcedure);
                //Cerrar la conexión
                _factoryConnection.CloseConnection();
                //Devolver resultado obtenido tras ejecución del procedimiento
                return instructor;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo encontrar el instructor", e);
            }
        }
    }
}