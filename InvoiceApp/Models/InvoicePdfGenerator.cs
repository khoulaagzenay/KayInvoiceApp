using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using InvoiceApp.Models;

namespace InvoiceApp.Services
{
    public class InvoicePdfGenerator
    {
        public static byte[] Generate(Invoice invoice, ApplicationUser user)
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var accentColor = Colors.Blue.Medium;
            var lightGray = Colors.Grey.Lighten3;
            var stream = new MemoryStream();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);

                    page.Header().BorderBottom(1).BorderColor(accentColor).PaddingBottom(10).Row(row =>
                    {
                        row.ConstantColumn(100).Height(60).Image("wwwroot/images/logo.png", ImageScaling.FitArea);

                        row.RelativeColumn().AlignLeft().Column(col =>
                        {
                            col.Item().Text(user.CompanyName).Bold().FontSize(18).FontColor(accentColor);
                            col.Item().Text($"SIREN : {user.SIREN}");
                            col.Item().Text(user.Address);
                            col.Item().Text(user.Email);
                        });

                        row.ConstantColumn(200).AlignRight().Column(col =>
                        {
                            col.Item().Text($"Facture n° {invoice.InvoiceNumber}")
                                .Bold().FontSize(16).FontColor(accentColor);
                            col.Item().Text($"Date : {invoice.InvoiceDate:dd/MM/yyyy}");
                        });
                    });

                    page.Content().PaddingVertical(25).Column(col =>
                    {
                        col.Spacing(15);

                        col.Item().Row(row =>
                        {
                            row.RelativeColumn().Border(1).BorderColor(lightGray).Padding(10).Column(inner =>
                            {
                                inner.Item().Text("Client").Bold().FontColor(accentColor).FontSize(13);
                                inner.Item().LineHorizontal(0.5f).LineColor(lightGray);
                                inner.Item().Text(invoice.Client?.Name ?? "");
                                inner.Item().Text(invoice.Client?.Address ?? "");
                                inner.Item().Text(invoice.Client?.Email ?? "");
                            });

                            row.ConstantColumn(20); 

                            row.RelativeColumn().Border(1).BorderColor(lightGray).Padding(10).Column(inner =>
                            {
                                inner.Item().Text("Informations").Bold().FontColor(accentColor).FontSize(13);
                                inner.Item().LineHorizontal(0.5f).LineColor(lightGray);
                                inner.Item().Text($"Date d’émission : {invoice.InvoiceDate:dd/MM/yyyy}");
                                inner.Item().Text($"Référence : {invoice.InvoiceNumber}");
                                inner.Item().Text($"Émise par : {user.CompanyName}");
                            });
                        });

                        if (!string.IsNullOrWhiteSpace(invoice.InvoiceNumber))
                        {
                            col.Item().PaddingTop(10).Text("Description").Bold().FontColor(accentColor).FontSize(13);
                            col.Item().Background(lightGray).Padding(8).Text(invoice.InvoiceNumber);
                        }

                        if (invoice.InvoiceLines != null && invoice.InvoiceLines.Any())
                        {
                            col.Item().PaddingTop(20).Text("Détails de la facture")
                                .Bold().FontSize(14).FontColor(accentColor);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4);  
                                    columns.RelativeColumn(2);  
                                    columns.RelativeColumn(2);  
                                    columns.RelativeColumn(2);  
                                    columns.RelativeColumn(2);  
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Background(accentColor).Padding(5)
                                        .Text("Désignation").Bold().FontColor(Colors.White);
                                    header.Cell().Background(accentColor).Padding(5)
                                        .Text("Quantité").Bold().FontColor(Colors.White);
                                    header.Cell().Background(accentColor).Padding(5)
                                        .Text("Prix Unitaire").Bold().FontColor(Colors.White);
                                    header.Cell().Background(accentColor).Padding(5)
                                        .Text("TVA").Bold().FontColor(Colors.White);
                                    header.Cell().Background(accentColor).Padding(5)
                                        .Text("Total Ligne").Bold().FontColor(Colors.White);
                                });

                                foreach (var item in invoice.InvoiceLines)
                                {
                                    var totalLigne = item.Quantity * item.UnitPrice * (1 + item.TaxRate / 100);

                                    table.Cell().BorderBottom(0.5f).BorderColor(lightGray).Padding(5).Text(item.Description);
                                    table.Cell().BorderBottom(0.5f).BorderColor(lightGray).Padding(5).Text(item.Quantity.ToString());
                                    table.Cell().BorderBottom(0.5f).BorderColor(lightGray).Padding(5).Text($"{item.UnitPrice:C}");
                                    table.Cell().BorderBottom(0.5f).BorderColor(lightGray).Padding(5).Text($"{item.TaxRate}%");
                                    table.Cell().BorderBottom(0.5f).BorderColor(lightGray).Padding(5).Text($"{totalLigne:C}");
                                }
                            });
                        }

                        col.Item().PaddingTop(20).AlignRight().Column(innerCol =>
                        {
                            innerCol.Item().Border(1).BorderColor(accentColor).Padding(10).Column(total =>
                            {
                                total.Item().AlignRight().Text($"Sous-total : {invoice.SubTotal:C}");
                                total.Item().AlignRight().Text($"TVA totale : {invoice.TotalTax:C}");
                                total.Item().LineHorizontal(1).LineColor(accentColor);
                                total.Item().AlignRight().Text($"Montant total : {invoice.TotalAmount:C}")
                                    .Bold().FontSize(14).FontColor(accentColor);
                            });
                        });
                    });

                    page.Footer().BorderTop(1).BorderColor(lightGray).AlignCenter().PaddingTop(10)
                        .Text(user.CompanyName + " - " + user.Address + " - " + user.SIREN)
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });

            document.GeneratePdf(stream);
            return stream.ToArray();
        }
    }
}
