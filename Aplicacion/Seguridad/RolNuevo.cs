using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class RolNuevo
    {
        public class Ejecuta : IRequest
        {
            public string Nombre {get;set;}
        }

        //Clase para las validaciones con Fluent
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            //Constructor para las diferentes reglas a aplicar
            public EjecutaValidacion()
            {
                //Regla a aplicar al campo Nombre
                RuleFor(x => x.Nombre).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            //Objeto de ASP Net Core para el manejo de roles
            private readonly RoleManager<IdentityRole> _roleManager;

            public Manejador(RoleManager<IdentityRole> roleManager)
            {
                _roleManager = roleManager;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Buscar entre los roles por el nombre del rol a crear
                var role = await _roleManager.FindByNameAsync(request.Nombre);

                //Validar si el rol a crear ya existe
                if (role != null)
                {
                    //Mandar error respecto a que el rol a crear ya existe
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new {mensaje = "Ya existe el rol"});
                }

                //Crear rol
                var resultado = await _roleManager.CreateAsync(new IdentityRole(request.Nombre));

                //Validar si la creación del rol fue exitosa
                if (resultado.Succeeded)
                {
                    return Unit.Value;                   
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo crear el rol");
            }
        }
    }
}