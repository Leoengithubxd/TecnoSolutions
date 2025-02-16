using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnoSolutions.Dtos;
using TecnoSolutions.Models;
namespace TechnoSolutions.Repositories
{
    public class ProductRepository
    {
      
            public List<ProductSelectionDto> GetAllProducts()
            {
                using (var db = new Entities7())
                {
                    var products = db.PRODUCTs
                        .Select(p => new
                        {
                            p.IdProduct,
                            p.Name,
                            p.UnitPrice
                        })
                        .AsEnumerable() // Convertir los resultados a memoria
                        .Select(p => new ProductSelectionDto
                        {
                            IdProduct = p.IdProduct,
                            Name = p.Name,
                            UnitPrice = Convert.ToDecimal(p.UnitPrice), // Conversión segura en memoria
                    Quantity = 0
                        })
                        .ToList();

                    return products;
                }
            }
        }

    
    public class ProductPersonRepository
    {
        public void SaveSelectedProducts(int userId, List<ProductSelectionDto> selectedProducts)
        {
            using (var db = new Entities7())
            {
                foreach (var product in selectedProducts)
                {
                    var productPerson = new PRODUCT_PERSON
                    {
                        IdPerson = userId,
                        IdProduct = product.IdProduct,
                        Stock = product.Quantity,
                        UnitPrice = (double?)product.UnitPrice,
                        TotalPriceProduct = (double?)product.TotalPriceProduct,
                        TotalPrice = (double?)product.TotalPriceProduct
                    };
                    db.PRODUCT_PERSON.Add(productPerson);
                }
                db.SaveChanges();
            }
        }
    }
}