using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechnoSolutions.Dtos;
using TecnoSolutions.Dtos;
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
            string address, string department, string city) // Crear Cotización
        {
            using (var db = new BD_14_02Entities())
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
                foreach (var service in selectedServices)
                {
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

        public QuoteDto QuoteDetail(int userId, int? quoteId) //Detalle Cotización
        {
            using (var db = new BD_14_02Entities())
            {
                var quote = (from q in db.QUOTE
                             join p in db.PERSON on q.IdPerson equals p.IdPerson
                             join s in db.STATE on q.IdState equals s.IdState
                             where q.IdPerson == userId && q.IdQuote == quoteId
                             select new
                             {
                                 q.IdQuote,
                                 q.IdState,
                                 q.ServiceAddress,
                                 q.IdCrewPerson,
                                 q.ServiceDepartment,
                                 q.ServiceCity,
                                 q.Price,
                                 p.FirstName, //NombrePersona
                                 s.Name //NombreEstado
                             })
                            .AsEnumerable()
                            .Select(q => new QuoteDto
                            {
                                IdQuote = q.IdQuote,
                                IdState = q.IdState,
                                IdCrewPerson = q.IdCrewPerson ?? 0,
                                ServiceAddress = q.ServiceAddress,
                                ServiceDepartment = q.ServiceDepartment,
                                ServiceCity = q.ServiceCity,
                                Price = Convert.ToDouble(q.Price),
                                PersonName = q.FirstName,
                                StateName = q.Name
                            })
                            .FirstOrDefault();

                return quote;
            }
        }


        public List<QuoteDto> GetAllQuotes()
        {
            using (var db = new BD_14_02Entities())
            {
                var quotes = db.QUOTE
                    .Where(q => q.IdState == 1) // Filtrar solo las cotizaciones con estado 1
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
                        StarDate = p.StarDate ?? default(DateTime),
                        EndDate = p.EndDate ?? default(DateTime),
                        Price = (double)(p.Price ?? 0)
                    })
                    .ToList();

                return quotes;
            }
        }
        public List<QuoteDto> GetAllQuotesById(int userId)
        {
            using (var db = new BD_14_02Entities())
            {
                var quotes = (from q in db.QUOTE
                              join p in db.PERSON on q.IdPerson equals p.IdPerson
                              where q.IdPerson == userId && q.IdState == 2 // Filtrar por usuario y estado 2 "Solicitada"
                              select new QuoteDto
                              {
                                  IdQuote = q.IdQuote,
                                  IdPerson = q.IdPerson,
                                  IdState = q.IdState,
                                  ServiceAddress = q.ServiceAddress,
                                  ServiceDepartment = q.ServiceDepartment,
                                  ServiceCity = q.ServiceCity,
                                  StarDate = q.StarDate ?? DateTime.MinValue,
                                  EndDate = q.EndDate ?? DateTime.MinValue,
                                  Price = (double)(q.Price ?? 0),
                                  IdCrew = (int)q.IdCrewPerson
                              }).ToList();

                return quotes;
            }
        }
        public List<QuoteDto> GetAllQuotesAproved()
        {
            using (var db = new BD_14_02Entities())
            {
                var quotes = db.QUOTE
                    .Where(q => q.IdState == 3) // Filtrar solo las cotizaciones con estado 3
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
                        StarDate = (DateTime)(p.StarDate.HasValue ? p.StarDate.Value : (DateTime?)null),
                        EndDate = (DateTime)(p.EndDate.HasValue ? p.EndDate.Value : (DateTime?)null),
                        Price = (double)(p.Price ?? 0)
                    })
                    .ToList();

                return quotes;
            }
        }

    }

}