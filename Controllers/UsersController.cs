using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projekt.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace projekt.Controllers
{
    [Authorize(Roles = "Admin")] // Dostęp tylko dla administratorów
    public class UsersController : Controller
    {
        private readonly LibraryContext _context;

        public UsersController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,UserName,Role,Password")] ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                user.EmailConfirmed = true; // Domyślne ustawienie
                user.SecurityStamp = Guid.NewGuid().ToString(); // Wymagane przez Identity
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,Email,UserName,Role")] ApplicationUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Znajdź użytkownika w bazie
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Zaktualizuj dane użytkownika
                    existingUser.FirstName = user.FirstName;
                    existingUser.LastName = user.LastName;
                    existingUser.Email = user.Email;
                    existingUser.UserName = user.UserName;
                    existingUser.Role = user.Role; // Zaktualizuj kolumnę Role w AspNetUsers

                    // Specyficzne ID dla ról
                    string adminRoleId = "49740416-AEF2-40D7-BBAD-D7499A6601FB";
                    string readerRoleId = "C729FB7C-ABDD-407F-A80C-DDD39CA748CC";

                    // Pobierz istniejące przypisanie roli użytkownika
                    var userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);

                    if (user.Role == "Admin")
                    {
                        if (userRole != null)
                        {
                            // Usuń istniejące przypisanie roli
                            _context.UserRoles.Remove(userRole);
                            await _context.SaveChangesAsync();
                        }

                        // Dodaj nowe przypisanie roli Admin
                        _context.UserRoles.Add(new IdentityUserRole<string>
                        {
                            UserId = id,
                            RoleId = adminRoleId
                        });
                    }
                    else if (user.Role == "Reader")
                    {
                        if (userRole != null)
                        {
                            // Usuń istniejące przypisanie roli
                            _context.UserRoles.Remove(userRole);
                            await _context.SaveChangesAsync();
                        }

                        // Dodaj nowe przypisanie roli Reader
                        _context.UserRoles.Add(new IdentityUserRole<string>
                        {
                            UserId = id,
                            RoleId = readerRoleId
                        });
                    }
                    else
                    {
                        // Usuń przypisanie roli, jeśli użytkownik nie ma przypisanej żadnej roli
                        if (userRole != null)
                        {
                            _context.UserRoles.Remove(userRole);
                        }
                    }

                    // Zapisz zmiany w AspNetUsers
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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

            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
