using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SPMD.Models;
using System.Collections.Generic;

namespace SPMD.Services
{
    public class PdfService
    {
        public byte[] GeneratePrescriptionPdf(Prescription prescription)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Inch);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("SPMD: Smart Prescription System").FontSize(20).SemiBold().FontColor("#0F172A");
                            col.Item().Text($"Prescription #{prescription.PrescriptionNumber}").FontSize(14).SemiBold();
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text($"Date: {prescription.IssuedAt:yyyy-MM-dd}");
                            col.Item().Text($"Status: {prescription.Status}").SemiBold();
                        });
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                    {
                        x.Spacing(10);

                        x.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Patient Details").SemiBold().FontColor("#475569");
                                c.Item().Text($"{prescription.Patient.FirstName} {prescription.Patient.LastName}");
                                c.Item().Text(prescription.Patient.Email ?? "N/A");
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Doctor Details").SemiBold().FontColor("#475569");
                                c.Item().Text($"Dr. {prescription.Doctor.FirstName} {prescription.Doctor.LastName}");
                                c.Item().Text(prescription.Doctor.Speciality ?? "N/A");
                            });
                        });

                        x.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        x.Item().Text("Medications").FontSize(14).SemiBold().FontColor("#1E293B");

                        x.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(30);
                                columns.RelativeColumn();
                                columns.ConstantColumn(100);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("#");
                                header.Cell().Element(CellStyle).Text("Medicine");
                                header.Cell().Element(CellStyle).Text("Dosage");
                                header.Cell().Element(CellStyle).Text("Frequency / Duration");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                                }
                            });

                            int index = 1;
                            foreach (var item in prescription.Items)
                            {
                                table.Cell().Element(CellStyle).Text(index++.ToString());
                                table.Cell().Element(CellStyle).Text(item.Medicine?.MedicineName ?? "Unknown Medicine");
                                table.Cell().Element(CellStyle).Text(item.Dosage ?? "-");
                                table.Cell().Element(CellStyle).Text($"{item.Frequency} / {item.DurationDays} days");

                                static IContainer CellStyle(IContainer container)
                                {
                                    return container.PaddingVertical(5);
                                }
                            }
                        });

                        if (!string.IsNullOrEmpty(prescription.Notes))
                        {
                            x.Item().PaddingTop(20).Column(c =>
                            {
                                c.Item().Text("Clinical Notes").SemiBold().FontColor("#475569");
                                c.Item().Text(prescription.Notes);
                            });
                        }

                        x.Item().AlignBottom().PaddingTop(50).Column(c => {
                            c.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Medium);
                            c.Item().AlignCenter().Text("Digitally generated by SPMD. Confidential Medical Record.").FontSize(10).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            }).GeneratePdf();
        }
    }
}
