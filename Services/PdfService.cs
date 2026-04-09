using Ishurim.Models;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.ComponentModel;
using System.Reflection;

namespace Ishurim.Services
{
    public class PdfService
    {
        public byte[] GenerateApproval(Approval approval)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            using var fontStream = File.OpenRead("Resources/Fonts/DavidLibre-Regular.ttf");
            FontManager.RegisterFontWithCustomName("David", fontStream);

            using var fontStream1 = File.OpenRead("Resources/Fonts/DavidLibre-Medium.ttf");
            FontManager.RegisterFontWithCustomName("DavidMed", fontStream1);

            using var fontStream2 = File.OpenRead("Resources/Fonts/DavidLibre-Bold.ttf");
            FontManager.RegisterFontWithCustomName("DavidBold", fontStream2);

            DateOnly date = DateOnly.FromDateTime(DateTime.Now);
            void AddLine(TextDescriptor builder, string content, string font, int fontSize, int extraSpace = 0, bool italic = false, bool truncate = true)
            {
                string text = content.Length > 24 && truncate ? content.Substring(0, 24) + "..." : content;
                if (italic) builder.Line(text).FontFamily(font).FontSize(fontSize).Italic();
                else { builder.Line(text).FontFamily(font).FontSize(fontSize); }
                builder.Line("").FontSize(2);
                if (extraSpace > 0)
                {
                    builder.Line("").FontSize(extraSpace);
                }
            }

            ApproverService approverService = new();
            HospitalService hospitalService = new();
            TestService testService = new();
            InstituteService instituteService = new();
            VehicleService vehicleService = new();
            AccountService accountService = new();

            string approverFullName = approverService.GetApproverById(approval.ApproverId).FullName;
            Institute institute = instituteService.GetInstituteById(approval.InstituteId);
            string instituteName = institute.Name;
            string hospitalName = hospitalService.GetHospitalById(institute.HospitalId).Name;
            string testName = testService.GetTestById(approval.TestId).Name;
            string vehicleName = vehicleService.GetVehicleById(approval.VehicleId).Name;
            string clerkName = accountService.GetUserById(approval.ClerkId).FullName;

            var doc = Document
                .Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(16);

