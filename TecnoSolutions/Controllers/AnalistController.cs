using System.Data.SqlClient;
using System.Collections.Generic;
using TecnoSolutions.Models;
using TecnoSolutions.Dtos;
using System;
using System.Data;
using Dapper;
using System.Linq;
using TecnoSolutions.Repository;
using System.Web.Mvc;
using TechnoSolutions.Repositories;

public class ProductRepository
{
    private string connectionString = "Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true";

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

    public void AddProduct(PRODUCT product)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("INSERT INTO PRODUCT (Name, Stock, UnitPrice) VALUES (@Name, @Stock, @UnitPrice)", connection);
            command.Parameters.AddWithValue("@Name", product.Name);
            command.Parameters.AddWithValue("@Stock", product.Stock);
            command.Parameters.AddWithValue("@UnitPrice", product.UnitPrice);
            command.ExecuteNonQuery();
        }
    }
    public void DeleteProduct(int id)
    {
        using (SqlConnection connection = new SqlConnection("Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true"))
        {
            string sql = "DELETE FROM PRODUCT WHERE IdProduct = @IdProduct";
            var parameters = new { IdProduct = id };
            connection.Execute(sql, parameters);
        }
    }
    public List<PRODUCT> GetProductsByIds(List<int> ids)
        {
            using (SqlConnection db = new SqlConnection("Data Source= LEO; Initial Catalog= BD 14_02; Integrated Security=true"))
            {
                string query = "SELECT * FROM PRODUCT WHERE IdProduct IN @Ids";
                return db.Query<PRODUCT>(query, new { Ids = ids }).ToList();
            }
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

    //public List<PRODUCT> GetProductsByIds(List<int> ids)
    //{
    //    using (IDbConnection db = DatabaseHelper.GetConnection())
    //    {
    //        if (ids == null || !ids.Any())
    //            return new List<PRODUCT>();

    //        string query = "SELECT * FROM PRODUCT WHERE IdProduct IN @Ids";

    //        var parameters = new { Ids = ids };

    //        return db.Query<PRODUCT>(query, parameters).ToList();
    //    }
    //}
    public Product GetProductById(int id)
    {
        using (SqlConnection connection = new SqlConnection("Data Source=LEO; Initial Catalog=BD 14_02; Integrated Security=true"))
        {
            // Cambia @Id a @id para que coincida con el nombre del parámetro en el objeto anónimo
            return connection.QuerySingleOrDefault<Product>("SELECT * FROM PRODUCT WHERE IdProduct = @id", new { id });
        }
    }

}