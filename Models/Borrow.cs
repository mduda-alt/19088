namespace projekt.Models
{
    public class Borrow
    {
        public int BorrowId { get; set; }

        // Relacja do Book
        public int BookId { get; set; }
        public Book Book { get; set; }

        // Relacja do ApplicationUser (User)
        public string UserId { get; set; } // Klucz obcy
        public ApplicationUser User { get; set; }

        public DateTime BorrowDate { get; set; }
    }
}