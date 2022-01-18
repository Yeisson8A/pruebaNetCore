using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Instructores;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistencia.DapperConexion.Instructor;

namespace WebAPI.Controllers
{
    //Endpoint para acceder a dicho controlador
    //http://localhost:5000/api/Instructor
    public class InstructorController : MiControllerBase
    {
        //Se usa Authorize y Roles para indicar que el método del controlador tiene autorización por roles
        [Authorize(Roles = "Admin")]
        //Método GET
        [HttpGet]
        public async Task<ActionResult<List<InstructorModel>>> ObtenerInstructores()
        {
            return await Mediator.Send(new Consulta.Lista());
        }

        //Método POST
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            return await Mediator.Send(data);
        }

        //Método PUT
        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Actualizar(Guid id, Editar.Ejecuta data)
        {
            //Asignar id del instructor a actualizar al objeto con los datos
            data.InstructorId = id;
            return await Mediator.Send(data);
        }

        //Método DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await Mediator.Send(new Eliminar.Ejecuta{Id = id});
        }

        //Método GET
        [HttpGet("{id}")]
        public async Task<ActionResult<InstructorModel>> ObtenerPorId(Guid id)
        {
            return await Mediator.Send(new ConsultaId.Ejecuta{Id = id});
        }
    }
}