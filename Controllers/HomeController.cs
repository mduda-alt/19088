using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using projekt.Models;
using System.Linq;

namespace projekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LibraryContext _context;

        // Konstruktor przyjmujący obie zależności
        public HomeController(ILogger<HomeController> logger, LibraryContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult TestDatabaseConnection()
        {
            // Pobierz książki z bazy danych
            var books = _context.Books.ToList();

            // Wyświetl dane w widoku lub zwróć jako JSON dla testów
            return Json(books);
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
