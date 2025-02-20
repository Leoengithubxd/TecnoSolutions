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
            using (var db = new BD_14_03Entities())
            {
                var services = db.SERVICEs
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
            using (var db = new BD_14_03Entities())
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
                    db.QUOTEs.Add(quote);
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
    
        public void SaveQuoteProducts(int idQuote, List<ProductSelectionDto> selectedProducts)
        {
            using (var db = new BD_14_03Entities())
            {
                // Aquí ya NO creamos la cotización, porque ya existe
                var quote = db.QUOTEs.Find(idQuote);

                if (quote == null)
                {
                    throw new Exception("No se encontró la cotización con el Id especificado.");
                }

                foreach (var product in selectedProducts)
                {
                    var quoteProduct = new QUOTE_PRODUCT
                    {
                        IdQuote = idQuote,
                        IdProduct = product.IdProduct,
                        Quantity = product.Quantity,
                        TotalPrice = (double?)(product.UnitPrice * product.Quantity)
                    };
                    db.QUOTE_PRODUCT.Add(quoteProduct);
                }
                db.SaveChanges();
            }
        }


        public List<ProductSelectionDto> GetQuoteProducts(int quoteId)
        {
            using (var db = new BD_14_03Entities())
            {
                var products = db.QUOTE_PRODUCT
                    .Where(qp => qp.IdQuote == quoteId)
                    .Select(qp => new ProductSelectionDto
                    {
                        IdProduct = (int)qp.IdProduct,
                        Quantity = (int)qp.Quantity,
                        TotalPriceProduct = Convert.ToDecimal(qp.TotalPrice)
                    }).ToList();

                return products;
            }
        }

    
}
}