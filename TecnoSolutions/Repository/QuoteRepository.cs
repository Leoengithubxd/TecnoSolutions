using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnoSolutions.Dtos;
using TecnoSolutions.Models;
namespace TechnoSolutions.Repositories
{
    public class QuoteRepository
    {
        public List<ServiceSelectionDto> GetAllServices() //Traer tabla Service
        {
            using (var db = new BD_14_02Entities())
            {
                var services = db.SERVICE
                    .Select(p => new
                    {
                        p.IdService,
                        p.Name,
                    })
                    .AsEnumerable()
                    .Select(p => new ServiceSelectionDto
                    {
                        IdService = p.IdService,
                        Name = p.Name,
                    })
                    .ToList();

                return services;
            }
        }

        public void SaveSelectedServices(
            int userId, List<ServiceSelectionDto> selectedServices, 
            string address, string department, string city) //Cargar Cotizacion
        {
            using (var db = new BD_14_02Entities())
            {
                foreach (var service in selectedServices)
                {
                    var quote = new QUOTE
                    {
                        IdPerson = userId,
                        IdState = 1,
                        ServiceAddress = address,
                        ServiceDepartment = department,
                        ServiceCity = city,
                    };
                    db.QUOTE.Add(quote);
                    db.SaveChanges();

                    var quoteService = new QUOTE_SERVICE
                    {
                        IdQuote = quote.IdQuote,
                        IdService = service.IdService,
                    };
                    db.QUOTE_SERVICE.Add(quoteService);
                }
                db.SaveChanges();
            }
        }
    }
}