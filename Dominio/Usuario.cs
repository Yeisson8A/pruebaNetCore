using Microsoft.AspNetCore.Identity;

namespace Dominio
{
    //Clase que va a representar el usuario que se va a loguear a la aplicación
    //usa IdentityCore
    public class Usuario : IdentityUser
    {
        public string NombreCompleto {get;set;}
    }
}