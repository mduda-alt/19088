using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using projekt.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Dodanie DbContext
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<LibraryContext>()
.AddDefaultTokenProviders()
.AddRoleManager<RoleManager<IdentityRole>>();

builder.Services.AddSingleton<IEmailSender, MockEmailSender>();

// Dodanie kontrolerów z widokami
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Dodanie Razor Pages

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();
    context.Database.EnsureCreated();
}

// Konfiguracja środowiska
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Middleware dla uwierzytelniania
app.UseAuthorization();  // Middleware dla autoryzacji

app.MapRazorPages(); // Mapowanie stron Razor
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Tworzenie roli "Reader"
    if (!await roleManager.RoleExistsAsync("Reader"))
    {
        var createRoleResult = await roleManager.CreateAsync(new IdentityRole("Reader"));
        if (!createRoleResult.Succeeded)
        {
            Console.WriteLine($"Nie udało się stworzyć roli 'Reader': {string.Join(", ", createRoleResult.Errors.Select(e => e.Description))}");
        }
        else
        {
            Console.WriteLine("Rola 'Reader' została stworzona.");
        }

    }
}

public class MockEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Mockowe wysyłanie e-maila (logowanie w konsoli)
        Console.WriteLine($"Sending email to {email} with subject: {subject}");
        return Task.CompletedTask;
    }
}
