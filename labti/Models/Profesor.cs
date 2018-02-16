using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace labti.Models
{
    public class Profesor
    {
        //Properties
        [Key]
        public int ProfesorId { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Especifique un nombre del profesor.")]
        public String FirstName { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Especifique un apellido del profesor.")]
        public String LastName { get; set; }

        //Navigation properties
        public List<Asignatura> Asignaturas { get; set; }

    }
}
