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
using Microsoft.Ajax.Utilities;
using System.Text.RegularExpressions;

namespace TecnoSolutions.Views.User
{
    public class UserController : Controller
    {
        static string cadena = "Data Source= LEO ;Initial Catalog=BD 14_02; Integrated Security=true"; //DESKTOP-MG8HU3J, Carlitos

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult AdminHome()
        {
            return View();
        }
        public ActionResult AssistantHome()
        {
            return View();
        }
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult AnalysticHome()
        {
            return View();
        }
        public ActionResult OrderManagement()
        {
            return View();
        }
        public ActionResult Users()
        {
            return View();
        }
        public ActionResult SignUpEmployees()
        {
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignUpEmployees(UserDto user)
        {
            if (string.IsNullOrWhiteSpace(user.Document) || user.Document.Length > 10)
            {
                ViewData["Message"] = "El documento no puede ser mayor a 10 caracteres";
                return View();
            }
            if (!Regex.IsMatch(user.Phone.Trim(), @"^\d{10}$"))
            {
                ViewData["Message"] = "El teléfono debe tener 10 dígitos";
                return View();
            }
            if (!user.Email.Trim().EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                ViewData["Message"] = "Solo se admiten cuentas @gmail.com";
                return View();
            }
            if (user.Password.Length < 5 || user.Password.Length > 15)
            {
                ViewData["Message"] = "La contraseña debe tener entre 5 y 15 caracteres";
                return View();
            }
            if (!user.Password.Any(char.IsUpper))
            {
                ViewData["Message"] = "La contraseña debe contener al menos 1 letra mayúscula";
                return View();
            }
            if (user.Password.Any(c => !char.IsLetterOrDigit(c)))
            {
                ViewData["Message"] = "La contraseña no puede contener caracteres especiales";
                return View();
            }
            if (user.Password != user.VerifyPassword)
            {
                ViewData["Message"] = "La contraseña y la confirmación no coinciden";
                return View();
            }

            user.Password = ConvertirSha256(user.Password);

            bool registered;
            string message;

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_UserRegisteredEmployees", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdRole", user.IdRole);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName.Trim());
                cmd.Parameters.AddWithValue("@LastName", user.LastName.Trim());
                cmd.Parameters.AddWithValue("@Document", user.Document.Trim());
                cmd.Parameters.AddWithValue("@Phone", user.Phone.Trim());
                cmd.Parameters.AddWithValue("@Address", user.Address.Trim());
                cmd.Parameters.AddWithValue("@Department", user.Department.Trim());
                cmd.Parameters.AddWithValue("@City", user.City.Trim());
                cmd.Parameters.AddWithValue("@Email", user.Email.Trim());
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@Arl", user.Arl.Trim());
                cmd.Parameters.AddWithValue("@Eps", user.Eps.Trim());
                cmd.Parameters.AddWithValue("@Position", user.Position.Trim());

                SqlParameter outputRegistered = new SqlParameter("@Registered", SqlDbType.Bit) { Direction = ParameterDirection.Output };
                SqlParameter outputMessage = new SqlParameter("@Message", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };

                cmd.Parameters.Add(outputRegistered);
                cmd.Parameters.Add(outputMessage);

                cn.Open();
                cmd.ExecuteNonQuery();

                registered = Convert.ToBoolean(outputRegistered.Value);
                message = outputMessage.Value.ToString();
            }

            if (registered)
            {
                return RedirectToAction("Login", "User");
            }

