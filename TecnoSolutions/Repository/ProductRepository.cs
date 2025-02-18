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
        public void UpdateProduct(PRODUCT product)
        {
            using (IDbConnection db = DatabaseHelper.GetConnection())
            {
                string sql = "UPDATE PRODUCT SET Name = @Name, Stock = @Stock, UnitPrice = @UnitPrice WHERE IdProduct = @IdProduct";
                var parameters = new
                {
                    IdProduct = product.IdProduct, // Asegúrate de que el ID esté correctamente asignado
                    Name = product.Name,
                    Stock = product.Stock,
                    UnitPrice = product.UnitPrice
                };

                int rowsAffected = db.Execute(sql, parameters); // Ejecuta la consulta y obtiene el número de filas afectadas

                // Agrega un log para verificar cuántas filas se actualizaron
                Console.WriteLine($"Rows affected: {rowsAffected}");
            }
        }
        public List<PRODUCT> GetProductsByIds(List<int> ids)
        {
            using (IDbConnection db = DatabaseHelper.GetConnection())
            {
                string query = "SELECT * FROM PRODUCT WHERE IdProduct IN @Ids";
                return db.Query<PRODUCT>(query, new { Ids = ids }).ToList();
            }
        }


    }
    public class ProductPersonRepository
    {
        public string connectionString = "Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true";
        
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
            var product = new List<PRODUCT>();
            using (SqlConnection connection = new SqlConnection("Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true"))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("SELECT * FROM PRODUCT", connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    product.Add(new PRODUCT
                    {
                        IdProduct = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Stock = (int)reader["Stock"],
                        UnitPrice = (double)reader["UnitPrice"]
                    });
                }
            }
            return product  ;
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
    }
}