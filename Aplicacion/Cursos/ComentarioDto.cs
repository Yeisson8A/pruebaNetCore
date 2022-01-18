using System;

namespace Aplicacion.Cursos
{
    public class ComentarioDto
    {
        //Se indica a las claves primarias que son de tipo Guid
        public Guid ComentarioId {get;set;}
        public string Alumno {get;set;}
        public int Puntaje {get;set;}
        public string ComentarioTexto {get;set;}
        public Guid CursoId {get;set;}
        //Con ? se indica al tipo DateTime que acepta nulos
        public DateTime? FechaCreacion {get;set;}
    }
}