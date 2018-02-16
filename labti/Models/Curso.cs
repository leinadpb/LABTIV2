using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace labti.Models
{
    public class Curso
    {
        //Properties
        [Key]
        public int CursoId { get; set; }
        [Required]
        public String CursoName { get; set; }
    }
}
