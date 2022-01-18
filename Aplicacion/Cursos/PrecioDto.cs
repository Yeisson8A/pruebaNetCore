using System;

namespace Aplicacion.Cursos
{
    public class PrecioDto
    {
        //Se indica las claves primarias que son de tipo Guid
        public Guid PrecioId {get;set;}
        public decimal PrecioActual {get;set;}
        public decimal Promocion {get;set;}
        public Guid CursoId {get;set;}
    }
}