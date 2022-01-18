using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI.Controllers
{
    [ApiController]
    //En el endpoint será reemplazado por el nombre de la clase. Ejm: http://localhost:5000/WeatherForecast
    [Route("[controller]")]
    //Al indicar que una clase hereda de ControllerBase, inmediatamente la clase se convierte en un controlador
    public class WeatherForecastController : ControllerBase
    {
        //Inyección de dependencias,
        //Se va a cargar el context externamente, el cual se configuró en Startup como servicio
        //Crear un objeto indirectamente mediante un servicio externo dentro de una clase
        private readonly CursosOnlineContext context;
        public WeatherForecastController(CursosOnlineContext _context)
        {
            this.context = _context;
        }

        //Método GET
        [HttpGet]
        public IEnumerable<Curso> Get()
        {
            //Usamos el context para acceder a la entidad y a la base de datos
            return context.Curso.ToList();
        }
    }
}
