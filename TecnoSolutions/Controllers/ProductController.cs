using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TechnoSolutions.Dtos;
using TechnoSolutions.Repositories;
using TecnoSolutions.Dtos;
using TecnoSolutions.Models;

namespace TecnoSolutions.Controllers
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
        
        public ActionResult AnalistCrud()
        {
            var products = _productRepository.GetAllProducts(); 
            return View(products); 
        }

        //public ActionResult DeleteCrud()
        //{
        //    var products = _productRepository.GetAllProducts();
        //    return View(products);
        //}
            

        [HttpPost]
        public ActionResult Create(PRODUCT product) 
        {
            if (ModelState.IsValid)
            {
                _productRepository.AddProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int IdProduct)
        {
            var product = _productRepository.GetProductById(IdProduct);
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        public ActionResult Edit(PRODUCT product) // Cambia PRODUCT a Product
        {
            if (ModelState.IsValid)
            {
                _productRepository.UpdateProduct(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                _productRepository.DeleteProduct(id); // Llama al método del repositorio para eliminar el producto
                return Json(new { success = true }); // Devuelve un resultado exitoso
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }); // Devuelve un error si ocurre
            }
        }

        // GET: Select Products
        public ActionResult AddProducts()
        {
            return View();
        }
        public ActionResult SelectProducts()
        {
            var products = _productRepository.GetAllProducts(); // Obtiene la lista de productos
            var productSelectionDtos = products.Select(p => new ProductSelectionDto // Convierte a ProductSelectionDto
            {
                IdProduct = p.IdProduct,
                Name = p.Name,
                // Asigna otros campos según sea necesario
            }).ToList();

            return View(productSelectionDtos); // Pasa el modelo correcto a la vista
        }

        [HttpPost]
        public ActionResult SelectProducts(List<ProductSelectionDto> selectedProducts)
        {
            var userId = (Session["IdUser "] != null) ? (int)Session["IdUser "] : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "User ");
            }

            var productsToSave = selectedProducts?.Where(p => p.Quantity > 0).ToList();

            if (productsToSave == null || !productsToSave.Any())
            {
                TempData["Message"] = "Debe seleccionar al menos un producto y especificar la cantidad.";
                return RedirectToAction("SelectProducts");
            }

            _productPersonRepository.SaveSelectedProducts(userId, productsToSave);

            return RedirectToAction("purchaseConfirmation");
        }
    }
}