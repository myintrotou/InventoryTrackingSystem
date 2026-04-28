using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using WarehouseDashboard.API.Models;

namespace WarehouseDashboard.API.Services
{
    public interface IPdfReportService
    {
        byte[] GenerateStockReport(List<Product> products);
    }

    public class PdfReportService : IPdfReportService
    {
        public byte[] GenerateStockReport(List<Product> products)
        {
            // QuestPDF requires a license configuration for commercial use, 
            // but Community is free for many use cases.
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Warehouse Inventory Report").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{DateTime.Now:MMMM dd, yyyy}");
                        });
                    });

                    page.Content().PaddingVertical(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(50); // ID
                            columns.RelativeColumn();   // Name
                            columns.ConstantColumn(80); // Quantity
                            columns.ConstantColumn(80); // Reorder Level
                            columns.ConstantColumn(100); // Status
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("ID");
                            header.Cell().Element(CellStyle).Text("Product Name");
                            header.Cell().Element(CellStyle).Text("Quantity");
                            header.Cell().Element(CellStyle).Text("Reorder");
                            header.Cell().Element(CellStyle).Text("Status");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                            }
                        });

                        foreach (var item in products)
                        {
                            table.Cell().Element(Padding).Text(item.ProductID.ToString());
                            table.Cell().Element(Padding).Text(item.ProductName);
                            table.Cell().Element(Padding).Text(item.StockQuantity.ToString());
                            table.Cell().Element(Padding).Text(item.ReorderLevel.ToString());
                            
                            var status = item.StockQuantity <= item.ReorderLevel ? "LOW STOCK" : "In Stock";
                            var color = item.StockQuantity <= item.ReorderLevel ? Colors.Red.Medium : Colors.Green.Medium;
                            
                            table.Cell().Element(Padding).Text(status).FontColor(color).SemiBold();

                            static IContainer Padding(IContainer container) => container.PaddingVertical(5);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
