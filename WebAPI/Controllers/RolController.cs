using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Seguridad;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //Endpoint para acceder a dicho controlador
    //http://localhost:5000/api/Rol
    public class RolController : MiControllerBase
    {
        //Método POST
        [HttpPost("crear")]
        public async Task<ActionResult<Unit>> Crear(RolNuevo.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }

        //Método DELETE
        [HttpDelete("eliminar")]
        public async Task<ActionResult<Unit>> Eliminar(RolEliminar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }

        //Método GET
        [HttpGet("lista")]
        public async Task<ActionResult<List<IdentityRole>>> Lista()
        {
            return await Mediator.Send(new RolLista.Ejecuta());
        }

        //Método POST
        [HttpPost("agregarRolUsuario")]
        public async Task<ActionResult<Unit>> AgregarRolUsuario(UsuarioRolAgregar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }

        //Método POST
        [HttpPost("eliminarRolUsuario")]
        public async Task<ActionResult<Unit>> EliminarRolUsuario(UsuarioRolEliminar.Ejecuta parametros)
        {
            return await Mediator.Send(parametros);
        }

        //Método GET
        [HttpGet("{username}")]
        public async Task<ActionResult<List<string>>> ObtenerRolesUsuario(string username)
        {
            return await Mediator.Send(new RolesPorUsuario.Ejecuta{Username = username});
        }
    }
}