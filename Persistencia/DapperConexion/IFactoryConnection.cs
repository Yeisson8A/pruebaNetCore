using System.Data;
namespace Persistencia.DapperConexion
{
    public interface IFactoryConnection
    {
        //Método para cerrar conexión
         void CloseConnection();
         //Método para obtener objeto conexión
         IDbConnection GetConnection();
    }
}