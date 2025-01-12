using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace projekt.Models
{
public class ApplicationUser : IdentityUser
{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; } = "Reader"; // Domyślna wartość dla nowych użytkowników
    }
}
