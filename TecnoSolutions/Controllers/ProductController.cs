using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TechnoSolutions.Dtos;
using TechnoSolutions.Repositories;

namespace TechnoSolutions.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly ProductPersonRepository _productPersonRepository;

        public ProductController()
        {
            _productRepository = new ProductRepository();
            _productPersonRepository = new ProductPersonRepository();
        }

        public ActionResult SelectProducts()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        [HttpPost]
        public ActionResult SelectProducts(List<ProductSelectionDto> selectedProducts)
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
                return RedirectToAction("SelectProducts");
            }

            _productPersonRepository.SaveSelectedProducts(userId, productsToSave);

            return RedirectToAction("PurchaseConfirmation");
        }
    }
}