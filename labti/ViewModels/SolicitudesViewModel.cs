using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using labti.Models;

namespace labti.ViewModels
{
    public class SolicitudesViewModel
    {
        public ErrorMock Errors { get; set; }
        public QueriesSolicitudes QueriesSolicitudes { get; set; }
        public List<Profesor> Profesores { get; set; }
    }
}
