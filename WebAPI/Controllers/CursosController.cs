using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Cursos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistencia.DapperConexion.Paginacion;

namespace WebAPI.Controllers
{
    //Endpoint para acceder a dicho controlador
    //http://localhost:5000/api/Cursos
    //[Route("api/[controller]")]
    //[ApiController]
    //Hereda de MiControllerBase es decir el controlador generico creado
    public class CursosController : MiControllerBase
    {
        /*private readonly IMediator _mediator;
        public CursosController(IMediator mediator)
        {
            _mediator = mediator;
        }*/

        //Método GET
        [HttpGet]
        //Implementa autorización
        //[Authorize]
        public async Task<ActionResult<List<CursoDto>>> Get()
        {
            //Se usa al mediador para llamar al proyecto Aplicación, clase Consulta y clase Lista Cursos
            //es decir la clase modelo de lo que se va a devolver
            return await Mediator.Send(new Consulta.ListaCursos());
        }

        //Método GET con parámetro
        //http://localhost:5000/api/Cursos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDto>> Detalle(Guid id)
        {
            //Se usa al mediador para llamar al proyecto Aplicación, clase ConsultaId y clase CursoUnico
            //es decir la clase modelo de lo que se va a devolver indicandole el Id recibido
            return await Mediator.Send(new ConsultaId.CursoUnico{Id = id});
        }

        //Método POST
        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            //Se usa al mediador para llamar al proyecto Aplicación, clase Nuevo 
            //y que cree el nuevo objeto de Curso en la base de datos
            return await Mediator.Send(data);
        }

        //Método PUT
        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Editar(Guid id, Editar.Ejecuta data)
        {
            //Se asigna ID recibido del curso a editar
            data.CursoId = id;
            //Se usa al mediador para llamar al proyecto Aplicación, clase Editar 
            //y que actualice el objeto de Curso en la base de datos
            return await Mediator.Send(data);
        }

        //Método DELETE
        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            //Se usa al mediador para llamar al proyecto Aplicación, clase Eliminar
            //y que elimine el Curso en la base de datos
            return await Mediator.Send(new Eliminar.Ejecuta{Id = id});
        }

        //Método POST
        [HttpPost("report")]
        public async Task<ActionResult<PaginacionModel>> Report(PaginacionCurso.Ejecuta data)
        {
            return await Mediator.Send(data);
        }
    }
}