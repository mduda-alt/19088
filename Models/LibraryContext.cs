using Microsoft.EntityFrameworkCore;
using projekt.Models;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Borrow> Borrows { get; set; }
    public DbSet<Category> Categories { get; set; }
}
