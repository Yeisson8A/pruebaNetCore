using System.Collections.Immutable;
using System.Text;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Cursos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;
using FluentValidation.AspNetCore;
using WebAPI.Middleware;
using Dominio;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using Aplicacion.Contratos;
using Seguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;
using Persistencia.DapperConexion;
using Persistencia.DapperConexion.Instructor;
using Microsoft.OpenApi.Models;
using Persistencia.DapperConexion.Paginacion;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Se agrega Cors como servicio a fin de poder consumir los webservices desde cualquier cliente
            services.AddCors(o => o.AddPolicy("corsApp", builder => {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            }));

            //Se agrega DbContext de proyecto Persistencia como servicio de proyecto WebAPI
            services.AddDbContext<CursosOnlineContext>(opt => {
                //Se usa cadena de conexión para conectarse a la base de datos
                //Se usa la variable Configuration que hace referencia al archivo appsettings.json,
                //y se obtiene la cadena de conexión
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            //Se agrega Dapper como servicio a fin de consumir procedimientos almacenados, 
            //indicandole la cadena de conexión para conectarse a la base de datos
            services.AddOptions();
            services.Configure<ConexionConfiguracion>(Configuration.GetSection("ConnectionStrings"));
            //Se agregar Manejador del proyecto Aplicación como servicio de proyecto WebAPI
            services.AddMediatR(typeof(Consulta.Manejador).Assembly);
            //Agregar validación con Fluent pasando la instancia de la clase que se va a validar
            //Agregar a los controladores la autorización antes de recibir una petición
            services.AddControllers(opt => {
                //Regla usuario debe estar autenticado
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>());
            //Se agrega IdentityCore como servicio y se indica que es el manejador del login de los usuarios
            var builder = services.AddIdentityCore<Usuario>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            //Se agrega IdentityRole como servicio a fin de manejar los roles y también al token
            identityBuilder.AddRoles<IdentityRole>();
            identityBuilder.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Usuario, IdentityRole>>();

            identityBuilder.AddEntityFrameworkStores<CursosOnlineContext>();
            identityBuilder.AddSignInManager<SignInManager<Usuario>>();
            services.TryAddSingleton<ISystemClock, SystemClock>();

            //Llave para desencriptar el token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            //Agregar autenticación para que no se pueda acceder a los controlares si no hay token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

            //Se agrega la interfaz para generar JWT y la clase que lo implementa como servicio
            services.AddScoped<IJwtGenerador, JwtGenerador>();
            //Se agrega la interfaz para obtener el Username actual del usuario en sesión como servicio
            services.AddScoped<IUsuarioSesion, UsuarioSesion>();
            //Se agrega la interfaz Mapper para mapear de Entity Framework a DTO como servicio
            services.AddAutoMapper(typeof(Consulta.Manejador));
            //Se agrega la interfaz para el manejo de la conexión a base datos a utilizar por Dapper como servicio
            services.AddTransient<IFactoryConnection, FactoryConnection>();
            //Se agrega la interfaz para las operaciones del instructor a ejecutar con Dapper como servicio
            services.AddScoped<IInstructor, InstructorRepositorio>();
            //Se agrega la interfaz para la paginación utilizando procedimientos y Dapper como servicio
            services.AddScoped<IPaginacion, PaginacionRepositorio>();
            //Se agrega Swagger como servicio a fin de documentar los endpoints del proyecto WebAPI
            //Se usa CustomSchemaIds(c => c.FullName) a fin de que Swagger tome el nombre completo de la clase 
            //incluyendo el namespace
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo{
                    Title = "Servicios para el mantenimiento de cursos",
                    Version = "v1"
                });
                c.CustomSchemaIds(c => c.FullName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Usar los Cors con la politica definida
            app.UseCors("corsApp");

            //Usar clase para manejo de errores como middleware
            app.UseMiddleware<ManejadorErrorMiddleware>();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            //Sólo para ambientes productivos
            //app.UseHttpsRedirection();
            //Indicar a la aplicación para que use la autenticación
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //Indicar que use Swagger
            app.UseSwagger();
            //Indicar que habilite la interfaz gráfica de Swagger a fin de visualizar la documentación
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cursos Online v1");
            });
        }
    }
}
