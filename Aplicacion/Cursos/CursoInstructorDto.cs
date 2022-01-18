using System;

namespace Aplicacion.Cursos
{
    public class CursoInstructorDto
    {
        //Se indica a las claves primarias que son de tipo Guid
        public Guid CursoId {get;set;}
        public Guid InstructorId {get;set;}
    }
}