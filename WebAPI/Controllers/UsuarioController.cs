using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using Dominio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //No implementar autorización
    [AllowAnonymous]
    public class UsuarioController : MiControllerBase
    {
        //http://localhost:5000/api/Usuario/login
        //Método POST
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioData>> Login(Login.Ejecuta parametros)
        {
            //Se usa al mediador para llamar al proyecto Aplicación, clase Login y su manejador
            //para control del login
            return await Mediator.Send(parametros);
        }

        //http://localhost:5000/api/Usuario/registrar
        //Método POST
        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioData>> Registrar(Registrar.Ejecuta parametros)
        {
            //Se usa al mediador para llamar al proyecto Aplicación, clase Registrar y su manejador
            //para registrar nuevo usuario
            return await Mediator.Send(parametros);
        }

        //http://localhost:5000/api/Usuario
        //Método GET
        [HttpGet]
        public async Task<ActionResult<UsuarioData>> ObtenerUsuario()
        {
            //Se usa al mediador para llamar al proyecto Aplicación, clase UsuarioActual y su manejador
            //para obtener los datos del usuario actual
            return await Mediator.Send(new UsuarioActual.Ejecuta());
        }

        //Método PUT
        [HttpPut]
        public async Task<ActionResult<UsuarioData>> Actualizar(UsuarioActualizar.Ejecuta parametros)
        {
            //Se usa al mediador para llamar al proyecto Aplicación, clase UsuarioActualizar y su manejador
            //para registrar actualizar un usuario existente
            return await Mediator.Send(parametros);
        }
    }
}