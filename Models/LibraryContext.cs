using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace projekt.Models
{
    public class LibraryContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Borrow> Borrows { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<IdentityUserRole<string>> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacja Borrow -> User
            modelBuilder.Entity<Borrow>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacja Category -> Books
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Books)
                .WithOne(b => b.Category)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Konfiguracja User
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasMaxLength(20)
                .IsRequired();

            // Konfiguracja pól w ApplicationUser
            modelBuilder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Server=localhost;Database=LibrarySystem;Trusted_Connection=True;Encrypt=False")
                .EnableSensitiveDataLogging() // Dla debugowania
                .LogTo(Console.WriteLine);   // Log zapytań do konsoli
        }

        private class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
        {
            public void Configure(EntityTypeBuilder<ApplicationUser> builder)
            {
                builder.Property(x => x.FirstName).HasMaxLength(30);
                builder.Property(x => x.LastName).HasMaxLength(30);
            }
        }
    }
}
