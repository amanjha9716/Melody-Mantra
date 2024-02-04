using Microsoft.AspNetCore.Mvc;
using MelodyMantra.Models;
using System.Data.SqlClient;
using System.Data;
namespace MelodyMantra.Controllers
{
    public class MusicController : Controller
    {
      
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
            return View();
        }
        public IActionResult Payment()
        {
            ViewBag.ShowHeader = false;
            return View();
        }
		public IActionResult specific(int id)
		{
            SqlConnection con= new SqlConnection(GetConnectionString());
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "specificAlbum";
            cmd.Connection = con;
            SqlParameter pr1 = new SqlParameter("@albumId", id);
            cmd.Parameters.Add(pr1);
            SqlDataReader reader= cmd.ExecuteReader();
            Album? album=null;
                if (reader.Read())
            {
                 album= new Album
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

	}
}
