using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TechnoSolutions.Dtos;
using TecnoSolutions.Dtos;
using TecnoSolutions.Models;
namespace TechnoSolutions.Repositories
{
    public class ProductRepository
    {
        public string connectionString = "Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true";

        public List<ProductSelectionDto> GetAllProducts()
        {
            using (var db = new BD_14_02Entities())
            {
                var products = db.PRODUCT
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
            using (var db = new BD_14_02Entities())
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

        //CRUD EMPIEZA ACA
        public List<PRODUCT> GetAllProducts()
        {
            var products = new List<PRODUCT>();
            using (SqlConnection connection = new SqlConnection("Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM PRODUCT", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    products.Add(new PRODUCT
                    {
                        IdProduct = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Stock = (int)reader["Stock"],
                        UnitPrice = (double)reader["UnitPrice"]
                    });
                }
            }
            return products;
        }

        public void AddProduct(PRODUCT product)
        {
            using (SqlConnection connection = new SqlConnection("Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("INSERT INTO PRODUCT (Name, Stock, UnitPrice) VALUES (@Name, @Stock, @UnitPrice)", connection);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Stock", product.Stock);
                command.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                command.ExecuteNonQuery();
            }
        }

        public PRODUCT GetProductById(int IdProduct)
        {
            PRODUCT product = null;
            using (SqlConnection connection = new SqlConnection("Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM PRODUCT WHERE IdProduct = @IdProduct", connection);
                command.Parameters.AddWithValue("@IdProduct", IdProduct);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    product = new PRODUCT
                    {
                        IdProduct = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Stock = (int)reader["Stock"],
                        UnitPrice = (double)reader["Description"]
                    };
                }
            }
            return product;
        }

        public void UpdateProduct(PRODUCT product)
        {
            using (SqlConnection connection = new SqlConnection("Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("UPDATE Products SET Name = @Name, @Stock = @UnitPrice, UnitPrice = @UnitPrice WHERE IdProduct = @IdProduct", connection);
                command.Parameters.AddWithValue("@IdProduct", product.IdProduct);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Price", product.Stock);
                command.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteProduct(int IdProduct)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=LEO; Initial Catalog=BD 14_02; Integrated Security=true"))
            {
                connection.Open();

                // Cambia la consulta para eliminar el producto por IdProduct
                SqlCommand command = new SqlCommand("DELETE FROM PRODUCT WHERE IdProduct = @IdProduct", connection);
                command.Parameters.AddWithValue("@IdProduct", IdProduct); // Usa el IdProduct para eliminar el producto

                command.ExecuteNonQuery(); // Ejecuta la consulta
            }
        }

    }
}