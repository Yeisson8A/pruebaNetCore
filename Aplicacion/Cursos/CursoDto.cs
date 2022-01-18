using System.Collections;
using System;
using System.Collections.Generic;

namespace Aplicacion.Cursos
{
    public class CursoDto
    {
        //Se indica a las claves primarias que son de tipo Guid
        public Guid CursoId {get;set;}
        public string Titulo {get;set;}
        public string Descripcion {get;set;}
        //Con ? se indica al tipo DateTime que acepta nulos
        public DateTime? FechaPublicacion {get;set;}
        public byte[] FotoPortada {get;set;}
        //Con ? se indica al tipo DateTime que acepta nulos
        public DateTime? FechaCreacion {get;set;}
        //Listado de los instructores que tiene asignado dicho curso
        public ICollection<InstructorDto> Instructores {get;set;}
        //Precio asignado a dicho curso
        public PrecioDto Precio {get;set;}
        //Listado de los comentarios que tiene asignado dicho curso
        public ICollection<ComentarioDto> Comentarios {get;set;}
    }
}