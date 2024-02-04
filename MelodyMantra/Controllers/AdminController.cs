using MelodyMantra.Models;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using System.Diagnostics;
public class AdminController : Controller
{
    private readonly IConfiguration _configuration;

    public AdminController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult showAll()
    {
        List<Album> albums = GetAllAlbums();
        return View(albums);
    }
    public IActionResult Index()
    {
        List<Album> albums = GetAllAlbums();
        return View(albums);
    }
    public IActionResult Details(int id)
    {
        Album album = GetAlbumById(id);

        if (album == null)
        {
            return NotFound();
        }

        return View(album);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Album album)
    {
        if (ModelState.IsValid)
        {
            AddAlbum(album);
            return RedirectToAction("Index");
        }

        return View(album);
    }
    public IActionResult Edit(int id)
    {
        Album album = GetAlbumById(id);

        if (album == null)
        {
            return NotFound();
        }

        return View(album);
    }

    [HttpPost]
    public IActionResult Edit(Album album)
    {
        try
        {
            if (ModelState.IsValid)
            {
                UpdateAlbum(album);
                return RedirectToAction("Index");
            }

            return View(album);
        }
        catch (Exception ex)
        {
            // Log or handle the exception
            return View("Error");
        }
    }


    [HttpPost]
    public IActionResult Delete(int id)
    {
        DeleteAlbum(id);
        return RedirectToAction("Index");
    }

    private List<Album> GetAllAlbums()
    {
        List<Album> albums = new List<Album>();

        using (SqlConnection connection = new SqlConnection(GetConnectionString()))
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
                }
            }
        }

        return albums;
    }
    static private string GetConnectionString()
    {
        // To avoid storing the connection string in your code,
        // you can retrieve it from a configuration file.
        return "Data Source=G1-HLML114-L\\SQLEXPRESS;Initial Catalog=MelodyMantra;"
            + "Integrated Security=true;";
    }
    private Album GetAlbumById(int id)
    {
        Album? album = null;

        using (SqlConnection connection = new SqlConnection(GetConnectionString()))
        {
            connection.Open();

            string storedProcedureName = "specificAlbum";
            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter pr1 = new SqlParameter("@albumId", id);
                command.Parameters.Add(pr1);

                using (SqlDataReader reader = command.ExecuteReader())
                {
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
                }
            }
        }

        return album;
    }
  

    private void AddAlbum(Album album)
    {
        using (SqlConnection connection = new SqlConnection(GetConnectionString()))
        {
            connection.Open();

            string storedProcedureName = "AddNewAlbum";
            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters and set their values based on the 'album' object

                command.Parameters.AddWithValue("@albumName", album.AlbumName);
                command.Parameters.AddWithValue("@image", album.Image);
                command.Parameters.AddWithValue("@price", album.Price);
                command.Parameters.AddWithValue("@release_date", album.ReleaseDate);
                command.Parameters.AddWithValue("@genre", album.Genre);
                command.Parameters.AddWithValue("@artistName", album.ArtistName);

                command.ExecuteNonQuery();
            }
        }
    }

    private void UpdateAlbum(Album album)
    {
        using (SqlConnection connection = new SqlConnection(GetConnectionString()))
        {
            connection.Open();

            string storedProcedureName = "updateAlbum";
            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@albumId", album.AlbumId);
                command.Parameters.AddWithValue("@albumName", album.AlbumName);
                command.Parameters.AddWithValue("@image", album.Image);
                command.Parameters.AddWithValue("@price", album.Price);
                command.Parameters.AddWithValue("@release_date", album.ReleaseDate);
                command.Parameters.AddWithValue("@genre", album.Genre);
                command.Parameters.AddWithValue("@artistName", album.ArtistName);

                command.ExecuteNonQuery();
            }
        }
    }

    private void DeleteAlbum(int id)
    {
        using (SqlConnection connection = new SqlConnection(GetConnectionString()))
        {
            connection.Open();

            string storedProcedureName = "deleteAlbum";
            using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add parameter and set its value based on the 'id' parameter
                command.Parameters.AddWithValue("@albumId", id);

                command.ExecuteNonQuery();
            }
        }
    }
    
}
