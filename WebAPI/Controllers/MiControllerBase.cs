using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //Hereda de ControllerBase para indicar que es un controlador
    public class MiControllerBase : ControllerBase
    {
        //Se inyecta IMediator para usar lógica de negocio del proyecto Aplicación
        private IMediator _mediator;
        
        //Se instancia el mediator
        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
    }
}