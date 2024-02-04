using Microsoft.AspNetCore.Mvc;
using System.Data;
using MelodyMantra.Models;
using System.Data.SqlClient;
using Newtonsoft.Json;
namespace MelodyMantra.Controllers
{
   
    public class UserController : Controller
    {
        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
        {
            _connectionString = GetConnectionString();
        }

        // GET: /Account/Register
        public IActionResult Registration()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(User user)
        {

            Console.WriteLine(user.Age+" "+user.IsAdmin);
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "RegisterUser";
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@isAdmin", user.IsAdmin);

                        command.Parameters.AddWithValue("@UserName", user.UserName);
                        command.Parameters.AddWithValue("@Password", user.Password);
                        command.Parameters.AddWithValue("@Name", user.Name);
                        command.Parameters.AddWithValue("@PhNo", user.PhNo);
                        command.Parameters.AddWithValue("@Age", user.Age);
                        command.Parameters.AddWithValue("@Image", user.Image);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Login");
            

            return View(user);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
		public IActionResult Login(string userName, string password)
		{
			User? user = AuthenticateUser(userName, password);

			if (user != null)
			{
				HttpContext.Session.SetString("UserName", user.UserName);
                HttpContext.Session.SetString("Name", user.Name);

                return RedirectToAction("Catalog", "Music");
			}

			// Authentication failed, redirect to the login page
			return RedirectToAction("Login");
		}


		// GET: /Account/Logout
		public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }



        private User AuthenticateUser(string userName, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "LoginUser";
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userName", userName);
                        command.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserName = reader["userName"] is DBNull ? string.Empty : reader["userName"].ToString(),
                                    Name = reader["name"] is DBNull ? string.Empty : reader["name"].ToString(),
                                    // Add other properties accordingly
                                };
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or print the exception details for debugging
                Console.WriteLine($"Exception in AuthenticateUser: {ex.Message}");
            }

            return null;
        }

        [HttpPost]
        public IActionResult AdminLogin(string userName, string password)
        {
            User? user = AuthenticateAdmin(userName, password);
            if(user!=null)
            Console.WriteLine(user.Name);
            if (user != null)
            {
                HttpContext.Session.SetString("userName", user.UserName);
                HttpContext.Session.SetString("Name", user.Name);

                return RedirectToAction("Index", "Admin");
            }

            // Authentication failed, redirect to the login page
            return RedirectToAction("Login");
        }
        private User AuthenticateAdmin(string userName, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string storedProcedureName = "LoginAdmin";
                    using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@userName", userName);
                        command.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    UserName = reader["userName"] is DBNull ? string.Empty : reader["userName"].ToString(),
                                    Name = reader["name"] is DBNull ? string.Empty : reader["name"].ToString(),
                                    // Add other properties accordingly
                                };
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or print the exception details for debugging
                Console.WriteLine($"Exception in AuthenticateUser: {ex.Message}");
            }

            return null;
        }

        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file.
            return "Data Source=G1-HLML114-L\\SQLEXPRESS;Initial Catalog=MelodyMantra;"
                + "Integrated Security=true;";
        }

    }
}
