using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class UsuarioRolEliminar
    {
        public class Ejecuta : IRequest
        {
            public string Username {get;set;}
            public string RolNombre {get;set;}
        }

        //Clase para las validaciones con Fluent
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            //Constructor para las diferentes reglas a aplicar
            public EjecutaValidacion()
            {
                //Regla a aplicar al campo Username
                RuleFor(x => x.Username).NotEmpty();
                //Regla a aplicar al campo RolNombre
                RuleFor(x => x.RolNombre).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;

            public Manejador(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Buscar entre los roles el rol a quitar al usuario
                var role = await _roleManager.FindByNameAsync(request.RolNombre);

                //Validar si el rol a quitar al usuario existe
                if (role == null)
                {
                    //Mandar error respecto a que el rol a asociar no existe
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "El rol no existe"});
                }

                //Buscar entre los usuarios el usuario al cual se le va a quitar el rol
                var user = await _userManager.FindByNameAsync(request.Username);

                //Validar si el usuario al cual se le va a quitar el rol existe
                if (user == null)
                {
                    //Mandar error respecto a que el usuario al cual se le va a quitar el rol no existe
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "El usuario no existe"});
                }

                //Quitar rol al usuario
                var resultado = await _userManager.RemoveFromRoleAsync(user, request.RolNombre);

                //Validar si la eliminación del rol asociado al usuario es exitosa
                if (resultado.Succeeded)
                {
                    return Unit.Value;
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo eliminar el rol del usuario");
            }
        }
    }
}