using System.Collections.Generic;
using Dominio;

namespace Aplicacion.Contratos
{
    public interface IJwtGenerador
    {
        //Método para crear token
         string CrearToken(Usuario usuario, List<string> roles);
    }
}