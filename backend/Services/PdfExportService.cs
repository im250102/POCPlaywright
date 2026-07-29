using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using backend.Models;

namespace backend.Services;

public class PdfExportService
{
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
