using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace labti.Models
{
    public class Day
    {
        //Properties
        [Key]
        public int DayId { get; set; }
        [Required]
        public String DayName { get; set; }

        public int AsignaturaId { get; set; }

    }
}
