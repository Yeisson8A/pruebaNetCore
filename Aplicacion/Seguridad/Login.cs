using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class Login
    {
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string Email {get;set;}
            public string Password {get;set;}
        }

        //Clase para las validaciones con Fluent
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            //Constructor donde se definen las validaciones a aplicar
            public EjecutaValidacion()
            {
                //Regla a aplicar a campo Email
                RuleFor(x => x.Email).NotEmpty();
                //Regla a aplicar a campo Password
                RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly SignInManager<Usuario> _signInManager;
            private readonly IJwtGenerador _jwtGenerador;

            public Manejador(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, IJwtGenerador jwtGenerador)
            {
                _userManager = userManager;
                _signInManager = signInManager;
                _jwtGenerador = jwtGenerador;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Buscar el usuario por el correo en la base de datos usando Identity Core
                var usuario = await _userManager.FindByEmailAsync(request.Email);

                //Validar si se encontro el usuario
                if (usuario == null)
                {
                    //Devolver error, usuario no autorizado
                    throw new ManejadorExcepcion(HttpStatusCode.Unauthorized);
                }

                //Comprobar la contraseña
                var resultado = await _signInManager.CheckPasswordSignInAsync(usuario, request.Password, false);
                //Obtener listado de roles del usuario
                var roles = await _userManager.GetRolesAsync(usuario);
                //Convertir listado de roles obtenido a tipo List<string>
                var listaRoles = new List<string>(roles);

                //Validar si fue exitoso
                if (resultado.Succeeded)
                {
                    //Devolver información especifica del usuario tras logueo
                    return new UsuarioData{
                        NombreCompleto = usuario.NombreCompleto,
                        Token = _jwtGenerador.CrearToken(usuario, listaRoles), //Llamar método para crear token
                        Email = usuario.Email,
                        Username = usuario.UserName,
                        Imagen = null
                    };
                }

                //Devolver error, usuario no autorizado
                throw new ManejadorExcepcion(HttpStatusCode.Unauthorized);
            }
        }
    }
}