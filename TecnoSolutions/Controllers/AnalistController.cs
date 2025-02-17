using System.Data.SqlClient;
using System.Collections.Generic;
using TecnoSolutions.Models;
using TecnoSolutions.Dtos;
using System;

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

    public PRODUCT GetProductById(int IdProduct)
    {
        PRODUCT product = null;
        using (SqlConnection connection = new SqlConnection(connectionString))
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
        using (SqlConnection connection = new SqlConnection(connectionString))
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
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("DELETE FROM Products WHERE IdProduct = @IdProduct", connection);
            command.Parameters.AddWithValue("@IdProduct", IdProduct);
            command.ExecuteNonQuery();
        }
    }

    internal void EliminarUsuario(int idUsuario)
    {
        throw new NotImplementedException();
    }
}