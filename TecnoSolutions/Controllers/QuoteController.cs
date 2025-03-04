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
        private readonly ProductRepository _productRepository;

        public QuoteController()
        {
            _quoteRepository = new QuoteRepository();
            _productRepository = new ProductRepository();
        }

        public ActionResult SelectServices() //Cargar servicios
        {
            var services = _quoteRepository.GetAllServices();
            return View(services);
        }
        public ActionResult ApproveQuote() //Traer cotizaciones por persona
        {
            using (var db = new BD_14_02Entities())
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                var quotes = (from q in db.QUOTE
                              join s in db.STATE on q.IdState equals s.IdState
                              where q.IdPerson == userId
                              select new
                              {
                                  q.IdQuote,
                                  q.IdState,
                                  q.Price,
                                  s.Name // Nombre del estado
                              })
                             .AsEnumerable()
                             .Select(q => new QuoteDto
                             {
                                 IdQuote = q.IdQuote,
                                 IdState = q.IdState,
                                 Price = Convert.ToDouble(q.Price),
                                 StateName = q.Name
                             })
                             .ToList();

                return View(quotes);
            }
        }
        public ActionResult QuoteDetails(int? quoteId) //Detalle cotizacion
        {
            if (!quoteId.HasValue)
            {
                return RedirectToAction("ApproveQuote");
            }
            int userId = Convert.ToInt32(Session["UserId"]);

            var quotes = _quoteRepository.QuoteDetail(userId, quoteId);

            return View(quotes);
        }


        public ActionResult ListQuotess()
        {
            var quotes = _quoteRepository.GetAllQuotes();
            return View(quotes);
        }
        [HttpPost]
        public ActionResult SaveQuotes(List<QuoteDto> quoteDtos)
        {
            if (quoteDtos == null || !quoteDtos.Any())
            {
                ViewBag.Message = "No se recibieron datos.";
                return View("ListQuotess", quoteDtos);
            }

            try
            {
                using (var db = new BD_14_02Entities())
                {
                    foreach (var quote in quoteDtos)
                    {
                        var existingQuote = db.QUOTE.FirstOrDefault(q => q.IdQuote == quote.IdQuote);

                        if (existingQuote != null)
                        {
                            bool hasChanges = false;

                            // Verificar y actualizar cada campo solo si hay un cambio
                            if (quote.IdCrew != 0 && existingQuote.IdCrewPerson != quote.IdCrew)
                            {
                                existingQuote.IdCrewPerson = quote.IdCrew;
                                hasChanges = true;
                            }
                            if (quote.StarDate != DateTime.MinValue && existingQuote.StarDate != quote.StarDate)
                            {
                                existingQuote.StarDate = quote.StarDate;
                                hasChanges = true;
                            }
                            if (quote.EndDate != DateTime.MinValue && existingQuote.EndDate != quote.EndDate)
                            {
                                existingQuote.EndDate = quote.EndDate;
                                hasChanges = true;
                            }
                            if (quote.Price != 0 && existingQuote.Price != (double?)quote.Price)
                            {
                                existingQuote.Price = (double?)quote.Price;
                                hasChanges = true;
                            }

                            // Si hubo cambios, actualizar el estado a 2
                            if (hasChanges)
                            {
                                existingQuote.IdState = 2;
                            }
                        }
                    }

                    db.SaveChanges();
                }

                ViewBag.Message = "Cotizaciones actualizadas correctamente.";
                return RedirectToAction("ListQuotess");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al actualizar: " + ex.Message;
                return View("ListQuotess", quoteDtos);
            }
        }
        public ActionResult SelectProducts(int id)
        {
            // Obtener todos los productos
            var products = _productRepository.GetAllProducts();

                    // Convertir a DTO para la vista
                    var productDtos = products.Select(p => new ProductSelectionDto
                    {
                        IdProduct = p.IdProduct,  // Asegurar que coincide con el modelo esperado
                        NameProduct = p.Name,
                        UnitPrice = decimal.TryParse(p.UnitPrice, out decimal price) ? price : 0m,

                        Quantity = 0, // Inicializar en 0
               
                    }).ToList();

            ViewBag.IdQuote = id; // Pasar el ID de la cotización

            return View(productDtos);
        }

        [HttpPost]
        public ActionResult SelectProducts(List<ProductSelectionDto> selectedProducts, string address, string department, string city, int idQuote)
        {
            if (selectedProducts == null || !selectedProducts.Any(p => p.Quantity > 0))
            {
                // Si no hay productos seleccionados con cantidad válida, regresar a la vista con un mensaje
                TempData["ErrorMessage"] = "Debe seleccionar al menos un producto con cantidad mayor a 0.";
                return RedirectToAction("SelectProducts", new { id = idQuote });
            }

            using (var db = new BD_14_02Entities())
            {
                foreach (var product in selectedProducts)
                {
                    if (product.Quantity > 0) // Solo guardar productos con cantidad mayor a 0
                    {
                        var newQuoteProduct = new QUOTE_PRODUCT
                        {
                            IdQuote = idQuote, // Usar el ID correcto de la cotización
                            IdProduct = product.IdProduct,
                            Quantity = product.Quantity,
                            TotalPrice = (double?)(product.Quantity * product.UnitPrice)
                        };

                        db.QUOTE_PRODUCT.Add(newQuoteProduct);
                    }
                }

                db.SaveChanges(); // Guardar cambios en la BD solo una vez
            }

            // Redireccionar a la lista de cotizaciones después de guardar
            return RedirectToAction("ListQuotess");
        }


        public ActionResult ApproveQuoteAdmin()
        {
            using (var db = new BD_14_02Entities())
            {
                var quotes = db.QUOTE
                    .Where(p => p.IdState == 4)
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
                        Price = Convert.ToDouble(p.Price)
                    })
                    .ToList();

                return View(quotes);
            }
        }


        public ActionResult QuoteDetailsAdmin(int? quoteId)
        {
            if (!quoteId.HasValue)
            {
                return RedirectToAction("ApproveQuoteAdmin");
            }
            using (var db = new BD_14_02Entities())
            {
                var quote = (from q in db.QUOTE
                             join s in db.STATE on q.IdState equals s.IdState
                             join cp in db.CREW_PERSON on q.IdCrewPerson equals cp.IdCrewPerson
                             join p in db.PERSON on q.IdPerson equals p.IdPerson
                             where q.IdQuote == quoteId
                             select new QuoteDto
                             {  
                                 IdQuote = q.IdQuote,
                                 PersonName = p.FirstName + " " + p.LastName,
                                 StateName = s.Name,
                                 ServiceAddress = q.ServiceAddress,
                                 ServiceCity = q.ServiceCity,
                                 ServiceDepartment = q.ServiceDepartment,
                                 StarDate = (DateTime)q.StarDate,
                                 EndDate = (DateTime)q.EndDate,
                                 IdCrewPerson = cp.IdCrewPerson, // Agregado
                                 Price = (double)q.Price
                             }).FirstOrDefault();
                return View(quote);
            }
        }

        public ActionResult ApproveQuoteAssis()
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
                        Price = Convert.ToDouble(p.Price)
                    })
                    .ToList();

                return View(quotes);
            }
        }
        public ActionResult ApproveQuoteAna()
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
                        Price = Convert.ToDouble(p.Price)
                    })
                    .ToList();

                return View(quotes);
            }
        }
        //public ActionResult QuoteDetails(int id)
        //{
        //    using (var db = new BD_14_02Entities())
        //    {
        //        var quote = db.QUOTE
        //            .Where(p => p.IdQuote == id)
        //            .Select(p => new
        //            {
        //                p.IdQuote,
        //                p.IdPerson,
        //                p.IdState,
        //                p.ServiceAddress,
        //                p.ServiceDepartment,
        //                p.ServiceCity,
        //                p.StarDate,
        //                p.EndDate,
        //                p.Price
        //            })
        //            .AsEnumerable()
        //            .Select(p => new QuoteDto
        //            {
        //                IdQuote = p.IdQuote,
        //                IdPerson = p.IdPerson,
        //                IdState = p.IdState,
        //                ServiceAddress = p.ServiceAddress,
        //                ServiceDepartment = p.ServiceDepartment,
        //                ServiceCity = p.ServiceCity,
        //                StarDate = p.StarDate,
        //                EndDate = p.EndDate,
        //                Price = p.Price.HasValue ? Convert.ToDouble(p.Price) : 0
        //            })
        //            .FirstOrDefault();

        //        if (quote == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        return View(quote);
        //    }
        //}
        //public ActionResult QuoteDetailsAna(int id)
        //{
        //    using (var db = new BD_14_02Entities())
        //    {
        //        var quote = db.QUOTE
        //            .Where(p => p.IdQuote == id)
        //            .Select(p => new
        //            {
        //                p.IdQuote,
        //                p.IdPerson,
        //                p.IdState,
        //                p.ServiceAddress,
        //                p.ServiceDepartment,
        //                p.ServiceCity,
        //                p.StarDate,
        //                p.EndDate,
        //                p.Price
        //            })
        //            .AsEnumerable()
        //            .Select(p => new QuoteDto
        //            {
        //                IdQuote = p.IdQuote,
        //                IdPerson = p.IdPerson,
        //                IdState = p.IdState,
        //                ServiceAddress = p.ServiceAddress,
        //                ServiceDepartment = p.ServiceDepartment,
        //                ServiceCity = p.ServiceCity,
        //                StarDate = p.StarDate,
        //                EndDate = p.EndDate,
        //                Price = p.Price.HasValue ? Convert.ToDouble(p.Price) : 0
        //            })
        //            .FirstOrDefault();

        //        if (quote == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        return View(quote);
        //    }
        //}
        //public ActionResult QuoteDetailsA(int id)
        //{
        //    using (var db = new BD_14_02Entities())
        //    {
        //        var quote = db.QUOTE
        //            .Where(p => p.IdQuote == id)
        //            .Select(p => new
        //            {
        //                p.IdQuote,
        //                p.IdPerson,
        //                p.IdState,
        //                p.ServiceAddress,
        //                p.ServiceDepartment,
        //                p.ServiceCity,
        //                p.StarDate,
        //                p.EndDate,
        //                p.Price
        //            })
        //            .AsEnumerable()
        //            .Select(p => new QuoteDto
        //            {
        //                IdQuote = p.IdQuote,
        //                IdPerson = p.IdPerson,
        //                IdState = p.IdState,
        //                ServiceAddress = p.ServiceAddress,
        //                ServiceDepartment = p.ServiceDepartment,
        //                ServiceCity = p.ServiceCity,
        //                StarDate = p.StarDate,
        //                EndDate = p.EndDate,
        //                Price = p.Price.HasValue ? Convert.ToDouble(p.Price) : 0
        //            })
        //            .FirstOrDefault();

        //        if (quote == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        return View(quote);
        //    }
        //}
        //public ActionResult QuoteDetailsAS(int id)
        //{
        //    using (var db = new BD_14_02Entities())
        //    {
        //        var quote = db.QUOTE
        //            .Where(p => p.IdQuote == id)
        //            .Select(p => new
        //            {
        //                p.IdQuote,
        //                p.IdPerson,
        //                p.IdState,
        //                p.ServiceAddress,
        //                p.ServiceDepartment,
        //                p.ServiceCity,
        //                p.StarDate,
        //                p.EndDate,
        //                p.Price
        //            })
        //            .AsEnumerable()
        //            .Select(p => new QuoteDto
        //            {
        //                IdQuote = p.IdQuote,
        //                IdPerson = p.IdPerson,
        //                IdState = p.IdState,
        //                ServiceAddress = p.ServiceAddress,
        //                ServiceDepartment = p.ServiceDepartment,
        //                ServiceCity = p.ServiceCity,
        //                StarDate = p.StarDate,
        //                EndDate = p.EndDate,
        //                Price = p.Price.HasValue ? Convert.ToDouble(p.Price) : 0
        //            })
        //            .FirstOrDefault();

        //        if (quote == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        return View(quote);
        //    }
        //}



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

            return RedirectToAction("ApproveQuote", "Quote");
        }


        //22/02/2025
        public ActionResult ListQuotesAproved()
        {
            var quotes = _quoteRepository.GetAllQuotesAproved();
            return View(quotes);
        }
        [HttpPost]
        public JsonResult AcceptQuote(int id)
        {
            using (var db = new BD_14_02Entities())
            {
                var quote = db.QUOTE.FirstOrDefault(q => q.IdQuote == id);

                if (quote == null)
                {
                    return Json(new { success = false, message = "Cotización no encontrada." });
                }
                quote.IdState = 3;
                db.SaveChanges();

                return Json(new { success = true });
            }
        }
        [HttpPost]
        public JsonResult EndQuote(int id)
        {
            using (var db = new BD_14_02Entities())
            {
                var quote = db.QUOTE.FirstOrDefault(q => q.IdQuote == id);

                if (quote == null)
                {
                    return Json(new { success = false, message = "Cotización no encontrada." });
                }

                quote.IdState = 4;
                db.SaveChanges();

                return Json(new { success = true });
            }
        }


        public ActionResult MyQuotes()
        {
            int userId = Convert.ToInt32(Session["UserId"]); // Obtener el ID del usuario autenticado

            var quotes = _quoteRepository.GetAllQuotesById(userId); // Obtener las cotizaciones asignadas a este usuario

            return View(quotes); // Enviar la lista de cotizaciones a la vista
        }

        [HttpPost]
        public JsonResult BuyQuote(int idQuote) //Comprar cotizacion
        {
            using (var db = new BD_14_02Entities())
            {
                var quote = db.QUOTE.FirstOrDefault(q => q.IdQuote == idQuote);
                if (quote != null)
                {
                    quote.IdState = 3;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public JsonResult DeleteQuote(int idQuote) //Eliminar cotizacion
        {
            using (var db = new BD_14_02Entities())
            {
                var quote = db.QUOTE.FirstOrDefault(q => q.IdQuote == idQuote);
                if (quote != null)
                {
                    // Eliminar la cotización
                    db.QUOTE.Remove(quote);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false });
            }
        }


        public ActionResult EmployeeHome()
        {
            using (var db = new BD_14_02Entities())
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                var crewId = (from cp in db.CREW_PERSON
                              where cp.IdPerson == userId
                              select cp.IdCrew).FirstOrDefault();

                if (crewId == 0)
                {
                    return View(new List<QuoteDto>());
                }

                var quotes = (from q in db.QUOTE
                              join cp in db.CREW_PERSON on q.IdCrewPerson equals cp.IdCrewPerson
                              join s in db.STATE on q.IdState equals s.IdState
                              where cp.IdCrew == crewId
                              select new
                              {
                                  q.IdQuote,
                                  q.IdState,
                                  q.Price,
                                  s.Name // Nombre del estado
                              })
                             .AsEnumerable()
                             .Select(q => new QuoteDto
                             {
                                 IdQuote = q.IdQuote,
                                 IdState = q.IdState,
                                 Price = Convert.ToDouble(q.Price),
                                 StateName = q.Name
                             })
                             .ToList();

                return View(quotes);
            }
        } //Cargar Asignaciones Cotizaciones

        public ActionResult QuoteDetailsEmployee(int quoteId)
        {
            using (var db = new BD_14_02Entities())
            {
                var quote = (from q in db.QUOTE
                             join s in db.STATE on q.IdState equals s.IdState
                             join cp in db.CREW_PERSON on q.IdCrewPerson equals cp.IdCrewPerson
                             join p in db.PERSON on q.IdPerson equals p.IdPerson
                             where q.IdQuote == quoteId
                             select new QuoteDto
                             {
                                 IdQuote = q.IdQuote,
                                 PersonName = p.FirstName + " " + p.LastName,
                                 StateName = s.Name,
                                 ServiceAddress = q.ServiceAddress,
                                 ServiceCity = q.ServiceCity,
                                 ServiceDepartment = q.ServiceDepartment,
                                 StarDate = (DateTime)q.StarDate,
                                 EndDate = (DateTime)q.EndDate,
                                 IdCrewPerson = cp.IdCrewPerson, // Agregado
                                 Price = (double)q.Price
                             }).FirstOrDefault();

                if (quote == null)
                {
                    return HttpNotFound(); // Retorna un error 404 si no encuentra la cotización
                }

                return View(quote);
            }
        }

    }
}