using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using labti.Models;

namespace labti.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<Asignatura> Asignaturas { get; set; }
        public DbSet<Profesor> Profesores { get; set; }
        public DbSet<Tecnico> Tecnicos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Solicitud - Tecnico

            //Solicitud - Curso

            //Asignatura - Dia

            //Asignatura - Profesor

            // Dia - Asignatura

            base.OnModelCreating(builder);
        }
    }
}
