using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio
{
    public class Precio
    {
        //Se indica las claves primarias que son de tipo Guid
        public Guid PrecioId {get;set;}
        //Se indica el formato que va a tener la columna en la base de datos
        [Column(TypeName = "decimal(18,4)")]
        public decimal PrecioActual {get;set;}
        //Se indica el formato que va a tener la columna en la base de datos
        [Column(TypeName = "decimal(18,4)")]
        public decimal Promocion {get;set;}
        public Guid CursoId {get;set;}
        //Para referenciar a nivel de objetos Precio-Curso
        public Curso Curso {get;set;}
    }
}