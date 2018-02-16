using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace labti.Models
{
    public class Tecnico
    {
        //Properties
        [Key]
        public int TecnicoId { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Nombre muy largo.")]
        public String FirstName { get; set; }
        [Required]
        [StringLength(30, ErrorMessage = "Apellido muy largo.")]
        public String LastName { get; set; }
        [StringLength(50, ErrorMessage = "Valor muy largo.")]
        public String Puesto { get; set; }
        [StringLength(80, ErrorMessage = "Valor mu largo.")]
        public String Departamento { get; set; }

        public int SolicitudId { get; set; }

        //Navigation properties
        public List<Solicitud> Solicitudes { get; set; }

    }
}
