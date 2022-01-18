using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            //Vaidación por anotación, campo requerido
            //[Required(ErrorMessage = "Por favor ingrese el Titulo del curso")]
            public string Titulo {get;set;}
            public string Descripcion {get;set;}
            //Con ? en el tipo DateTime se indica que el campo acepta nulos
            public DateTime? FechaPublicacion {get;set;}
            //Listado con los GUIDs de los instructores a ingresar en la entidad CursoInstructor
            public List<Guid> ListaInstructor {get;set;}
            //Precios a asignar al curso en la entidad Precio
            public decimal Precio {get;set;}
            public decimal Promocion {get;set;}
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
                //Generar GUID para el Id de curso
                Guid _cursoId = Guid.NewGuid();

                //Objeto Curso a guardar en la base de datos
                var curso = new Curso{
                    CursoId = _cursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion,
                    FechaCreacion = DateTime.UtcNow
                };

                //Agregar objeto a entidad
                _context.Curso.Add(curso);

                //Validar que si exista un listado de instructores
                if (request.ListaInstructor != null)
                {
                    //Recorrer listado de instructores
                    foreach (var id in request.ListaInstructor)
                    {
                        //Asignar valores a objeto CursoInstructor a guardar en la base de datos
                        var cursoInstructor = new CursoInstructor{
                            CursoId = _cursoId,
                            InstructorId = id
                        };

                        //Agregar objeto a entidad
                        _context.CursoInstructor.Add(cursoInstructor);
                    }
                }

                //Crear objeto Precio a guardar en la base datos
                var precioEntidad = new Precio{
                    CursoId = _cursoId,
                    PrecioActual = request.Precio,
                    Promocion = request.Promocion,
                    PrecioId = Guid.NewGuid()
                };

                //Agregar objeto a entidad
                _context.Precio.Add(precioEntidad);

                //Guardar en la base de datos
                var valor = await _context.SaveChangesAsync();

                //Validar respuesta obtenida de la inserción
                if (valor > 0)
                {
                    return Unit.Value;
                }

                //Lanzar alerta en todo el programa en caso de error
                throw new Exception("No se pudo insertar el curso");
            }
        }
    }
}