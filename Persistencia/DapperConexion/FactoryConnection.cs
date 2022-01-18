using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Persistencia.DapperConexion
{
    public class FactoryConnection : IFactoryConnection
    {
        private IDbConnection _connection;
        private readonly IOptions<ConexionConfiguracion> _configs;

        public FactoryConnection(IOptions<ConexionConfiguracion> configs)
        {
            _configs = configs;
        }

        public void CloseConnection()
        {
            //Validar que el objeto connection no sea nulo y la conexión este abierta
            if (_connection != null && _connection.State == ConnectionState.Open)
            {
                //Cerrar la conexión
                _connection.Close();
            }
        }

        public IDbConnection GetConnection()
        {
            //Validar si el objeto connection es nulo
            if (_connection == null)
            {
                //Crear un objeto connection con base en la cadena de conexión
                _connection = new SqlConnection(_configs.Value.DefaultConnection);
            }

            //Validar si la conexión no esta abierta, a fin de abrirla
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
            //Devolver objeto connection
            return _connection;
        }
    }
}