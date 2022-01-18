using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class RolesPorUsuario
    {
        public class Ejecuta : IRequest<List<string>>
        {
            public string Username {get;set;}
        }

        public class Manejador : IRequestHandler<Ejecuta, List<string>>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public Manejador(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public async Task<List<string>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Buscar entre los usuarios el usuario para el cual se va a obtener el listado de roles
                var user = await _userManager.FindByNameAsync(request.Username);

                //Validar si el usuario para el cual se va a obtener el listado de roles existe
                if (user == null)
                {
                    //Mandar error respecto a que el usuario al cual se va a obtener el listado de roles no existe
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "El usuario no existe"});
                }

                //Obtener listado de roles asociados al usuario
                var resultado = await _userManager.GetRolesAsync(user);
                //Devolver resultado obtenido convirtiendolo a tipo List<string>
                return new List<string>(resultado);
            }
        }
    }
}