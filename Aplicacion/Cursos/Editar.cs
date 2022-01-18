using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Editar
    {
        public class Ejecuta : IRequest
        {
            public Guid CursoId {get;set;}
            public string Titulo {get;set;}
            public string Descripcion {get;set;}
            //Para los datetime con ? se indica que aceptan nulos
            public DateTime? FechaPublicacion {get;set;}
            //Listado con los GUIDs de los instructores a ingresar en la entidad CursoInstructor
            public List<Guid> ListaInstructor {get;set;}
            //Precios a asignar al curso en la entidad Precio
            //Para los decimal con ? se indica que aceptan nulos
            public decimal? Precio {get;set;}
            public decimal? Promocion {get;set;}
        }

        //Clase para las validaciones con Fluent
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            //Constructor donde se definen las validaciones a aplicar
            public EjecutaValidacion()
            {
                //Regla a aplicar a campo Titulo
                RuleFor(x => x.Titulo).NotEmpty();
                //Regla a aplicar a campo Descripción
                RuleFor(x => x.Descripcion).NotEmpty();
                //Regla a aplicar a campo Fecha Publicación
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;

            //Inyectar context a través del constructor de la clase,
            //de forma que se pueda acceder a la base de datos
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Buscar curso que se va a actualizar
                var curso = await _context.Curso.FindAsync(request.CursoId);

                //Validar si el curso a actualizar existe
                if (curso == null)
                {
                    //throw new Exception("El curso no existe");
                    //Se lanza excepción usando la clase creada para el manejo de errores,
                    //la cual se utiliza en WebAPI como Middleware
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "No se encontro el curso"});
                }

                //Actualizar objeto con la información enviada
                //Con ?? se valida si no se envió valor en el request se mantiene el valor original
                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;
                curso.FechaCreacion = DateTime.UtcNow;

                //Obtener la información de la entidad Precio asignada a dicho curso
                var precioEntidad = _context.Precio.Where(x => x.CursoId == request.CursoId).FirstOrDefault();

                //Validar si se encontró información
                if (precioEntidad != null)
                {
                    //Actualizar objeto con la información enviada
                    //Con ?? se valida si no se envió valor en el request se mantiene el valor original
                    precioEntidad.PrecioActual = request.Precio ?? precioEntidad.PrecioActual;
                    precioEntidad.Promocion = request.Promocion ?? precioEntidad.Promocion;
                }
                else
                {
                    //En caso de que el curso no tenga precio asociado se crea un objeto Precio
                    precioEntidad = new Precio{
                        PrecioId = Guid.NewGuid(),
                        PrecioActual = request.Precio ?? 0, //En caso de no indicar valor se coloca por defecto 0
                        Promocion = request.Promocion ?? 0, //En caso de no indicar valor se coloca por defecto 0
                        CursoId = request.CursoId
                    };

                    //Agregar objeto Precio a la base datos
                    _context.Precio.Add(precioEntidad);
                }

                //Validar si se indicó una lista de instructores
                if (request.ListaInstructor != null)
                {
                    if (request.ListaInstructor.Count > 0)
                    {
                        //Eliminar los instructores actuales del curso de la base datos
                        var instructoresBD = _context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();
                        //Recorrer listado de instructores para dicho curso
                        foreach (var instructorEliminar in instructoresBD)
                        {
                            //Eliminar instructor de la entidad CursoInstructor
                            _context.CursoInstructor.Remove(instructorEliminar);
                        }

                        //Adicionar los nuevos instructores del curso
                        foreach (var id in request.ListaInstructor)
                        {
                            //Crear objeto con el nuevo instructor para dicho curso
                            var nuevoInstructor = new CursoInstructor{
                                CursoId = request.CursoId,
                                InstructorId = id
                            };

                            //Agregar objeto a la entidad CursoInstructor a almacenar en la base datos
                            _context.CursoInstructor.Add(nuevoInstructor);
                        }
                    }
                }

                //Guardar en la base de datos
                var resultado = await _context.SaveChangesAsync();

                //Validar respuesta obtenida de la actualización
                if(resultado > 0)
                {
                    return Unit.Value;
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se guardaron los cambios en el curso");
            }
        }
    }
}