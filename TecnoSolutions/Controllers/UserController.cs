using TecnoSolutions.Models;
using TecnoSolutions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;
using System.Web.Services.Description;

namespace TecnoSolutions.Views.User
{
    public class UserController : Controller
    {

        static string cadena = "Data Source= LEO ; Initial Catalog= BD 14_02 ; Integrated Security=true";

        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult SignUpEmployees()
        {
            return View();
        }
        
        [HttpPost]
        
            public ActionResult SignUpEmployees(UserDto user)
            {
                bool registered;
                string message;

                if (user.Password == user.VerifyPassword)
                {
                    user.Password = ConvertirSha256(user.Password);
                }
                else
                {
                    ViewData["Mensaje"] = "The passwords must be the same. Try again.";
                    return View();
                }

              

                using (SqlConnection cn = new SqlConnection(cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_UserRegisteredEmployees", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdRole", user.IdRole);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Document", user.Document);
                    cmd.Parameters.AddWithValue("@Phone", user.Phone);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@Department", user.Department);
                    cmd.Parameters.AddWithValue("@City", user.City);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Arl", user.Arl);
                    cmd.Parameters.AddWithValue("@Eps", user.Eps);
                    
                    cmd.Parameters.AddWithValue("@Position", user.Position);

                    SqlParameter outputRegistered = new SqlParameter("@Registered", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                    SqlParameter outputMessage = new SqlParameter("@Message", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(outputRegistered);
                    cmd.Parameters.Add(outputMessage);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    registered = Convert.ToBoolean(outputRegistered.Value);
                    message = outputMessage.Value.ToString();
                }

                ViewData["Message"] = message;

                if (registered)
                {
                    return RedirectToAction("Login", "User");
                }

                return View();
            }
            public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(UserDto user)
        {
            bool registered;
            string message;
            if (user.Password == user.VerifyPassword)
            {
                user.Password = ConvertirSha256(user.Password);
            } else
            {
                ViewData["Mensaje"] = "The passwords must be the same. Try again.";
                return View();
            }
            int IdRol = 1;
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_UserRegistered", cn);
                cmd.Parameters.AddWithValue("FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("IdRol", IdRol);
                cmd.Parameters.AddWithValue("LastName", user.LastName);
                cmd.Parameters.AddWithValue("Document", user.Document);
                cmd.Parameters.AddWithValue("Phone", user.Phone);
                cmd.Parameters.AddWithValue("Address", user.Address);
                cmd.Parameters.AddWithValue("Department", user.Department);
                cmd.Parameters.AddWithValue("City", user.City);
                cmd.Parameters.AddWithValue("Email", user.Email);
                cmd.Parameters.AddWithValue("Password", user.Password);
                cmd.Parameters.Add("@Registered",SqlDbType.BigInt).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                cmd.ExecuteNonQuery();
                registered = true;
                //message= cmd.Parameters["Message"].Value.ToString();
            }
            //ViewData["Message"] = message;
            if (registered==true)
            {
                return RedirectToAction("Login", "User");
            }

            return View();
            
        }

        [HttpPost]
        public ActionResult Login(UserDto user)
        {
            user.Password = ConvertirSha256(user.Password);
            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_VerifyUser", cn);
                cmd.Parameters.AddWithValue("Email", user.Email);
                cmd.Parameters.AddWithValue("Password", user.Password);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                cmd.ExecuteNonQuery();
                user.IdPerson = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if (user.IdPerson != 0)
            {
                Session["IdUser"] = user.IdPerson;

                return RedirectToAction("SelectProducts","Product");
            }
            else
            {
                ViewData["Message"] = "User not found.";
                return View();
            }
        }
        public static string ConvertirSha256(string texto)
        {
            //using System.Text;
            //USAR LA REFERENCIA DE "System.Security.Cryptography"

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}