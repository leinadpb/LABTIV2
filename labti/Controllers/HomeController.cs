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
            HViewModel.SubjectAdded = "";
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
            return View(HViewModel);
        }

        [HttpGet]
        public IActionResult Schedules(String selectedRoom)
        {

            ViewBag.Search = true;
            ViewBag.Room = selectedRoom;
            var query = _context.Profesores.OrderBy(p => p.FirstName).ThenBy(p => p.LastName);
           //var query2 = _context.Asignaturas.OrderBy(a => a.D)
            HViewModel.Profesores = query.ToList();
            
            return View(HViewModel);
        }

        [HttpPost]
        public IActionResult AddAsignatura(String Nombre, int HoraInicio, int HoraFin, String Seccion, String Codigo,
            String CourseName, int Profesor, bool isLunes, bool isMartes, bool isMiercoles, bool isJueves, bool isViernes,
            bool isSabado)
        {
            Curso curso = _context.Cursos.Where(c => c.CursoName.Equals(CourseName)).FirstOrDefault();

            Asignatura asignatura = CreateSubject(Nombre, HoraInicio, HoraFin, Seccion, Codigo, curso, Profesor, isLunes, isMartes, isMiercoles, isJueves, isViernes, isSabado);

            _context.Add(asignatura);

            _context.SaveChanges();
            HViewModel.SubjectAdded = "Asignatura agregada exitosamente.";
            return RedirectToAction("Schedules");
        }

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
