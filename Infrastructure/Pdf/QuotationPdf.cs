using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using CatalogWebApi.Data;
using CatalogWebApi.DTO;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CatalogWebApi.Infrastructure.Pdf
{
    public class QuotationPdf : IDocument
    {
        public QuotationPdfDTO _quotation { get; }
        
        public QuotationPdf(QuotationPdfDTO quotation)
        {
            _quotation = quotation;
        }

        public DocumentMetadata GetMetadata() => new DocumentMetadata()
        {
            Title = "Cotizacion de Dotaciones.",
            Author = "SEGURIDAD INDUSTRIAL Y SUMINISTROS S.A.S."
        };

        public DocumentSettings GetSettings() => new DocumentSettings()
        {
            CompressDocument = true,
        };

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item()
                            .Text($"Cotizacion")
                            .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                        column.Item().Text(text =>
                        {
                            text.Span("Fecha de cotizacion: ").SemiBold();
                            text.Span($"{_quotation.createdAt.ToShortDateString()}");
                        });
                        column.Item().Text(text =>
                        {
                            text.Span("Cliente: ").SemiBold();
                            text.Span($"{_quotation.clientName}");
                        });
                        column.Item().Text(text =>
                        {
                            text.Span("Empresa: ").SemiBold();
                            text.Span($"{_quotation.clientCompany}");
                        });
                        column.Item().Text(text =>
                        {
                            text.Span("Email: ").SemiBold();
                            text.Span($"{_quotation.clientEmail}");
                        });
                        column.Item().Text(text =>
                        {
                            text.Span("Telefono: ").SemiBold();
                            text.Span($"{_quotation.clientPhone}");
                        });
                    });

                    row.RelativeItem().Height(100).Width(240).Image(_quotation.companyInfo);
                });



                page.Content().Column(col =>
                {
                    col.Spacing(15);

                    col.Item().Table(table =>
                    {
                        // === COLUMNAS ===
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(70); // Imagen
                            columns.RelativeColumn();   // Nombre
                            columns.RelativeColumn();   // Talla
                            columns.RelativeColumn();   // Color
                            columns.ConstantColumn(60); // Cantidad
                        });

                        // === HEADER ===
                        table.Header(header =>
                        {
                            header.Cell().Element(BlockHeader).Text("Imagen").SemiBold().AlignCenter();
                            header.Cell().Element(BlockHeader).Text("Nombre").SemiBold().AlignCenter();
                            header.Cell().Element(BlockHeader).Text("Talla").SemiBold().AlignCenter();
                            header.Cell().Element(BlockHeader).Text("Color").SemiBold().AlignCenter();
                            header.Cell().Element(BlockHeader).Text("Cantidad").SemiBold().AlignCenter();
                        });

                        // === FILAS ===
                        foreach (var item in _quotation.Items)
                        {
                            table.Cell().Element(BlockCell).Image(item.Images).FitArea();

                            table.Cell().Element(BlockCell).Text($"{item.EndowmentName}").AlignCenter();
                            table.Cell().Element(BlockCell).Text($"{item.SizeName}").AlignCenter();
                            table.Cell().Element(BlockCell).Text($"{item.ColorName}").AlignCenter();
                            table.Cell().Element(BlockCell).Text(item.Quantity.ToString()).AlignCenter();
                        }
                    });
                });


                // === ESTILOS ===

                IContainer BlockHeader(IContainer container) =>
                    container
                        .Padding(5)
                        .BorderBottom(1)
                        .BorderColor(Colors.Grey.Lighten2);

                IContainer BlockCell(IContainer container) =>
                    container
                        .Padding(10)
                        .BorderBottom(0.5f)
                        .BorderColor(Colors.Grey.Lighten3);

            });
        }
    }
}
