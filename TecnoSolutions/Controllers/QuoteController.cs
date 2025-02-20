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
        public ActionResult ApproveQuote()
        {
            using (var db = new BD_14_02Entities())
            {
                var quotes = db.QUOTE
                    .Select(p => new
                    {
                        p.IdQuote,
                        p.IdState,
                        p.Price
                    })
                    .AsEnumerable()
                    .Select(p => new QuoteDto
                    {
                        IdQuote = p.IdQuote,
                        IdState = p.IdState,
                        Price = Convert.ToDecimal(p.Price)
                    })
                    .ToList();

                return View(quotes);
            }
        }


        public ActionResult QuoteDetails(int id)
{
    using (var db = new BD_14_02Entities())
    {
        var quote = db.QUOTE
            .Where(p => p.IdQuote == id)
            .Select(p => new
            {
                p.IdQuote,
                p.IdPerson,
                p.IdState,
                p.ServiceAddress,
                p.ServiceDepartment,
                p.ServiceCity,
                p.StarDate,
                p.EndDate,
                p.Price
            })
            .AsEnumerable()
            .Select(p => new QuoteDto
            {
                IdQuote = p.IdQuote,
                IdPerson = p.IdPerson,
                IdState = p.IdState,
                ServiceAddress = p.ServiceAddress,
                ServiceDepartment = p.ServiceDepartment,
                ServiceCity = p.ServiceCity,
                StarDate = p.StarDate,
                EndDate = p.EndDate,
                Price = p.Price.HasValue ? Convert.ToDecimal(p.Price) : 0
            })
            .FirstOrDefault();

        if (quote == null)
        {
            return HttpNotFound();
        }

        return View(quote);
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