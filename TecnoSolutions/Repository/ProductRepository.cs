using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using TechnoSolutions.Dtos;

using TecnoSolutions.Models;
using TecnoSolutions.Repository;
using TechnoSolutions.Controllers;
using System.Web.Mvc;
namespace TechnoSolutions.Repositories
{
    public class ProductRepository
    {
        public string connectionString = "Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true";


        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            using (SqlConnection connection = new SqlConnection("Data Source=LEO; Initial Catalog=BD 14_02; Integrated Security=true"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM PRODUCT", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        IdProduct = (int)reader["IdProduct"], // No nulo, se puede convertir directamente
                        Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : string.Empty, // Manejo de nulos
                        Stock = reader["Stock"] != DBNull.Value ? Convert.ToSingle(reader["Stock"]) : 0.0f, // Manejo de nulos
                        UnitPrice = reader["UnitPrice"] != DBNull.Value ? reader["UnitPrice"].ToString() : "0.0" // Manejo de nulos
                    });
                }
            }
            return products;
        }

        public void UpdateProduct(PRODUCT product)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=LEO; Initial Catalog=BD 14_02; Integrated Security=true"))
            {
                string sql = "UPDATE PRODUCT SET Name = @Name, Stock = @Stock, UnitPrice = @UnitPrice WHERE IdProduct = @IdProduct";
                var parameters = new
                {
                    IdProduct = product.IdProduct, // Asegúrate de que el ID esté correctamente asignado
                    Name = product.Name,
                    Stock = product.Stock,
                    UnitPrice = product.UnitPrice
                };

                int rowsAffected = connection.Execute(sql, parameters); // Ejecuta la consulta y obtiene el número de filas afectadas

                // Agrega un log para verificar cuántas filas se actualizaron
                Console.WriteLine($"Rows affected: {rowsAffected}");
            }
        }
        public List<Product> GetProductsByIds(List<int> ids)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=LEO; Initial Catalog=BD 14_02; Integrated Security=true"))
            {
                string query = "SELECT * FROM PRODUCT WHERE IdProduct IN @Ids";
                return connection.Query<Product>(query, new { Ids = ids }).ToList();
            }
        }
        public PRODUCT GetProductById(int id)
        {
            using (IDbConnection db = DatabaseHelper.GetConnection())
            {
                // Cambia @Id a @id para que coincida con el nombre del parámetro en el objeto anónimo
                return db.QuerySingleOrDefault<PRODUCT>("SELECT * FROM PRODUCT WHERE IdProduct = @id", new { id });
            }
        }

        public void DeleteProduct(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM PRODUCT WHERE IdProduct = @IdProduct";
                var parameters = new { IdProduct = id };
                connection.Execute(sql, parameters);
            }
        }
    }

    public class ProductPersonRepository
    {
        public string connectionString = "Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true";
        public void DeleteProductsByUserId(int userId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "DELETE FROM PRODUCT_PERSON WHERE IdPerson = @UserId";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveSelectedProducts(int userId, List<ProductSelectionDto> selectedProducts,
            string address, string department, string city) //Llenar tabla Product_Person
        {
            using (var db = new BD_14_02Entities())
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
                        Address = address,
                        Department = department,
                        City = city,
                    };
                    db.PRODUCT_PERSON.Add(productPerson);
                }
                db.SaveChanges();
            }
        }

        public List<ProductSelectionDto> GetSelectedProducts(int userId) //Traer productos seleccionados de tabla Product_Person
        {
            using (var db = new BD_14_02Entities())
            {
                var productsSelected = db.PRODUCT_PERSON
                .Where(p => p.IdPerson == userId)
                .Select(p => new
                {
                    p.IdProduct,
                    p.NameProduct,
                    p.Quantity,
                    p.UnitPrice,
                    p.TotalPriceProduct,
                    p.Address,
                    p.Department,
                    p.City
                })
                .AsEnumerable()
                .Select(p1 => new ProductSelectionDto
                {
                    IdProduct = (int)p1.IdProduct,
                    NameProduct = p1.NameProduct,
                    UnitPrice = Convert.ToDecimal(p1.UnitPrice),
                    Quantity = (int)p1.Quantity,
                    TotalPriceProduct = Convert.ToDecimal(p1.TotalPriceProduct),
                    ProductsAddress = p1.Address,
                    ProductsDepartment = p1.Department,
                    ProductsCity = p1.City
                })
                .ToList();
                return productsSelected;
            }
        }

        public void DeleteUserProducts(int userId) //Eliminar productos seleccionados tabla Product_Person
        {
            using (var db = new BD_14_02Entities())
            {
                var userProducts = db.PRODUCT_PERSON.Where(p => p.IdPerson == userId).ToList();

                if (userProducts.Any())
                {
                    db.PRODUCT_PERSON.RemoveRange(userProducts);
                    db.SaveChanges();
                }
            }
        }

        public ProductSelectionDto GetAddressInvoice(int userId) //Traer Direccion Factura
        {
            using (var db = new BD_14_02Entities())
            {
                return db.PRODUCT_PERSON
                         .Where(p => p.IdPerson == userId)
                         .Select(p => new ProductSelectionDto
                         {
                             ProductsAddress = p.Address,
                             ProductsDepartment = p.Department,
                             ProductsCity = p.City
                         })
                         .FirstOrDefault();
            }
        }

        public void CreateInvoice(int userId, List<ProductSelectionDto> selectedProducts,
            string address, string department, string city)
        {
            using (var db = new BD_14_02Entities())
            {
                var invoice = new INVOICE
                { 
                    IdPerson = userId,
                    Address = address,
                    Department = department,
                    City = city,
                    TotalPrice = (double)selectedProducts.Sum(p => p.UnitPrice * p.Quantity),
                    RegisteredAt = DateTime.Now
                };

                db.INVOICE.Add(invoice);
                db.SaveChanges();

                var invoiceProducts = selectedProducts.Select(product => new INVOICE_PRODUCT
                {
                    IdInvoice = invoice.IdInvoice,
                    IdProduct = product.IdProduct,
                    NameProduct = product.NameProduct,
                    Quantity = product.Quantity,
                    UnitPrice = (double)product.UnitPrice,
                    TotalPriceProduct = (double)(product.UnitPrice * product.Quantity)
                }).ToList();

                db.INVOICE_PRODUCT.AddRange(invoiceProducts);
                db.SaveChanges();
            }
        } //Crear Factura

        public List<InvoiceDto> GetInvoices(int userId)
        {
            using (var db = new BD_14_02Entities())
            {
                return db.INVOICE
                         .Where(i => i.IdPerson == userId)
                         .OrderByDescending(i => i.RegisteredAt)
                         .ToList()
                         .Select(i => new InvoiceDto
                         {
                             IdInvoice = i.IdInvoice,
                             RegisteredAt = i.RegisteredAt,
                             TotalPriceProduct = Convert.ToDecimal(i.TotalPrice)
                         })
                         .ToList();
            }
        }

        public List<InvoiceDto> GetInvoicesDelivery()
        {
            using (var db = new BD_14_02Entities())
            {
                return db.INVOICE
                         .OrderByDescending(i => i.RegisteredAt)
                         .ToList()
                         .Select(i => new InvoiceDto
                         {
                             IdInvoice = i.IdInvoice,
                             RegisteredAt = i.RegisteredAt,
                             TotalPriceProduct = Convert.ToDecimal(i.TotalPrice)
                         })
                         .ToList();
            }
        }

        public InvoiceDto GetInvoiceDetail(int userId, int? invoiceId)
        {
            using (var db = new BD_14_02Entities())
            {
                var invoice = (from i in db.INVOICE
                               join p in db.PERSON on i.IdPerson equals p.IdPerson
                               where i.IdPerson == userId && i.IdInvoice == invoiceId
                               select new
                               {
                                   i.IdInvoice,
                                   i.RegisteredAt,
                                   i.TotalPrice,
                                   i.Address,
                                   i.City,
                                   i.Department,
                                   p.FirstName,
                                   p.Email
                               })
                              .AsEnumerable()
                              .Select(i => new InvoiceDto
                              {
                                  IdInvoice = i.IdInvoice,
                                  RegisteredAt = i.RegisteredAt,
                                  TotalPriceProduct = (decimal)Convert.ToDouble(i.TotalPrice),
                                  ProductsAddress = i.Address,
                                  ProductsCity = i.City,
                                  ProductsDepartment = i.Department,
                                  FirstName = i.FirstName,
                                  Email = i.Email
                              })
                              .FirstOrDefault();

                if (invoice != null)
                {
                    invoice.Products = (from ip in db.INVOICE_PRODUCT
                                        join prod in db.PRODUCT on ip.IdProduct equals prod.IdProduct
                                        where ip.IdInvoice == invoiceId
                                        select new
                                        {
                                            prod.Name,
                                            ip.Quantity,
                                            ip.UnitPrice
                                        })
                   .AsEnumerable() // Cargar en memoria antes de cálculos
                   .Select(ip => new ProductSelectionDto
                   {
                       NameProduct = ip.Name,
                       Quantity = ip.Quantity,
                       UnitPrice = (decimal)ip.UnitPrice,
                       TotalPriceProduct = (decimal)(ip.Quantity * ip.UnitPrice) // Calcular en C#
                   })
                   .ToList();
                }
                return invoice;
            }
        }

        public InvoiceDto GetInvoiceDetailDelivery(int? invoiceId)
        {
            using (var db = new BD_14_02Entities())
            {
                var invoice = (from i in db.INVOICE
                               join p in db.PERSON on i.IdPerson equals p.IdPerson
                               where i.IdInvoice == invoiceId
                               select new
                               {
                                   i.IdInvoice,
                                   i.RegisteredAt,
                                   i.TotalPrice,
                                   i.Address,
                                   i.City,
                                   i.Department,
                                   p.FirstName,
                                   p.Email
                               })
                              .AsEnumerable()
                              .Select(i => new InvoiceDto
                              {
                                  IdInvoice = i.IdInvoice,
                                  RegisteredAt = i.RegisteredAt,
                                  TotalPriceProduct = Convert.ToDecimal(i.TotalPrice),
                                  ProductsAddress = i.Address,
                                  ProductsCity = i.City,
                                  ProductsDepartment = i.Department,
                                  FirstName = i.FirstName,
                                  Email = i.Email
                              })
                              .FirstOrDefault();

                if (invoice != null)
                {
                    invoice.Products = (from ip in db.INVOICE_PRODUCT
                                        join prod in db.PRODUCT on ip.IdProduct equals prod.IdProduct
                                        where ip.IdInvoice == invoiceId
                                        select new
                                        {
                                            prod.Name,
                                            ip.Quantity,
                                            ip.UnitPrice
                                        })
                   .AsEnumerable()
                   .Select(ip => new ProductSelectionDto
                   {
                       NameProduct = ip.Name,
                       Quantity = ip.Quantity,
                       UnitPrice = Convert.ToDecimal(ip.UnitPrice),
                       TotalPriceProduct = Convert.ToDecimal(ip.Quantity * ip.UnitPrice)
                   })
                   .ToList();
                }
                return invoice;
            }
        }

    }
}