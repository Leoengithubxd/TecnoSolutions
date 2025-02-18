using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using Dapper;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using TechnoSolutions.Dtos;
using TechnoSolutions.Repositories;
using TecnoSolutions.Dtos;
using TecnoSolutions.Models;

namespace TecnoSolutions.Controllers
{
    public class ProductController : Controller
    {
        private const string V = "Update Successful";
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
                using (IDbConnection db = DatabaseHelper.GetConnection())
                {
                    string query = @"UPDATE PRODUCT 
                             SET Name = @Name, 
                                 Stock = @Stock, 
                                 UnitPrice = @UnitPrice 
                             WHERE IdProduct = @IdProduct";

                    int rowsAffected = db.Execute(query, product);

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
        public JsonResult Delete(int IdProduct)
        {
            try
            {
                _productRepository.DeleteProduct(IdProduct); // Aquí debería existir el método
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
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
        //[HttpPost]
        //public ActionResult GeneratePDF(Dictionary<int, int> requestedStock)
        //{
        //    // Aquí generas el PDF con la lista de productos solicitados
        //    var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
        //    var memoryStream = new System.IO.MemoryStream();
        //    var writer = iTextSharp.text.pdf.PdfWriter.GetInstance(document, memoryStream);
        //    document.Open();

        //    document.Add(new iTextSharp.text.Paragraph("Report of Products Ordered"));

        //    foreach (var entry in requestedStock)
        //    {
        //        var product = _productRepository.GetProductById(entry.Key);
        //        document.Add(new iTextSharp.text.Paragraph($"Product: {product.Name}, Requested Stock: {entry.Value}"));
        //    }

        //    document.Close();

        //    byte[] pdfBytes = memoryStream.ToArray();
        //    return File(pdfBytes, "application/pdf", "ProductReport.pdf");
        //}
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


        //public ActionResult ExportToPdf(List<int> selectedProducts, Dictionary<int, int> requestedStock)
        //{
        //    if (selectedProducts == null || !selectedProducts.Any())
        //    {
        //        return RedirectToAction("AnalistCrud");
        //    }

        //    // Obtener los productos seleccionados
        //    var products = _productRepository.GetProductsByIds(selectedProducts);

        //    // Mapear los productos a un DTO que incluya el stock solicitado
        //    var ProductDtos = products.Select(p => new PRODUCT
        //    {
        //        IdProduct = p.IdProduct,
        //        Name = p.Name,
        //        Stock = requestedStock.ContainsKey(p.IdProduct) ? requestedStock[p.IdProduct] : p.Stock,
        //        UnitPrice = p.UnitPrice
        //    }).ToList();

        //    // Generar el reporte en PDF desde la vista
        //    return new Rotativa.ViewAsPdf("GenerateReport", ProductDtos)
        //    {
        //        FileName = "Reporte_Productos.pdf"
        //    };
        //}
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
                UnitPrice = p.UnitPrice
            }).ToList();

            // Generar el reporte en PDF desde la vista
            return new Rotativa.ViewAsPdf("GenerateReport", productDtos)
            {
                FileName = "Reporte_Productos.pdf"
            };
        }

    }
}