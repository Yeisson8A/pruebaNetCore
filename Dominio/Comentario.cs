using System;
namespace Dominio
{
    public class Comentario
    {
        //Se indica a las claves primarias que son de tipo Guid
        public Guid ComentarioId {get;set;}
        public string Alumno {get;set;}
        public int Puntaje {get;set;}
        public string ComentarioTexto {get;set;}
        public Guid CursoId {get;set;}
        //Con ? se indica que el tipo DateTime permite nulos
        public DateTime? FechaCreacion {get;set;}
        //Para referenciar a nivel de objetos Comentario-Curso
        public Curso Curso {get;set;}
    }
}