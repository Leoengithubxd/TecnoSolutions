using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TechnoSolutions.Dtos;
using TechnoSolutions.Repositories;
using TecnoSolutions.Models;

namespace TechnoSolutions.Controllers
{
    public class QuoteController : Controller
    {
        private readonly QuoteRepository _quoteRepository;

        public QuoteController()
        {
            _quoteRepository = new QuoteRepository();
        }

        public ActionResult SelectServices() //Cargar servicios
        {
            var services = _quoteRepository.GetAllServices();
            return View(services);
        }
        public ActionResult RequestQuotation()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SelectServices(List<ServiceSelectionDto> selectedServices, 
            string address, string department, string city) // Cargar solicitud cotizacion
        {
            var userId = (Session["IdUser"] != null) ? (int)Session["IdUser"] : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

            var servicesToSave = selectedServices?.Where(s => s.IsSelected).ToList();

            if (servicesToSave == null || !servicesToSave.Any())
            {
                TempData["Message"] = "Debe seleccionar al menos un servicio.";
                return RedirectToAction("SelectServices");
            }

            _quoteRepository.SaveSelectedServices(userId, servicesToSave, address, department, city);

            return RedirectToAction("RequestQuotation", "Quote");
        }
    }
}