            ViewData["Message"] = message;
            return View();
        }



        [HttpPost]
        public ActionResult SignUp(UserDto user)
        {
            bool registered;
            string message;
            if (user.Document.Length < 0 || user.Document.Length > 10)
            {
                ViewData["Message"] = "Tu documento no puede ser mayor a 10 digitos";
                return View();
            }
            if (!Regex.IsMatch(user.Phone, "^\\d{10}$"))
            {
                ViewData["Message"] = "Tu Telefono debe tener 10 digitos";
                return View();
            }
            if (!user.Email.EndsWith("@gmail.com"))
            {
                ViewData["Message"] = "Solo se admiten cuentas (@gmail.com)";
                return View();
            }
            if (user.Password.Length < 5 || user.Password.Length > 15)
            {
                ViewData["Message"] = "La Contraseña debe tener entre 5 y 15 caracteres";
                return View();
            }
            if (!user.Password.Any(char.IsUpper))
            {
                ViewData["Message"] = "La Contraseña debe tener 1 letra en mayuscula";
                return View();
            }
            else
            {
                if (user.Password.Any(c => !char.IsLetterOrDigit(c)))
                {
                    ViewData["Message"] = "La Contraseña no puede tener caracteres especiales";
                    return View();
                }
                if ((user.Password != user.VerifyPassword))
                {
                    ViewData["Mensaje"] = "La Contraseña y la Confirmacion no coinciden";
                    return View();
                }
                else
                {
                    user.Password = ConvertirSha256(user.Password);
                }
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
                cmd.Parameters.Add("@Registered", SqlDbType.BigInt).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Message", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                cmd.ExecuteNonQuery();
                registered = true;
                message = cmd.Parameters["@Message"].Value.ToString();
            }
            ViewData["Message"] = message;
            if (registered == true)
            {
                return RedirectToAction("Login", "User");
            }
            return View();
        }

        public ActionResult Logout()
        {
            // Limpia todas las variables de sesión
            Session.Clear();
            Session.Abandon();

            // Redirige al login
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult Login(UserDto user)
        {
            // Encriptar la contraseña
            user.Password = ConvertirSha256(user.Password);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                cn.Open();

                // Primera consulta: Obtener IdPerson usando el SP
                using (SqlCommand cmd = new SqlCommand("sp_VerifyUser", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);

                    // sp_VerifyUser devuelve solo IdPerson
                    object result = cmd.ExecuteScalar();
                    user.IdPerson = result != null ? Convert.ToInt32(result) : 0;
                }
              
                // Si se encontró el usuario, se consulta el rol
                if (user.IdPerson != 0)
                {
                    using (SqlCommand cmdRole = new SqlCommand("SELECT IdRole FROM PERSON WHERE IdPerson = @IdPerson", cn))
                    {
                        cmdRole.Parameters.AddWithValue("@IdPerson", user.IdPerson);
                        object roleResult = cmdRole.ExecuteScalar();
                        user.IdRole = roleResult != null ? Convert.ToInt32(roleResult) : 0;
                    }
                }
            }

            // Si se encontró el usuario, guardamos en Session y redirigimos según el rol
            if (user.IdPerson != 0)
            {
                Session["UserId"] = user.IdPerson;
                Session["UserRole"] = user.IdRole;

                // Ejemplo: si el rol es 6, se redirige a AdminHome, de lo contrario a UserHome
                if (user.IdRole == 6)
                    return RedirectToAction("AdminHome", "User");
                Session["IdUser"] = user.IdPerson;
                if (user.IdRole == 5)
                    return RedirectToAction("AssistantHome", "User");

                if (user.IdRole == 3)
                    return RedirectToAction("AnalysticHome", "User");
                if (user.IdRole == 2)
                    return RedirectToAction("EmployeeHome", "Quote");
                if (user.IdRole == 4)
                    return RedirectToAction("DeliveryHome", "Product");
                else
                    return RedirectToAction("Home", "User");
            }

            // Si no se encontró el usuario, se muestra el mensaje
            ViewData["Message"] = "User not found.";
            return View();
        }




        public static string ConvertirSha256(string texto)
        {
            if (string.IsNullOrEmpty(texto))
            {
                throw new ArgumentNullException(nameof(texto), "Insert password");
            }

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())  // SHA256.Create() en lugar de SHA256Managed
            {
                byte[] result = hash.ComputeHash(Encoding.UTF8.GetBytes(texto));

                foreach (byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }
            }
            return Sb.ToString();
        }
    }
}