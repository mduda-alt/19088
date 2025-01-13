using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projekt.Models;

namespace projekt.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BooksController(LibraryContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Books
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var libraryContext = _context.Books.Include(b => b.Category);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Books/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            // Pobierz datę wypożyczenia, jeśli książka jest wypożyczona
            if (book.Status == "Borrowed")
            {
                var borrow = await _context.Borrows
                    .Where(b => b.BookId == id)
                    .OrderByDescending(b => b.BorrowDate)
                    .FirstOrDefaultAsync();

                if (borrow != null)
                {
                    book.BorrowDate = borrow.BorrowDate;
                }
            }

            return View(book);
        }

        // GET: Books/Create
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", book.CategoryId);
            return View(book);
        }

        // GET: Books/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", book.CategoryId);
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Title,Author,ISBN,PublishedDate,Status,CategoryId")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "The book was updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", book.CategoryId);
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }

        [HttpPost]
        [Authorize(Roles = "Reader,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);

            // Sprawdź, czy książka istnieje i jest dostępna do wypożyczenia
            if (book == null || book.Status == "Borrowed")
            {
                TempData["Error"] = "Book is not available for borrowing.";
                return RedirectToAction(nameof(Index));
            }

            // Pobierz aktualnie zalogowanego użytkownika
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "You must be logged in to borrow a book.";
                return RedirectToAction(nameof(Index));
            }

            // Zmień status książki na "Borrowed"
            book.Status = "Borrowed";

            // Dodaj informację o wypożyczeniu
            var borrow = new Borrow
            {
                BookId = bookId,
                UserId = user.Id,
                BorrowDate = DateTime.Now
            };

            _context.Borrows.Add(borrow);

            try
            {
                await _context.SaveChangesAsync();
                TempData["Success"] = "Book has been successfully borrowed.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while borrowing the book.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}