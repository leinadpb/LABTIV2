using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using labti.Models;
using labti.Data;
using Microsoft.EntityFrameworkCore;
using labti.ViewModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace labti.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        private HorarioViewModel HViewModel;
        

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            HViewModel = new HorarioViewModel { };
            HViewModel.Profesores = new List<Profesor>();
            HViewModel.Asignaturas = new List<Asignatura>();
            //HViewModel.SubjectAdded = "";
        }


        public IActionResult Index()
        {

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Schedules()
        {

            ViewBag.Search = false;
            if(HViewModel.SubjectAdded != null)
            {
                if (!HViewModel.SubjectAdded.Equals(""))
                    ViewBag.Info = HViewModel.SubjectAdded;
            }
            
            return View(HViewModel);
        }

        [Authorize]
        public IActionResult TestSchedule(string selectedRoom) //Testing controller
        {
            ViewBag.Search = true;
            ViewBag.Room = selectedRoom;
            var query2 = _context.Asignaturas.Where(a => a.Curso.CursoName.Equals(selectedRoom)).OrderBy(a => a.HoraInicio)
                        .Join(_context.Profesores, a => a.ProfesorId, p => p.ProfesorId, (a, p) => a);
            HViewModel.Asignaturas = query2.ToList();
            string json = JsonConvert.SerializeObject(query2.ToList());
            ViewBag.Asigs = json;
            return View(HViewModel);
        }

        [HttpPost]
        public IActionResult Schedules(String selectedRoom)
        {

            ViewBag.Search = true;
            ViewBag.Room = selectedRoom;
            var query2 = _context.Asignaturas.Where(a => a.Curso.CursoName.Equals(selectedRoom)).OrderBy(a => a.HoraInicio)
                        .Join(_context.Profesores, a => a.ProfesorId, p => p.ProfesorId, (a, p) => a);
            HViewModel.Asignaturas = query2.ToList();
            
            string json = JsonConvert.SerializeObject(query2.ToList());
            ViewBag.Asigs = json;
            HViewModel.Profesores = _context.Profesores.OrderBy(p => p.FirstName).ThenBy(p => p.LastName).ToList();
            ViewBag.TotalAsigs = query2.ToList().Count();
            return View(HViewModel);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddAsignatura(String Nombre, int HoraInicio, int HoraFin, String Seccion, String Codigo,
            String CourseName, int Profesor, bool isLunes, bool isMartes, bool isMiercoles, bool isJueves, bool isViernes,
            bool isSabado)
        {
            Curso curso = _context.Cursos.Where(c => c.CursoName.Equals(CourseName)).FirstOrDefault();
            String info = "";
            Asignatura asignatura = CreateSubject(Nombre, HoraInicio, HoraFin, Seccion, Codigo, curso, Profesor, isLunes, isMartes, isMiercoles, isJueves, isViernes, isSabado);

            if (!existsAsignatura(asignatura, curso))
            {
                _context.Add(asignatura);

                _context.SaveChanges();
                HViewModel.SubjectAdded = "Asignatura agregada exitosamente.";
            }
            else
            {
                HViewModel.SubjectAdded = "La asignatura provoca un choque con otra ya existente.";
            }

            //return RedirectToAction("Schedules", new HorarioViewModel { SubjectAdded = info});
            return View("Schedules", HViewModel);
        }

        private bool existsAsignatura(Asignatura toAdd, Curso curso)
        {
            bool exists = false;
            List<Asignatura> asigs = _context.Asignaturas.Where(asg => asg.CursoId == curso.CursoId).ToList();
            foreach(Asignatura old in asigs)
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

        [Authorize]
        private Asignatura CreateSubject(String Nombre, int HoraInicio, int HoraFin, String Seccion, String Codigo,
            Curso Curso, int Profesor, bool lu, bool ma, bool mi, bool ju, bool vi, bool sa)
        {
            var professor = _context.Profesores.Find(Profesor);
            Asignatura asignatura = new Asignatura
            {
                Nombre = Nombre,
                HoraInicio = HoraInicio,
                HoraFin = HoraFin,
                Seccion = Seccion,
                Codigo = Codigo,
                Curso = Curso,
                Profesor = professor,
                CursoId = Curso.CursoId,
                ProfesorId = professor.ProfesorId,
                isLunes = lu,
                isMartes = ma,
                isMiercoles = mi,
                isJueves = ju,
                isViernes = vi,
                isSabado = sa
            };
            
            return asignatura;
        }

        public IActionResult RoomRequest()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RoomRequest(Solicitud solicitud)
        {
            //Verify user is a Professor at INTEC by it's email.
            if (ModelState.IsValid)
            {
                if (IsTeacherAtINTEC(solicitud.ProfesorEmail))
                {
                    if (solicitud.HoraFin > solicitud.HoraInicio)
                    {
                        //solicitud.FechaSolicitud = DateTime.Now.ToShortDateString();
                        _context.Add(solicitud);
                        await _context.SaveChangesAsync();
                        ViewBag.Success = true;
                        ViewBag.SuccessData = "La solicitud ha sido enviada con éxito.";
                        return View();
                    }
                    else
                    {
                        ViewBag.TimeError = "La hora final debe ser mayor que la inicial.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Success = false;
                    ViewBag.Error = true;
                    ViewBag.ErrorData = "Usted no está autorizado para hacer esta solicitud.";
                    return View();
                }
  
            }
            else
            {
                ViewBag.Success = false;
                ViewBag.Error = true;
                ViewBag.ErrorData = "Ha ocurrido un problema con la solicitud. Por favor, inténtelo de nuevo.";
                return View(solicitud);
            }      
        }

        private bool IsTeacherAtINTEC(string profesorEmail)
        {
            return profesorEmail.Substring(profesorEmail.IndexOf('@')).ToLower() == "@intec.edu.do";
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
