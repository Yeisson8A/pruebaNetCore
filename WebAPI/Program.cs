using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostserver = CreateHostBuilder(args).Build();

            //Código para ejecutar archivo de migración
            using (var ambiente = hostserver.Services.CreateScope())
            {
                 var services = ambiente.ServiceProvider;
                 
                 try
                 {
                    //Invocar al UserManager necesario para insertar un usuario de prueba
                    var userManager = services.GetRequiredService<UserManager<Usuario>>();
                    //Invocar al Contexto del proyecto Persistencia para ejecutar la migración
                    var context = services.GetRequiredService<CursosOnlineContext>();
                    //Ejecutar migración
                    context.Database.Migrate();
                    //Llamar clase DataPrueba para insertar un usuario de prueba
                    //Se usa el método Wait dado que InsertarData es asincrono y no se puede indicar que el Main es asincrono
                    DataPrueba.InsertarData(context, userManager).Wait();
                 }
                 catch (Exception e)
                 {
                     //Invocar al Ilogger para indicar que ocurrió un error
                     var logging = services.GetRequiredService<ILogger<Program>>();
                     logging.LogError(e, "Ocurrio un error en la migración");
                 }
            }

            hostserver.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
