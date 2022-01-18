using Dominio;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistencia
{
    public class CursosOnlineContext : IdentityDbContext<Usuario>
    {
        //Con base se indica que herede de DbContext
        public CursosOnlineContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Crear archivo de migración con toda la lógica para creación de tablas en la base de datos
            base.OnModelCreating(modelBuilder);

            //Para indicar que la entidad CursoInstructor tiene dos claves primarias
            modelBuilder.Entity<CursoInstructor>().HasKey(ci => new { ci.InstructorId, ci.CursoId });
        }

        //Crear entidades a partir de las clases
        public DbSet<Curso> Curso {get;set;}
        public DbSet<Instructor> Instructor {get;set;}
        public DbSet<Precio> Precio {get;set;}
        public DbSet<Comentario> Comentario {get;set;}
        public DbSet<CursoInstructor> CursoInstructor {get;set;}
    }
}