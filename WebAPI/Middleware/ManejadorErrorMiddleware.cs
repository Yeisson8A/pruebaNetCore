using System.Net;
using System;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WebAPI.Middleware
{
    public class ManejadorErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ManejadorErrorMiddleware> _logger;

        public ManejadorErrorMiddleware(RequestDelegate next, ILogger<ManejadorErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                //En caso de que no hayan errores en las validaciones se pasa el control al siguiente proceso
                await _next(context); 
            }
            catch (Exception ex)
            {
                //En caso de error en las validaciones se llama al manejador para registrar los errores
                await ManejadorExcepcionAsincrono(context, ex, _logger);
            }
        }

        private async Task ManejadorExcepcionAsincrono(HttpContext context, Exception ex, ILogger<ManejadorErrorMiddleware> logger)
        {
            object errores = null;

            switch (ex)
            {
                case ManejadorExcepcion me:
                    logger.LogError(ex, "Manejador Error");
                    //Adicionar errores
                    errores = me.Errores;
                    //Adicionar código estado a respuesta
                    context.Response.StatusCode = (int)me.Codigo;
                    break;
                case Exception e:
                    logger.LogError(ex, "Error de Servidor");
                    //Adicionar errores
                    //Se usa IsNullOrWhiteSpace para validar si es nulo
                    errores = String.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    //Adicionar código estado a respuesta
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            //Indicar que la respuesta es tipo JSON
            context.Response.ContentType = "application/json";

            //Validar si ocurrieron errores
            if (errores != null)
            {
                //Crear JSON con los errores
                var resultados = JsonConvert.SerializeObject(new {errores});
                //Escribir en respuesta el resultado con los errores
                await context.Response.WriteAsync(resultados);
            }
        }
    }
}