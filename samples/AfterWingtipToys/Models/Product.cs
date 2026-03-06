namespace WingtipToys.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? CategoryID { get; set; }
        public Category? Category { get; set; }
    }
}
