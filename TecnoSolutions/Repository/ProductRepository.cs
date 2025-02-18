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
        public List<ProductSelectionDto> GetAllProducts() //Traer tabla Product
        {
            using (var db = new BD_14_02Entities3())
            {
                var products = db.PRODUCT
                    .Select(p => new
                    {
                        p.IdProduct,
                        p.Name,
                        p.UnitPrice
                    })
                    .AsEnumerable()
                    .Select(p => new ProductSelectionDto
                    {
                        IdProduct = p.IdProduct,
                        NameProduct = p.Name,
                        UnitPrice = Convert.ToDecimal(p.UnitPrice),
                        Quantity = 0
                    })
                    .ToList();

                return products;
            }
        }
    }
    public class ProductPersonRepository
    {
        public void SaveSelectedProducts(int userId, List<ProductSelectionDto> selectedProducts) //Llenar tabla Product_Person
        {
            using (var db = new BD_14_02Entities3())
            {
                foreach (var product in selectedProducts)
                {
                    var productPerson = new PRODUCT_PERSON
                    {
                        IdPerson = userId,
                        IdProduct = product.IdProduct,
                        NameProduct = product.NameProduct,
                        Quantity = product.Quantity,
                        UnitPrice = (double?)product.UnitPrice,
                        TotalPriceProduct = (double?)(product.UnitPrice * product.Quantity),
                    };
                    db.PRODUCT_PERSON.Add(productPerson);
                }
                db.SaveChanges();
            }
        }

        public List<ProductSelectionDto> GetSelectedProducts(int userId) //Traer productos seleccionados de tabla Product_Person
        {
            using (var db = new BD_14_02Entities3())
            {
                var productsSelected = db.PRODUCT_PERSON
                .Where(p => p.IdPerson == userId)
                .Select(p => new
                {
                    p.IdProduct,
                    p.NameProduct,
                    p.Quantity,
                    p.UnitPrice,
                    p.TotalPriceProduct
                })
                .AsEnumerable() 
                .Select(p1 => new ProductSelectionDto
                {
                    IdProduct = (int)p1.IdProduct,
                    NameProduct = p1.NameProduct,
                    UnitPrice = Convert.ToDecimal(p1.UnitPrice),
                    Quantity = (int)p1.Quantity,
                    TotalPriceProduct = Convert.ToDecimal(p1.TotalPriceProduct)
                })
                .ToList();
                return productsSelected;
            }
        } 

        public void DeleteUserProducts(int userId) //Eliminar productos seleccionados tabla Product_Person
        {
            using (var db = new BD_14_02Entities3())
            {
                var userProducts = db.PRODUCT_PERSON.Where(p => p.IdPerson == userId).ToList();

                if (userProducts.Any())
                {
                    db.PRODUCT_PERSON.RemoveRange(userProducts);
                    db.SaveChanges();
                }
            }
        }
    }
}