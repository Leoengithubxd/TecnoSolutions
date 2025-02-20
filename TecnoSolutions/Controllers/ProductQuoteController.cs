using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TechnoSolutions.Dtos;
using TechnoSolutions.Repositories;

namespace TechnoSolutions.Controllers
{
    public class ProductQuoteController : Controller
    {
        private readonly QuoteRepository _quoteRepository = new QuoteRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();


        public ActionResult SelectProductsQuote(int idQuote)
        {
            ViewBag.IdQuote = idQuote;

            var products = _productRepository.GetAllProducts()
                .Select(p => new ProductSelectionDto
                {
                    IdProduct = p.IdProduct,
                    NameProduct = p.Name,
                    UnitPrice = decimal.Parse(p.UnitPrice),
                    Quantity = 0
                }).ToList();

            return View(products);
        }

        [HttpPost]
        public ActionResult SelectProductsQuote(int IdQuote, List<ProductSelectionDto> selectedProducts)
        {
            var userId = (Session["IdUser"] != null) ? (int)Session["IdUser"] : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

            var productsToSave = selectedProducts?.Where(p => p.Quantity > 0).ToList();
            if (productsToSave == null || !productsToSave.Any())
            {
                TempData["Message"] = "Debe seleccionar al menos un producto y especificar la cantidad.";
                return RedirectToAction("SelectProductsQuote", new { idQuote = IdQuote });
            }

            _quoteRepository.SaveQuoteProducts(IdQuote, productsToSave);
            return RedirectToAction("QuoteConfirmation");
        }
    }
    }