                        page.Content()
                            .Column(column =>
                            {

                                // Pre-header
                                column.Item().Element(container =>
                                {
                                    container.Height(20).Layers(layers =>
                                    {
                                        // Background / main layer (centered text)
                                        layers.PrimaryLayer()
                                            .AlignCenter()
                                            .AlignTop()
                                            .Text("מרכז רפואי כרמל - משרד קבלת חולים")
                                            .FontFamily("DavidMed")
                                            .FontSize(12);

                                        // Top-left overlay
                                        layers.Layer()
                                            .AlignTop()
                                            .AlignLeft()
                                            .Text(date.ToString("dd/MM/yyyy"))
                                            .FontFamily("David")
                                            .FontSize(10);
                                    });
                                });

                                // Header
                                column.Item().Element(container =>
                                {
                                    container
                                    .AlignCenter()
                                    .Shrink()
                                    .BorderBottom(1)
                                    .PaddingBottom(2)
                                    .Text("אישור לביצוע בדיקה לחולה")
                                    .FontFamily("DavidBold")
                                    .FontSize(20);
                                });

                                column.Item().PaddingTop(24).Row(row =>
                                {
                                    row.RelativeItem(24).Column(column =>
                                    {
                                        column.Item()
                                        .Height(50)
                                        .Row(row =>
                                        {
                                            row.RelativeItem(16).Element(container =>
                                            {
                                                container
                                                .ContentFromRightToLeft()
                                                .Height(50)
                                                .PaddingRight(5)
                                                .AlignRight()
                                                .Text(text =>
                                                {
                                                    int fs = 11;
                                                    string font = "David";

                                                    AddLine(text, hospitalName, font, fs, 0, true);
                                                    AddLine(text, instituteName, font, fs, 0, true);
                                                });
                                            });

                                            row.RelativeItem(4).Element(container =>
                                            {
                                                container
                                                .Height(50)
                                                .AlignRight()
                                                .Text(text =>
                                                {
                                                    int fs = 11;
                                                    string font = "DavidBold";

                                                    AddLine(text, "בית חולים", font, fs);
                                                    AddLine(text, "מכון", font, fs);
                                                });
                                            });
                                        });
                                        column.Item()
                                        .Height(120)
                                        .Row(row =>
                                        {
                                            row.RelativeItem(19).Element(container =>
                                            {
                                                container
                                                .AlignCenter()
                                                .Shrink()
                                                .BorderBottom(1)
                                                .PaddingBottom(2)
                                                .Text("חותמת בית החולים")
                                                .FontFamily("DavidBold")
                                                .FontSize(12);
                                            });
                                            row.RelativeItem(20).Element(container =>
                                            {
                                                container
                                                .AlignCenter()
                                                .Shrink()
                                                .BorderBottom(1)
                                                .PaddingBottom(2)
                                                .Text("מדבקת החולה")
                                                .FontFamily("DavidBold")
                                                .FontSize(12);
                                            });
                                        });

                                        column.Item()
                                        .ContentFromRightToLeft()
                                        .Height(150)
                                        .AlignRight()
                                        .Text(text =>
                                        {
                                            int fs = 12;

                                            AddLine(text, "הערות", "DavidBold", fs);
                                            AddLine(text, approval.Note, "David", fs, 0, true, false);
                                        });
                                    });

                                    row.RelativeItem(10).Element(container =>
                                    {
                                        container
                                        .ContentFromRightToLeft()
                                        .Height(200)
                                        .PaddingRight(5)
                                        .AlignRight()
                                        .Text(text =>
                                        {
                                            int fs = 11;
                                            string font = "David";

                                            AddLine(text, approval.ApprovalId.ToString(), font, fs, 0, true);
                                            AddLine(text, approval.HospitalizationId, font, fs, 0, true);
                                            AddLine(text, approval.ApprovalDate.ToString("dd/MM/yyyy"), font, fs, 6, true);
                                            AddLine(text, testName, font, fs, 10, true);
                                            AddLine(text, approval.FirstName, font, fs, 0, true);
                                            AddLine(text, approval.LastName, font, fs, 0, true);
                                            AddLine(text, approval.IdNumber, font, fs, 0, true);
                                            AddLine(text, approval.Department, font, fs, 0, true);
                                            AddLine(text, vehicleName, font, fs, 24, true);
                                            AddLine(text, approverFullName, font, fs, 0, true);
                                            AddLine(text, clerkName, font, fs, 0, true);
                                        });
                                    });

                                    row.RelativeItem(5).Element(container =>
                                    {
                                        container
                                        .ContentFromRightToLeft()
                                        .Height(200)
                                        .AlignRight()
                                        .Text(text =>
                                        {
                                            int fs = 11;
                                            string font = "DavidBold";

                                            AddLine(text, "מספר שובר", font, fs);
                                            AddLine(text, "מספר אשפוז", font, fs);
                                            AddLine(text, "תאריך", font, fs, 6);
                                            AddLine(text, "סוג בדיקה", font, fs, 10);
                                            AddLine(text, "שם משפחה", font, fs);
                                            AddLine(text, "שם פרטי", font, fs);
                                            AddLine(text, "תעודת זהות", font, fs);
                                            AddLine(text, "מחלקה שולחת", font, fs);
                                            AddLine(text, "כלי תחבורה", font, fs, 24);
                                            AddLine(text, "המאשר", font, fs);
                                            AddLine(text, "שם פקיד", font, fs);
                                        });

                                    });

                                });


                                column.Item().Element(container =>
                                {
                                    container
                                    .ContentFromRightToLeft()
                                    .AlignCenter()
                                    .Shrink()
                                    .Border(2)
                                    .Padding(2)
                                    .Text("אישור זה מהווה התחייבות עבור הבדיקה הנ\"ל בלבד.")
                                    .FontFamily("DavidBold")
                                    .FontSize(20);
                                });

                            });
                    });
                })
                .GeneratePdf();

            return doc;
        }
    }
}
