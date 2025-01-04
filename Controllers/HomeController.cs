using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using projekt.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace projekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibraryContext _context;

        // Konstruktor przyjmuj¹cy obie zale¿noœci
        public HomeController(ILogger<HomeController> logger, LibraryContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult TestDatabaseConnection()
        {
            // Pobierz ksi¹¿ki z bazy danych
            var books = _context.Books.ToList();

            // Wyœwietl dane w widoku lub zwróæ jako JSON dla testów
            return Json(books);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult AdminPanel()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
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

