using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace labti.Models
{
    public class DayAsignatura
    {
        public int DayId { get; set; }
        public Day Day { get; set; }

        public int AsignaturaId { get; set; }
        public Asignatura Asignatura { get; set; }

    }
}
