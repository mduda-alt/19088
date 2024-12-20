namespace projekt.Models
{
    public class Borrow
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; } // Relacja z książką
        public int UserId { get; set; }
        public User User { get; set; } // Relacja z użytkownikiem
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

}
