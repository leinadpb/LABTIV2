using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace labti.Models
{
    public class QueriesSolicitudes : Solicitud
    {
        public IEnumerable<Solicitud> Nuevas { get; set; }
        public IEnumerable<Solicitud> Aprobadas { get; set; }
        public IEnumerable<Solicitud> Denegadas { get; set; }

        public QueriesSolicitudes()
        {

        }
    }
}
