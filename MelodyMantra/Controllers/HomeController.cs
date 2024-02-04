using MelodyMantra.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Data.SqlClient;
namespace MelodyMantra.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.ShowHeader = true;
            string strcon=GetConnectionString();
            SqlConnection con = new SqlConnection(strcon);

            con.Open();
            Console.WriteLine(con.State);
                return View();
        }
        static private string GetConnectionString()
        {
            // To avoid storing the connection string in your code,
            // you can retrieve it from a configuration file.
            return "Data Source=G1-HLML114-L\\SQLEXPRESS;Initial Catalog=MelodyMantra;"
                + "Integrated Security=true;";
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
