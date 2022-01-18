using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Aplicacion.Comentarios;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //Endpoint para acceder a dicho controlador
    //http://localhost:5000/api/Comentario
    public class ComentarioController : MiControllerBase
    {
        //Método POST
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            return await Mediator.Send(data);
        }

        //Método DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await Mediator.Send(new Eliminar.Ejecuta{Id = id});
        }
    }
}