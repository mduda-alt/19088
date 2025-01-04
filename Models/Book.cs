using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projekt.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Required]
        [StringLength(13)]
        public string ISBN { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }

        public string Status { get; set; } = "Available";

        public string ShortDescription { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        public Category Category { get; set; } 

        [NotMapped]
        public DateTime? BorrowDate { get; set; }
    }

}
