using System;
namespace Dominio
{
    public class CursoInstructor
    {
        //Se indica a las claves primarias que son de tipo Guid
        public Guid CursoId {get;set;}
        public Guid InstructorId {get;set;}
        //Para referenciar a nivel de objetos CursoInstructor-Curso
        public Curso Curso {get;set;}
        //Para referencia a nivel de objetos CursoInstructor-Instructor
        public Instructor Instructor {get;set;}
    }
}