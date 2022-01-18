using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class UsuarioActual
    {
        public class Ejecuta : IRequest<UsuarioData>{}
        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;
            private readonly IUsuarioSesion _usuarioSesion;

            public Manejador(UserManager<Usuario> userManager, IJwtGenerador jwtGenerador, IUsuarioSesion usuarioSesion)
            {
                _userManager = userManager;
                _jwtGenerador = jwtGenerador;
                _usuarioSesion = usuarioSesion;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Obtener información de usuario actual de la base datos
                var usuario = await _userManager.FindByNameAsync(_usuarioSesion.ObtenerUsuarioSesion());
                //Obtener listado de roles del usuario
                var roles = await _userManager.GetRolesAsync(usuario);
                //Convertir listado de roles obtenido a tipo List<string>
                var listaRoles = new List<string>(roles);

                //Devolver datos del usuario actual           
                return new UsuarioData{
                    NombreCompleto = usuario.NombreCompleto,
                    Username = usuario.UserName,
                    Email = usuario.Email,
                    Token = _jwtGenerador.CrearToken(usuario, listaRoles),
                    Imagen = null
                };
            }
        }
    }
}