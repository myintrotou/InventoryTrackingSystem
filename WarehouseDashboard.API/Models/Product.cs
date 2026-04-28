using System.ComponentModel.DataAnnotations;

namespace WarehouseDashboard.API.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public int? SupplierID { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
