using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using backend.Models;

namespace backend.Services;

public class PdfExportService
{
    public byte[] GenerateAppointmentsPdf(List<Appointment> appointments)
    {
        using var document = new PdfDocument();
        var page = document.AddPage();
        page.Width = XUnit.FromMillimeter(210);
        page.Height = XUnit.FromMillimeter(297);

        var gfx = XGraphics.FromPdfPage(page);

        var titleFont = new XFont("Arial", 20, XFontStyle.Bold);
        var headerFont = new XFont("Arial", 10, XFontStyle.Bold);
        var cellFont = new XFont("Arial", 10, XFontStyle.Regular);
        var footerFont = new XFont("Arial", 9, XFontStyle.Regular);

        var darkBlue = XColor.FromArgb(0, 0, 102, 153);
        var darkBlueBrush = new XSolidBrush(darkBlue);
        var headerBg = XColor.FromArgb(230, 240, 250);
        var headerBgBrush = new XSolidBrush(headerBg);
        var grayPen = new XPen(XColors.DimGray, 1);
        var lightGrayPen = new XPen(XColors.LightGray, 0.5);

        double margin = 40;
        double y = margin;

        gfx.DrawString("Listado de Citas M\u00e9dicas", titleFont, darkBlueBrush, margin, y);
        y += 30;
        gfx.DrawLine(grayPen, margin, y, page.Width - margin, y);
        y += 15;

        double[] colWidths = [120, 120, 180, 100];
        double tableWidth = colWidths[0] + colWidths[1] + colWidths[2] + colWidths[3];
        double startX = margin;

        string[] headers = ["Paciente", "Fecha", "Motivo", "Estado"];
        gfx.DrawRectangle(headerBgBrush, startX, y - 4, tableWidth, 22);
        double x = startX;
        for (int i = 0; i < headers.Length; i++)
        {
            gfx.DrawString(headers[i], headerFont, XBrushes.Black, x + 4, y);
            x += colWidths[i];
        }

        y += 22;

        foreach (var a in appointments)
        {
            gfx.DrawString(a.PatientName, cellFont, XBrushes.Black, startX + 4, y);
            gfx.DrawString(a.Date.ToString("dd/MM/yyyy HH:mm"), cellFont, XBrushes.Black, startX + colWidths[0] + 4, y);
            gfx.DrawString(a.Reason, cellFont, XBrushes.Black, startX + colWidths[0] + colWidths[1] + 4, y);
            gfx.DrawString(a.Status, cellFont, XBrushes.Black, startX + colWidths[0] + colWidths[1] + colWidths[2] + 4, y);
            y += 20;

            if (y > page.Height - 50)
            {
                page = document.AddPage();
                page.Width = XUnit.FromMillimeter(210);
                page.Height = XUnit.FromMillimeter(297);
                gfx.Dispose();
                gfx = XGraphics.FromPdfPage(page);
                y = margin;
            }
            else
            {
                gfx.DrawLine(lightGrayPen, startX, y - 10, startX + tableWidth, y - 10);
            }
        }

        gfx.DrawString(
            $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}",
            footerFont,
            XBrushes.Gray,
            margin,
            page.Height - margin);

        using var stream = new MemoryStream();
        document.Save(stream);
        gfx.Dispose();
        return stream.ToArray();
    }
    public byte[] GenerateReportPdf(MedicalReport report)
    {
        using var document = new PdfDocument();
        var page = document.AddPage();
        page.Width = XUnit.FromMillimeter(210);
        page.Height = XUnit.FromMillimeter(297);

        using var gfx = XGraphics.FromPdfPage(page);

        var titleFont = new XFont("Arial", 22, XFontStyle.Bold);
        var labelFont = new XFont("Arial", 11, XFontStyle.Bold);
        var valueFont = new XFont("Arial", 11, XFontStyle.Regular);
        var sectionFont = new XFont("Arial", 14, XFontStyle.Bold);
        var footerFont = new XFont("Arial", 9, XFontStyle.Regular);

        var darkBlue = XColor.FromArgb(0, 0, 102, 153);
        var darkBlueBrush = new XSolidBrush(darkBlue);
        var grayPen = new XPen(XColors.DimGray, 1);
        var lightGrayPen = new XPen(XColors.LightGray, 1);

        double margin = 40;
        double y = margin;

        gfx.DrawString("Informe M\u00e9dico", titleFont, darkBlueBrush, margin, y);
        y += 35;

        gfx.DrawLine(grayPen, margin, y, page.Width - margin, y);
        y += 15;

        static void AddRow(XGraphics gfx, string label, string value, XFont labelFont, XFont valueFont, ref double y, double margin, double pageWidth)
        {
            gfx.DrawString(label, labelFont, XBrushes.Black, margin, y);
            gfx.DrawString(value, valueFont, XBrushes.Black, margin + 140, y);
            y += 22;
        }

        AddRow(gfx, "Paciente:", report.PatientName, labelFont, valueFont, ref y, margin, page.Width);
        AddRow(gfx, "T\u00edtulo:", report.Title, labelFont, valueFont, ref y, margin, page.Width);
        AddRow(gfx, "Doctor:", report.Doctor, labelFont, valueFont, ref y, margin, page.Width);
        AddRow(gfx, "Fecha:", report.Date.ToString("dd/MM/yyyy"), labelFont, valueFont, ref y, margin, page.Width);

        y += 8;
        gfx.DrawLine(lightGrayPen, margin, y, page.Width - margin, y);
        y += 15;

        gfx.DrawString("Diagn\u00f3stico / Contenido", sectionFont, darkBlueBrush, margin, y);
        y += 25;

        var contentRect = new XRect(margin, y, page.Width - margin * 2, 300);
        gfx.DrawString(report.Content, valueFont, XBrushes.Black, contentRect, XStringFormats.TopLeft);

        gfx.DrawString(
            $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}",
            footerFont,
            XBrushes.Gray,
            margin,
            page.Height - margin);

        using var stream = new MemoryStream();
        document.Save(stream);
        return stream.ToArray();
    }
}
