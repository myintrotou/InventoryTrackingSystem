namespace WarehouseDashboard.API.Models
{
    public class ProductDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
        public string? SupplierName { get; set; }
    }

    public class CreateProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int ReorderLevel { get; set; }
    }
}
