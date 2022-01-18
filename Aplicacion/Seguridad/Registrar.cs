using System.Net;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;
using System;
using FluentValidation;

namespace Aplicacion.Seguridad
{
    public class Registrar
    {
        public class Ejecuta : IRequest<UsuarioData>
        {
            public string NombreCompleto {get;set;}
            public string Email {get;set;}
            public string Password {get;set;}
            public string Username {get;set;}
        }

        //Clase para las validaciones con Fluent
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                //Regla a aplicar al campo Nombre Completo
                RuleFor(x => x.NombreCompleto).NotEmpty();
                //Regla a aplicar al campo Email
                RuleFor(x => x.Email).NotEmpty();
                //Regla a aplicar al campo Password
                RuleFor(x => x.Password).NotEmpty();
                //Regla a aplicar al campo Username
                RuleFor(x => x.Username).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly CursosOnlineContext _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador)
            {
                _context = context;
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Validar si ya existe un usuario con ese Email
                var existeEmail = await _context.Users.Where(x => x.Email == request.Email).AnyAsync();

                //Validar si se encontró el email
                if (existeEmail)
                {
                    //Mandar error Email ingresado ya existe
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new {mensaje = "El email ingresado ya existe"});
                }

                //Validar si ya existe un usuario con ese Username
                var existeUsername = await _context.Users.Where(x => x.UserName == request.Username).AnyAsync();

                if (existeUsername)
                {
                    //Mandar error Username ingresado ya existe
                    throw new ManejadorExcepcion(HttpStatusCode.BadRequest, new {mensaje = "El username ingresado ya existe"});
                }

                //Crear objeto con nuevo usuario a registrar
                var usuario = new Usuario{
                    NombreCompleto = request.NombreCompleto,
                    Email = request.Email,
                    UserName = request.Username
                };

                //Crear usuario
                var resultado = await _userManager.CreateAsync(usuario, request.Password);

                //Validar respuesta obtenida de la creación
                if (resultado.Succeeded)
                {
                    //Devolver datos del usuario creado
                    return new UsuarioData{
                        NombreCompleto = usuario.NombreCompleto,
                        Token = _jwtGenerador.CrearToken(usuario, null),
                        Username = usuario.UserName,
                        Email = usuario.Email
                    };
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo agregar al nuevo usuario");
            }
        }
    }
}