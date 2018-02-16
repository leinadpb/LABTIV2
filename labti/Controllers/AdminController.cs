using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using labti.Data;
using labti.Models;
using labti.ViewModels;

namespace labti.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly String APROBADA = "APROBADA";
        private readonly String DENEGADA = "DENEGADA";
        private readonly String NUEVA = "NUEVA";

        public SolicitudesViewModel SViewModel;
        public AdminController(ApplicationDbContext ctx)
        {
            _context = ctx;
            SViewModel = new SolicitudesViewModel { };
            SViewModel.Errors = new ErrorMock { };
            SViewModel.QueriesSolicitudes = new QueriesSolicitudes { };
            SViewModel.Profesores = new List<Profesor>();
        }


        public IActionResult Solicitudes()
        {
           
            var nuevas = _context.Solicitudes
                        .OrderBy(s => Convert.ToDateTime(s.FechaSolicitud))
                        .Where(s => s.Estado == NUEVA)
                        .ToList();

            var aprobadas = _context.Solicitudes
                        .OrderBy(s => Convert.ToDateTime(s.FechaSolicitud))
                        .Where(s => s.Estado == APROBADA)
                        .ToList();
            var denegadas = _context.Solicitudes
                        .OrderBy(s => Convert.ToDateTime(s.FechaSolicitud))
                        .Where(s => s.Estado == DENEGADA)
                        .ToList();

            var profesores = _context.Profesores.OrderBy(p => p.FirstName).ThenBy(p => p.LastName);

            ViewBag.CantidadNuevas = nuevas.Count();
            ViewBag.CantidadAprobadas = aprobadas.Count();
            ViewBag.CantidadDenegadas = denegadas.Count();

            SViewModel.QueriesSolicitudes.Nuevas = nuevas;
            SViewModel.QueriesSolicitudes.Aprobadas = aprobadas;
            SViewModel.QueriesSolicitudes.Denegadas = denegadas;
            SViewModel.Profesores = profesores.ToList();

            return View(SViewModel);
        }

        [HttpPost]
        public IActionResult Approve(int? id, String CursoAsignado, String Notas, int ProfesorAsignado)
        {
            if(id != null)
            {
                var solicitud = _context.Solicitudes.Where(s => s.SolicitudId == id).FirstOrDefault();
                var curso = _context.Cursos.Where(c => c.CursoName == CursoAsignado).FirstOrDefault();

                DateTime fechaProyecto = Convert.ToDateTime(solicitud.FechaProyecto);
                String myDay = fechaProyecto.DayOfWeek.ToString().ToLower();
                String d = null;
                foreach (var day in _context.Days.ToList())
                {
                    if(myDay.Equals("monday")) // LUNES
                    {
                        d = "LUNES";
                    }else if (myDay.Equals("tuesday")){
                        d = "MARTES";
                    }else if (myDay.Equals("wednesday")){
                        d = "MIERCOLES";
                    }
                    else if (myDay.Equals("thursday")){
                        d = "JUEVES";
                    }
                    else if (myDay.Equals("friday")){
                        d = "VIERNES";
                    }
                    else if (myDay.Equals("saturday"))
                    {
                        d = "SABADO";
                    }
                }
                var queryDay = _context.Days.Where(dd => dd.DayName.Equals(d)).SingleOrDefault();

                //Crear asignatura
                _context.Asignaturas.Add(CreateSubject(solicitud, curso, ProfesorAsignado));

                //Cambiar estado de la solicitud
                solicitud.Estado = APROBADA;

                //Guardar el contexto 
                _context.SaveChanges();

                SViewModel.Errors.AddedSuccessfully = "LSe ha agregado la asignatura al calendario exitosamente.";
            }
            else
            {
                SViewModel.Errors.IdNotFound = "Ha ocurrido un error con la solicitud, por favor inténtelo de nuevo.";
            }

            return RedirectToAction("Solicitudes");
        }

        private Asignatura CreateSubject(String nombre, int horai, int horaf, String seccion, String codigo,
            Curso curso, int profesor)
        {
            var p = _context.Profesores.Find(profesor);
            Asignatura asignatura = new Asignatura {
                Nombre = nombre,
                HoraInicio = horai,
                HoraFin = horaf,
                Seccion = seccion,
                Codigo = codigo,
                Curso = curso,
                Profesor = p,
                CursoId = curso.CursoId,
                ProfesorId = profesor
            };
            return asignatura;
        }

        private Asignatura CreateSubject(Solicitud solicitud, Curso curso, int teacher)
        {
            var p = _context.Profesores.Find(teacher);
            Asignatura asignatura = new Asignatura
            {
                Nombre = solicitud.Asignatura,
                HoraInicio = solicitud.HoraInicio,
                HoraFin = solicitud.HoraFin,
                Seccion = solicitud.Seccion,
                Codigo = solicitud.Codigo,
                Curso = curso,
                Profesor = p,
                CursoId = curso.CursoId,
                ProfesorId = teacher
            };
            return asignatura;
        }

        [HttpPost]
        public IActionResult Deny(int? id)
        {
            var errors = new ErrorMock { };
            if (id != null)
            {
                var solicitud = _context.Solicitudes.Where(s => s.SolicitudId == id).SingleOrDefault();
                solicitud.Estado = DENEGADA;
                _context.SaveChanges();
                SViewModel.Errors.DeniedSuccessfully = "La solicitud ha sido denegada exitosamente.";
            }
            else
            {
                SViewModel.Errors.IdNotFound = "Ha ocurrido un error con la solicitud, por favor inténtelo de nuevo.";
            }
            return RedirectToAction("Solicitudes");
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            var errors = new ErrorMock { };
            if (id != null)
            {
                var solicitud = _context.Solicitudes.Where(s => s.SolicitudId == id).SingleOrDefault();
                _context.Remove(solicitud);
                _context.SaveChanges();
                SViewModel.Errors.DeletedSuccessfully = "La solicitud ha sido eliminada exitosamente.";
            }
            else
            {
                SViewModel.Errors.IdNotFound = "Ha ocurrido un error con la solicitud, por favor inténtelo de nuevo.";
            }
            return RedirectToAction("Solicitudes");
        }

        [HttpPost]
        public IActionResult AddAsignatura(String CourseName)
        {

            return View();
        }
    }
}