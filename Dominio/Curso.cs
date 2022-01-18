using System.Collections;
using System.Collections.Generic;
using System;

namespace Dominio
{
    public class Curso
    {
        //Se indica a las claves primarias que son de tipo Guid
        public Guid CursoId {get;set;}
        public string Titulo {get;set;}
        public string Descripcion {get;set;}
        //Con ? se indica al tipo DateTime que acepta nulos
        public DateTime? FechaPublicacion {get;set;}
        public byte[] FotoPortada {get;set;}
        //Con ? se indica que el tipo DateTime permite nulos
        public DateTime? FechaCreacion {get;set;}
        //Para referenciar a nivel de objetos Curso-Precio (Uno a Uno)
        public Precio PrecioPromocion {get;set;}
        //Para referencia a nivel de objetos Curso-Comentario (Uno a muchos)
        public ICollection<Comentario> ComentarioLista {get;set;}
        //Para referencia a nivel de objetos Curso-CursoInstructor (Muchos a Muchos)
        public ICollection<CursoInstructor> InstructoresLink {get;set;}
    }
}