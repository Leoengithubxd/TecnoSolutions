using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using TechnoSolutions.Dtos;
using TecnoSolutions.Dtos;
using TecnoSolutions.Models;
using TecnoSolutions.Repository;
using TechnoSolutions.Controllers;
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

        public void CreateInvoice(int userId,List<ProductSelectionDto> selectedProducts, string address, string department, string city) //Llenar tabla Invoice
        {
            using (var db = new BD_14_02Entities())
            {
                int lastInvoiceId = db.INVOICE.Any() ? db.INVOICE.Max(i => i.IdInvoice) : 0;
                foreach (var invoice in selectedProducts)
                {
                    lastInvoiceId++;
                    var invoiceProducts = new INVOICE
                    {
                        IdInvoice = lastInvoiceId,
                        IdPerson = userId,
                        IdProduct = invoice.IdProduct,
                        NameProduct = invoice.NameProduct,
                        Quantity = invoice.Quantity,
                        UnitPrice = (double)invoice.UnitPrice,
                        TotalPriceProduct = (double)(invoice.UnitPrice * invoice.Quantity),
                        Address =invoice.ProductsAddress,
                        Department = invoice.ProductsDepartment,
                        City = invoice.ProductsCity,
                    };
                    db.INVOICE.Add(invoiceProducts);
                }
                db.SaveChanges();
            }
        }
        }
}