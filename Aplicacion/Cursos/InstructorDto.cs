using System;

namespace Aplicacion.Cursos
{
    public class InstructorDto
    {
        //Se indica a las claves primarias que son de tipo Guid
        public Guid InstructorId {get;set;}
        public string Nombre {get;set;}
        public string Apellidos {get;set;}
        public string Grado {get;set;}
        public byte[] FotoPerfil {get;set;}
        //Con ? se indica al tipo DateTime que acepta nulos
        public DateTime? FechaCreacion {get; set;}
    }
}