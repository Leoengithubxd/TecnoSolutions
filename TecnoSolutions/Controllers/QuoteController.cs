using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TechnoSolutions.Dtos;
using TechnoSolutions.Repositories;
using TecnoSolutions.Dtos;
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
        
        public ActionResult ListQuotes()
        {
            var quotes = _quoteRepository.GetAllQuotes();
            return View(quotes);
        }
        //[HttpPost]
        //public ActionResult UpdateQuotes(List<QuoteDto> quoteDtos)
        //{
        //    if (quoteDtos == null || !quoteDtos.Any())
        //    {
        //        return Json(new { success = false, message = "No se recibieron datos para actualizar." });
        //    }

        //    try
        //    {
        //        _quoteRepository.UpdateQuotes(quoteDtos);
        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}
        [HttpPost]
        public ActionResult SaveQuotes(List<QuoteDto> quoteDtos)
        {
            if (quoteDtos == null || !quoteDtos.Any())
            {
                ViewBag.Message = "No se recibieron datos.";
                return View("ListQuotes", quoteDtos); // Redirige a la vista original con el mensaje
            }

            try
            {
                using (var db = new BD_14_02Entities()) // Asegúrate de usar el contexto correcto
                {
                    foreach (var quote in quoteDtos)
                    {
                        var existingQuote = db.QUOTE.FirstOrDefault(q => q.IdQuote == quote.IdQuote);
                        if (existingQuote != null)
                        {
                            existingQuote.IdCrewPerson = quote.IdCrew;
                            existingQuote.StarDate = quote.StarDate;
                            existingQuote.EndDate = quote.EndDate;
                            existingQuote.Price = quote.Price;
                        }
                    }
                    db.SaveChanges();
                }

                ViewBag.Message = "Cotizaciones actualizadas correctamente.";
                return RedirectToAction("AnalistCrud");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al actualizar: " + ex.Message;
                return View("Index", quoteDtos);
            }
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