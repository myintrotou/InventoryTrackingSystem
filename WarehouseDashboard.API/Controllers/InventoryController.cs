using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.API.Data;
using WarehouseDashboard.API.Models;
using WarehouseDashboard.API.Services;

namespace WarehouseDashboard.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly WarehouseContext _context;
        private readonly IPdfReportService _pdfService;

        public InventoryController(WarehouseContext context, IPdfReportService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Supplier)
                .Select(p => new ProductDto {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    StockQuantity = p.StockQuantity,
                    ReorderLevel = p.ReorderLevel,
                    SupplierName = p.Supplier != null ? p.Supplier.SupplierName : "No Supplier"
                })
                .ToListAsync();
        }

        [HttpPost("add-product")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] CreateProductDto dto)
        {
            var product = new Product {
                ProductName = dto.ProductName,
                StockQuantity = dto.StockQuantity,
                ReorderLevel = dto.ReorderLevel
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("add-stock/{id}")]
        public async Task<IActionResult> AddStock(int id, [FromBody] int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.StockQuantity += quantity;
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpPost("take-stock/{id}")]
        public async Task<IActionResult> TakeStock(int id, [FromBody] int quantity)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            if (product.StockQuantity < quantity) return BadRequest("Not enough stock.");

            product.StockQuantity -= quantity;
            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpGet("report")]
        public async Task<IActionResult> DownloadReport()
        {
            var products = await _context.Products.ToListAsync();
            var pdfBytes = _pdfService.GenerateStockReport(products);
            return File(pdfBytes, "application/pdf", $"StockReport_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
