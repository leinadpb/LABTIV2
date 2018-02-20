using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using labti.Data;
using labti.Models;
using labti.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace labti.Controllers
{
    [Authorize]
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
            SViewModel.Errors.AddedSuccessfully = "";
            SViewModel.Errors.DeletedSuccessfully = "";
            SViewModel.Errors.DeniedSuccessfully = "";
            SViewModel.QueriesSolicitudes = new QueriesSolicitudes { };
            SViewModel.Profesores = new List<Profesor>();
        }

        [Authorize]
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
        [Authorize]
        public IActionResult Approve(int? id, String CursoAsignado, String Notas, int ProfesorAsignado)
        {
            if (id != null)
            {
                var solicitud = _context.Solicitudes.Where(s => s.SolicitudId == id).FirstOrDefault();
                var curso = _context.Cursos.Where(c => c.CursoName == CursoAsignado).FirstOrDefault();

                DateTime fechaProyecto = Convert.ToDateTime(solicitud.FechaProyecto);
                String myDay = fechaProyecto.DayOfWeek.ToString().ToLower();

                bool lu = false; bool ma = false; bool mi = false; bool ju = false; bool vi = false; bool sa = false;

                foreach (var day in _context.Days.ToList())
                {
                    if (myDay.Equals("monday")) // LUNES
                    {
                        lu = true;
                    }
                    else if (myDay.Equals("tuesday"))
                    {
                        ma = true;
                    }
                    else if (myDay.Equals("wednesday"))
                    {
                        mi = true;
                    }
                    else if (myDay.Equals("thursday"))
                    {
                        ju = true;
                    }
                    else if (myDay.Equals("friday"))
                    {
                        vi = true;
                    }
                    else if (myDay.Equals("saturday"))
                    {
                        sa = true;
                    }
                }

                //Crear asignatura
                var asg = CreateSubject(solicitud, curso, ProfesorAsignado, lu, ma, mi, ju, vi, sa);

                if(!existsAsignatura(asg, curso))
                {
                    _context.Asignaturas.Add(asg);

                    //Cambiar estado de la solicitud
                    solicitud.Estado = APROBADA;

                    //Guardar el contexto 
                    _context.SaveChanges();

                    SViewModel.Errors.AddedSuccessfully = "Se ha agregado la asignatura al calendario exitosamente.";
                }
                else
                {
                    SViewModel.Errors.AddedSuccessfully = "La asignatura provoca un choque con otra ya existente.";
                }
                
            }
            else
            {
                SViewModel.Errors.IdNotFound = "Ha ocurrido un error con la solicitud, por favor inténtelo de nuevo.";
            }
            getSolicitudes();

            ViewBag.CantidadNuevas = SViewModel.QueriesSolicitudes.Nuevas.Count();
            ViewBag.CantidadAprobadas = SViewModel.QueriesSolicitudes.Aprobadas.Count();
            ViewBag.CantidadDenegadas = SViewModel.QueriesSolicitudes.Denegadas.Count();
            return View("Solicitudes", SViewModel);
        }

        private void getSolicitudes()
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

            SViewModel.QueriesSolicitudes.Nuevas = nuevas;
            SViewModel.QueriesSolicitudes.Aprobadas = aprobadas;
            SViewModel.QueriesSolicitudes.Denegadas = denegadas;
            SViewModel.Profesores = profesores.ToList();
        }

        private Asignatura CreateSubject(String nombre, int horai, int horaf, String seccion, String codigo,
            Curso curso, int profesor, bool lu, bool ma, bool mi, bool ju, bool vi, bool sa)
        {
            var p = _context.Profesores.Find(profesor);
            Asignatura asignatura = new Asignatura
            {
                Nombre = nombre,
                HoraInicio = horai,
                HoraFin = horaf,
                Seccion = seccion,
                Codigo = codigo,
                Curso = curso,
                Profesor = p,
                CursoId = curso.CursoId,
                ProfesorId = profesor,
                isLunes = lu,
                isMartes = ma,
                isMiercoles = mi,
                isJueves = ju,
                isViernes = vi,
                isSabado = sa
            };
            return asignatura;
        }

        private Asignatura CreateSubject(Solicitud solicitud, Curso curso, int teacher, bool lu, bool ma, bool mi, bool ju, bool vi, bool sa)
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
                ProfesorId = teacher,
                isLunes = lu,
                isMartes = ma,
                isMiercoles = mi,
                isJueves = ju,
                isViernes = vi,
                isSabado = sa
            };
            return asignatura;
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public IActionResult AddAsignatura(String CourseName)
        {

            return View();
        }

        private bool existsAsignatura(Asignatura toAdd, Curso curso)
        {
            bool exists = false;
            List<Asignatura> asigs = _context.Asignaturas.Where(asg => asg.CursoId == curso.CursoId).ToList();
            foreach (Asignatura old in asigs)
            {
                if (toAdd.isLunes)
                {
                    if ((toAdd.HoraInicio >= old.HoraInicio && toAdd.HoraInicio < old.HoraFin) && old.isLunes) // choca
                    {
                        exists = true;
                        break;
                    }
                }
                if (toAdd.isMartes)
                {
                    if ((toAdd.HoraInicio >= old.HoraInicio && toAdd.HoraInicio < old.HoraFin) && old.isMartes) // choca
                    {
                        exists = true;
                        break;
                    }
                }
                if (toAdd.isMiercoles)
                {
                    if ((toAdd.HoraInicio >= old.HoraInicio && toAdd.HoraInicio < old.HoraFin) && old.isMiercoles) // choca
                    {
                        exists = true;
                        break;
                    }

                }
                if (toAdd.isJueves)
                {
                    if ((toAdd.HoraInicio >= old.HoraInicio && toAdd.HoraInicio < old.HoraFin) && old.isJueves) // choca
                    {
                        exists = true;
                        break;
                    }
                }
                if (toAdd.isViernes)
                {
                    if ((toAdd.HoraInicio >= old.HoraInicio && toAdd.HoraInicio < old.HoraFin) && old.isViernes) // choca
                    {
                        exists = true;
                        break;
                    }
                }
                if (toAdd.isSabado)
                {
                    if ((toAdd.HoraInicio >= old.HoraInicio && toAdd.HoraInicio < old.HoraFin) && old.isSabado) // choca
                    {
                        exists = true;
                        break;
                    }
                }

            }
            return exists;
        }
    }
}