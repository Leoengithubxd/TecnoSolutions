using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using Dapper;
using Microsoft.Ajax.Utilities;
using TechnoSolutions.Dtos;
using TechnoSolutions.Repositories;
using TecnoSolutions.Dtos;
using TecnoSolutions.Models;
using TecnoSolutions.Repository;
using TecnoSolutions.Controllers;
using System.Data.SqlClient;

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
        public ActionResult AnalistCrud()
        {
            var products = _productRepository.GetAllProducts();
            return View(products);
        }
        public ActionResult Edit(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return null;
            }
            return View(product);
        }
        [HttpPost]
        public ActionResult Edit(Product product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection("Data Source=LEO; Initial Catalog=BD 14_02; Integrated Security=true"))
                {
                    string query = @"UPDATE PRODUCT 
                             SET Name = @Name, 
                                 Stock = @Stock, 
                                 UnitPrice = @UnitPrice 
                             WHERE IdProduct = @IdProduct";

                    int rowsAffected = connection.Execute(query, product);

                    if (rowsAffected > 0)
                    {
                        return RedirectToAction("AnalistCrud"); // Redirige si la actualización fue exitosa.
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo actualizar el producto.");
                        return View(product);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el producto: " + ex.Message);
                return View(product); // Devuelve la vista con los datos ingresados.
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            _productRepository.DeleteProduct(id);
            return Json(new { success = true }); // Retorna un JSON indicando éxito
        }
        public ActionResult AddProducts()
        {
            return View();
        }
        public ActionResult SelectProducts()
        {
            var products = _productRepository.GetAllProducts();
            List<ProductSelectionDto> productDtos = products.Select(p => new ProductSelectionDto
            {
                IdProduct = p.IdProduct,
                NameProduct = p.Name,
                UnitPrice = Convert.ToDecimal(p.UnitPrice)
            }).ToList();
            return View(productDtos);
        }
        public ActionResult SelectProductsAnalist()
        {
            var products = _productRepository.GetAllProducts(); // Obtiene la lista de productos
            var productSelectionDtos = products.Select(p => new ProductSelectionDto // Convierte a ProductSelectionDto
            {
                IdProduct = p.IdProduct,
                NameProduct = p.Name,
                // Asigna otros campos según sea necesario
            }).ToList();

            return View(productSelectionDtos); // Pasa el modelo correcto a la vista
        }
        public ActionResult GenerateReport(List<int> selectedProducts)
        {
            if (selectedProducts == null || !selectedProducts.Any())
            {
                // Si no se seleccionan productos, redirigir al listado de productos
                return RedirectToAction("AnalistCrud");
            }

            // Obtener los productos seleccionados
            var products = _productRepository.GetProductsByIds(selectedProducts);

            // Pasar los productos a la vista
            return View("Report", products);
        }
        public ActionResult Report(List<int> ids)
        {
            return View();
        }
                public class ProductRequest
        {
            public int IdProduct { get; set; }
            public string Name { get; set; }
            public int RequestedStock { get; set; }

        }
        [HttpPost]
        public ActionResult GeneratePDF(List<ProductRequest> products)
        {
            if (products == null || !products.Any())
            {
                return Content("❌ No products selected or data not received.");
            }

            // Obtener nombres de productos desde el repositorio si no llegan en la lista
            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.Name))
                {
                    var dbProduct = _productRepository.GetProductById(product.IdProduct);
                    product.Name = dbProduct?.Name ?? "Unknown Product"; // Evita valores nulos
                }
            }

            // Crear el documento PDF
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            var memoryStream = new System.IO.MemoryStream();
            var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Agregar título
            document.Add(new iTextSharp.text.Paragraph("📝 Report of Products Ordered\n\n"));

            // Agregar los productos con nombre y stock solicitado
            foreach (var product in products)
            {
                document.Add(new iTextSharp.text.Paragraph($"📌 Product: {product.Name}, Requested Stock: {product.RequestedStock}\n"));
            }

            document.Close();

            byte[] pdfBytes = memoryStream.ToArray();
            return File(pdfBytes, "application/pdf", "ProductReport.pdf");
        }
        public ActionResult ExportToPdf(List<int> selectedProducts, Dictionary<int, int> requestedStock)
        {
            if (selectedProducts == null || !selectedProducts.Any())
            {
                return RedirectToAction("AnalistCrud");
            }

            // Si requestedStock es null, inicializarlo vacío para evitar errores
            if (requestedStock == null)
            {
                requestedStock = new Dictionary<int, int>();
            }
            // Obtener los productos seleccionados
            var products = _productRepository.GetProductsByIds(selectedProducts);

            // Mapear los productos a un DTO que incluya el stock solicitado
            var productDtos = products.Select(p => new PRODUCT
            {
                IdProduct = p.IdProduct,
                Name = p.Name,
                Stock = requestedStock.ContainsKey(p.IdProduct) ? requestedStock[p.IdProduct] : p.Stock,
                UnitPrice = (double?)Convert.ToDecimal(p.UnitPrice)
            }).ToList();

            // Generar el reporte en PDF desde la vista
            return new Rotativa.ViewAsPdf("GenerateReport", productDtos)
            {
                FileName = "Reporte_Productos.pdf"
            };
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
        public List<Product> GetProductsByIds(List<int> ids)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=LEO; Initial Catalog=BD 14_02; Integrated Security=true"))
            {
                string query = "SELECT * FROM PRODUCT WHERE IdProduct IN @Ids";
                return connection.Query<Product>(query, new { Ids = ids }).ToList();
            }
        }


        [HttpPost] //Cargar tabla Product_Person
        public ActionResult SelectProducts(List<ProductSelectionDto > selectedProducts, string address, string department, string city)
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
            _productPersonRepository.SaveSelectedProducts(userId, productsToSave, address, department, city);
            return RedirectToAction("PurchaseConfirmation","Product");
        }

        [HttpPost] //Confirmar Compra
        public ActionResult ConfirmPurchase(List<ProductSelectionDto> selectedProducts, string address, string department, string city)
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
            _productPersonRepository.CreateInvoice(userId, purchaseProducts, address, department, city);

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