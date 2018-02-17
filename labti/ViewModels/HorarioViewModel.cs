using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using labti.Models;

namespace labti.ViewModels
{
    public class HorarioViewModel
    {
        public List<Profesor> Profesores { get; set; }
        public String SubjectAdded { get; set; }
        public List<Asignatura> Asignaturas { get; set; }
    }
}
