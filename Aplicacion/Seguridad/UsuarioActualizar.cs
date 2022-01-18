using System.Collections.Generic;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class UsuarioActualizar
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
            //Constructor para las diferentes reglas a aplicar
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
            private readonly IPasswordHasher<Usuario> _passwordHasher;

            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, 
                            IJwtGenerador jwtGenerador, IPasswordHasher<Usuario> passwordHasher)
            {
                _context = context;
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
                _passwordHasher = passwordHasher;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Buscar entre los usuarios el usuario que se va a actualizar
                var usuario = await _userManager.FindByNameAsync(request.Username);

                //Validar si el usuario a actualizar existe
                if (usuario == null)
                {
                    //Mandar error respecto a que el usuario a actualizar no existe
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "El usuario no existe"});
                }

                //Buscar si existe otro usuario con el mismo email
                var resultado = await _context.Users.Where(x => x.Email == request.Email && x.UserName != request.Username).AnyAsync();

                //Validar si ya existe otro usuario con el mismo email
                if (resultado)
                {
                    //Mandar error respecto a que el email a actualizar ya esta asociado
                    throw new ManejadorExcepcion(HttpStatusCode.InternalServerError, new {mensaje = "El email a actualizar ya se encuentra asignado a otro usuario"});
                }

                //Asociar los valores a actualizar del usuario
                usuario.NombreCompleto = request.NombreCompleto;
                usuario.Email = request.Email;
                //Se usa el objeto IPasswordHasher y el método HashPassword para encriptar el password
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);

                //Actualizar usuario
                var resultadoUpdate = await _userManager.UpdateAsync(usuario);
                //Obtener listado de roles asociados al usuario
                var roles = await _userManager.GetRolesAsync(usuario);
                //Convertir listado de roles obtenido a tipo List<string>
                var listaRoles = new List<string>(roles);

                //Validar si la actualización del usuario fue exitosa
                if (resultadoUpdate.Succeeded)
                {
                    //Devolver datos del usuario actualizado
                    return new UsuarioData{
                        NombreCompleto = usuario.NombreCompleto,
                        Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                        Email = usuario.Email,
                        Username = usuario.UserName
                    };
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo actualizar el usuario");
            }
        }
    }
}