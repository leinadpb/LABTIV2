using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace labti.Models
{
    public class Solicitud
    {
        //Properties
        [Key]
        public int SolicitudId { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "Período intertrimestral no válido.")]
        public String Trimestre { get; set; }

        [Required]
        public String FechaSolicitud { get; set; }

        [Required]
        public string NumeroSolicitud { get; set; }

        [Required(ErrorMessage = "Indique su nombre.")]
        public String Profesor { get; set; }

        [Required(ErrorMessage = "Indique su dirección de email.")]
        public String ProfesorEmail { get; set; }

        [Required(ErrorMessage = "Especifique la asignatura.")]
        public String Asignatura { get; set; }

        [Required(ErrorMessage = "Especificque la dirección.")]
        [StringLength(600, ErrorMessage = "La descripción es muy larga.")]
        public String Descripcion { get; set; }

        [Required(ErrorMessage = "Indique la fecha del proyecto.")]
        public String FechaProyecto { get; set; }

        [Required(ErrorMessage = "Indique la cantidad de estudiantes.")]
        [Range(0,40)]
        public int CantEstudiantes { get; set; }

        [Required(ErrorMessage = "Especifique los recursos a utilizar.")]
        [StringLength(400, ErrorMessage = "El texto de recursos es muy largo.")]
        public String Recursos { get; set; }

        [Required]
        [Range(7,22, ErrorMessage = "La hora de incio es inválida.")]
        public int HoraInicio { get; set; }

        [Required]
        [Range(7,22, ErrorMessage = "La hora de fin es inválida.")]
        public int HoraFin { get; set; }

        public String Estado { get; set; } /* NUEVA, APROBADA o RECHAZADA */

        public String Seccion { get; set; }
        public String Codigo { get; set; }

        public Tecnico Tecnico { get; set; }
        
        public String Notas { get; set; }

        

    }
}
