using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;

namespace FinalProjectWinUi.Services
{
    public static class ReceiptService
    {
        public static string GenerateReceipt(int paymentId, int planId, int customerId,
            decimal amount, DateTime date, string method)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var fileName = $"Payment_{paymentId}.pdf";
            var folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Receipts");
            Directory.CreateDirectory(folderPath);
            var fullPath = Path.Combine(folderPath, fileName);

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .AlignCenter()
                        .Text("Payment Receipt")
                        .Bold().FontSize(24);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Item().Text($"Payment ID: {paymentId}");
                            column.Item().Text($"Plan ID: {planId}");
                            column.Item().Text($"Customer ID: {customerId}");
                            column.Item().Text($"Amount: {amount:C}");
                            column.Item().Text($"Date: {date:yyyy-MM-dd HH:mm}");
                            column.Item().Text($"Method: {method}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x => x.CurrentPageNumber());
                });
            }).GeneratePdf(fullPath);

            return fullPath;
        }
    }
}