using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TechnoSolutions.Dtos;
using TechnoSolutions.Repositories;
using TecnoSolutions.Models;

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

        public ActionResult AddProducts()
        {
            return View();
        }
        public ActionResult SelectProducts()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }

        public ActionResult Invoice()
        {
            return View();
        }

        [HttpGet] //Mostrar Productos tabla Product_Person
        public ActionResult PurchaseConfirmation(List<ProductSelectionDto> GetSelectedProducts)
        {
            var userId = (Session["IdUser"] != null) ? (int)Session["IdUser"] : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

            var selectedProducts = _productPersonRepository.GetSelectedProducts(userId);

            if (selectedProducts == null || !selectedProducts.Any())
            {
                return RedirectToAction("SelectProducts");
            }

            ViewBag.TotalGeneral = selectedProducts.Sum(p => p.TotalPriceProduct);
            return View(selectedProducts);
        }



        [HttpPost] //Cargar tabla Product_Person
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
            return RedirectToAction("PurchaseConfirmation","Product");
        }

        [HttpPost] //Confirmar Compra
        public ActionResult ConfirmPurchase()
        {
            var userId = (Session["IdUser"] != null) ? (int)Session["IdUser"] : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }
            var purchaseProducts = _productPersonRepository.GetSelectedProducts(userId);
            if (purchaseProducts == null || !purchaseProducts.Any())
            {
                TempData["Message"] = "No tienes productos seleccionados.";
                return RedirectToAction("SelectProducts");
            }
            _productPersonRepository.DeleteUserProducts(userId);

            TempData["Message"] = "Compra realizada";

            ViewBag.TotalGeneral = purchaseProducts.Sum(p => p.TotalPriceProduct);
            return View("Invoice", purchaseProducts);
        }

        [HttpPost] //Cancelar compra
        public ActionResult CancelPurchase()
        {
            var userId = (Session["IdUser"] != null) ? (int)Session["IdUser"] : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User");
            }

            _productPersonRepository.DeleteUserProducts(userId);

            TempData["Message"] = "Compra cancelada. Los productos han sido eliminados.";
            return RedirectToAction("SelectProducts");
        }
    }
}