﻿namespace projekt.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

}
