using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace labti.Models
{
    public class Asignatura
    {
        //Properties
        [Key]
        public int AsignaturaId { get; set; }

        [Required]
        [StringLength(60, ErrorMessage = "Especifique un nombre de asignatura.")]
        public String  Nombre{ get; set; }

        [Required]
        [Range(7,22,ErrorMessage = "La hora está fuera de intervalo.")]
        public int HoraInicio { get; set; }

        [Required]
        [Range(7,22,ErrorMessage = "La hora está fuera de intervalo.")]
        public int HoraFin { get; set; }

        public String Seccion { get; set; }

        public String Codigo { get; set; }

        [ForeignKey("ProfesorId")]
        public int ProfesorId { get; set; }
        public Profesor Profesor { get; set; }

        [ForeignKey("CursoId")]
        public int CursoId { get; set; }
        public Curso Curso { get; set; }
        
        //Navigation properties
        public List<Day> Days { get; set; }
    }
}
