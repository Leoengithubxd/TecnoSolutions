using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
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

        public void UpdateQuotes(List<QuoteDto> quotes)
        {
            using (var connection = new SqlConnection("Data Source=LEO; Initial Catalog=BD 14_02; Integrated Security=true"))
            {
                connection.Open();
                foreach (var quote in quotes)
                {
                    var query = @"
                UPDATE QUOTE 
                SET IdCrewPerson = @IdCrew, 
                    StarDate = @StarDate, 
                    EndDate = @EndDate, 
                    Price = @Price 
                WHERE IdQuote = @IdQuote";
                    connection.Execute(query, quote);
                }
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
        public List<QuoteDto> GetAllQuotes()
        {
            using (var db = new BD_14_02Entities())
            {
                var quotes = from q in db.QUOTE
                             join p in db.PERSON on q.IdPerson equals p.IdPerson
                             join s in db.STATE on q.IdState equals s.IdState
                             select new QuoteDto
                             {
                                 IdQuote = q.IdQuote,
                                 PersonName = p.FirstName + " " + p.LastName,
                                 State = s.Name,
                                 ServiceAddress = q.ServiceAddress,
                             };

                return quotes.ToList();
            }
        }
    }
}