using Microsoft.AspNetCore.Mvc;
using MelodyMantra.Models;
using System.Data.SqlClient;
using System.Data;
namespace MelodyMantra.Controllers
{
    public class MusicController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            ViewBag.ShowHeader = true;
            List<Album> albums = new List<Album>();

            string connectionString = GetConnectionString();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string storedProcedureName = "showAllAlbum";
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Album album = new Album
                            {
                                AlbumId = Convert.ToInt32(reader["albumId"]),
                                AlbumName = reader["albumName"].ToString(),
                                Image = reader["image"].ToString(),
                                Price = Convert.ToInt32(reader["price"]),
                                ReleaseDate = Convert.ToDateTime(reader["release_date"]),
                                Genre = reader["genre"].ToString(),
                                ArtistName = reader["artistName"].ToString(),
                            };

                            albums.Add(album);
                        }
                        reader.Close();
                    }
                }
            }

            return View(albums);
        }
        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file.
            return "Data Source=G1-HLML114-L\\SQLEXPRESS;Initial Catalog=MelodyMantra;"
                + "Integrated Security=true;";
        }
        public IActionResult Cart()
        {
            ViewBag.ShowHeader = false;
            List<CartItem> cartItems = new List<CartItem>();
            List<Album> albums = new List<Album>();

            string strcon = GetConnectionString();

            using (SqlConnection con = new SqlConnection(strcon))
            {
                con.Open();

                // Fetch cart items
                using (SqlCommand cmd = new SqlCommand("GetCartItems", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@userName", HttpContext.Session.GetString("userName")));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CartItem item = new CartItem
                            {
                                albumId = Convert.ToInt32(reader["albumId"]),
                                qty = Convert.ToInt32(reader["qty"]),
                            };
                            cartItems.Add(item);
                        }
                    }
                }
                Console.WriteLine(cartItems[0].userName);
                // Fetch details for each album in the cart
                foreach (var item in cartItems)
                {
                    using (SqlCommand cmnd = new SqlCommand("specificAlbum", con))
                    {
                        cmnd.CommandType = CommandType.StoredProcedure;
                        cmnd.Parameters.Add(new SqlParameter("@albumId", item.albumId));

                        using (SqlDataReader reader = cmnd.ExecuteReader())
                        {
                            Album? album = null;
                            if (reader.Read())
                            {
                                album = new Album
                                {
                                    AlbumId = Convert.ToInt32(reader["albumId"]),
                                    AlbumName = reader["albumName"].ToString(),
                                    Image = reader["image"].ToString(),
                                    Price = Convert.ToInt32(reader["price"]),
                                    ReleaseDate = Convert.ToDateTime(reader["release_date"]),
                                    Genre = reader["genre"].ToString(),
                                    ArtistName = reader["artistName"].ToString(),
                                };
                                albums.Add(album);
                            }
                        }
                    }
                }
            }

            return View(albums);
        }

        public IActionResult Payment(int amount )
        {
            ViewBag.ShowHeader = false;
            ViewData["amount"] = amount;
            return View();
        }
        public IActionResult specific(int id)
        {
            SqlConnection con = new SqlConnection(GetConnectionString());
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "specificAlbum";
            cmd.Connection = con;
            SqlParameter pr1 = new SqlParameter("@albumId", id);
            cmd.Parameters.Add(pr1);
            SqlDataReader reader = cmd.ExecuteReader();
            Album? album = null;
            if (reader.Read())
            {
                album = new Album
                {
                    AlbumId = Convert.ToInt32(reader["albumId"]),
                    AlbumName = reader["albumName"].ToString(),
                    Image = reader["image"].ToString(),
                    Price = Convert.ToInt32(reader["price"]),
                    ReleaseDate = Convert.ToDateTime(reader["release_date"]),
                    Genre = reader["genre"].ToString(),
                    ArtistName = reader["artistName"].ToString(),
                };
            }

            reader.Close(); // Close the SqlDataReader
            con.Close();

            return View(album);

        }
        /*cart functionality */
     
        public IActionResult AddToCart(int id)
        {
            
            CartItem temp = new CartItem();
            temp.qty = 1;
            temp.userName = HttpContext.Session.GetString("userName");
            temp.albumId = id;
            AddCart(temp);
            Console.WriteLine(temp);
            return View("Cart");
        }
        private void AddCart(CartItem temp)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                string storedProcedureName = "AddToCart";
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters and set their values based on the 'album' object

                    command.Parameters.AddWithValue("@albumId", temp.albumId);
                    command.Parameters.AddWithValue("@userName", temp.userName);
                    command.Parameters.AddWithValue("@qty", temp.qty);
                    command.ExecuteNonQuery();
                }
            }
        }


    }

    }
