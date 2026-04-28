namespace WarehouseDashboard.API.Models
{
    public class Supplier
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
