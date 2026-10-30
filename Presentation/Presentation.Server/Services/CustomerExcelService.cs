using System.Globalization;
using System.Text;
using Application.Features.CustomerFeature.Commands.Import;
using ClosedXML.Excel;
using Domain.Exceptions;

namespace Presentation.Server.Services;

public sealed class CustomerExcelService
{
    private const int MaximumRowCount = 5_000;

    private static readonly IReadOnlyDictionary<string, string> HeaderAliases = CreateHeaderAliases();

    public IReadOnlyList<ImportCustomerRow> ReadRows(Stream stream)
    {
        try
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw new AppValidationException("Excel dosyasında çalışma sayfası bulunamadı.");
            var headerRow = worksheet.FirstRowUsed()
                ?? throw new AppValidationException("Excel dosyası boş.");
            var columns = ReadColumns(headerRow);

            if (!columns.ContainsKey("FullName"))
                throw new AppValidationException("Zorunlu 'Ad Soyad' sütunu bulunamadı.");
            if (!columns.ContainsKey("CustomerType"))
                throw new AppValidationException("Zorunlu 'Müşteri Tipi' sütunu bulunamadı.");

            var rows = new List<ImportCustomerRow>();
            var lastRowNumber = worksheet.LastRowUsed()?.RowNumber() ?? headerRow.RowNumber();

            for (var rowNumber = headerRow.RowNumber() + 1; rowNumber <= lastRowNumber; rowNumber++)
            {
                var row = worksheet.Row(rowNumber);
                var values = columns.ToDictionary(
                    column => column.Key,
                    column => GetNullableValue(row.Cell(column.Value)));

                if (values.Values.All(value => value is null))
                    continue;

                rows.Add(new ImportCustomerRow(
                    rowNumber,
                    GetValue(values, "FullName"),
                    GetValue(values, "CompanyName"),
                    GetValue(values, "PhoneNumber"),
                    GetValue(values, "Email"),
                    GetValue(values, "CustomerType"),
                    GetValue(values, "Sector"),
                    GetValue(values, "Website"),
                    GetValue(values, "SourceUrl"),
                    GetValue(values, "Location"),
                    GetValue(values, "Note")));

                if (rows.Count > MaximumRowCount)
                    throw new AppValidationException($"Tek seferde en fazla {MaximumRowCount:N0} müşteri içe aktarılabilir.");
            }

            return rows;
        }
        catch (AppValidationException)
        {
            throw;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new AppValidationException("Excel dosyası okunamadı. Lütfen şablonu kullanarak geçerli bir .xlsx dosyası yükleyin.");
        }
    }

    public byte[] CreateTemplate()
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Müşteriler");
        var headers = new[]
        {
            "Ad Soyad *", "Şirket Adı", "Telefon", "E-posta", "Müşteri Tipi *",
            "Sektör", "Web Sitesi", "Kaynak Adresi", "Konum", "Not"
        };

        for (var index = 0; index < headers.Length; index++)
            worksheet.Cell(1, index + 1).Value = headers[index];

        var headerRange = worksheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontColor = XLColor.White;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#2D73E8");
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.SheetView.FreezeRows(1);
        worksheet.Range(1, 1, 1, headers.Length).SetAutoFilter();

        worksheet.Column(1).Width = 28;
        worksheet.Column(2).Width = 28;
        worksheet.Column(3).Width = 20;
        worksheet.Column(4).Width = 30;
        worksheet.Column(5).Width = 20;
        worksheet.Column(6).Width = 22;
        worksheet.Column(7).Width = 32;
        worksheet.Column(8).Width = 32;
        worksheet.Column(9).Width = 28;
        worksheet.Column(10).Width = 45;
        worksheet.Column(3).Style.NumberFormat.Format = "@";

        var instructions = workbook.AddWorksheet("Açıklamalar");
        instructions.Cell("A1").Value = "Müşteri İçe Aktarma Açıklamaları";
        instructions.Cell("A1").Style.Font.Bold = true;
        instructions.Cell("A1").Style.Font.FontSize = 14;
        instructions.Cell("A3").Value = "• Ad Soyad ve Müşteri Tipi zorunludur.";
        instructions.Cell("A4").Value = "• Müşteri Tipi alanına Bireysel veya Kurumsal yazın.";
        instructions.Cell("A5").Value = "• Boş bırakılan isteğe bağlı alanlar null olarak kaydedilir.";
        instructions.Cell("A6").Value = "• Web Sitesi ve Kaynak Adresi alanlarında tam adres kullanın (ör. https://example.com).";
        instructions.Cell("A7").Value = $"• Tek dosyada en fazla {MaximumRowCount:N0} müşteri içe aktarılabilir.";
        instructions.Column(1).Width = 100;

        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }

    private static Dictionary<string, int> ReadColumns(IXLRow headerRow)
    {
        var columns = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var cell in headerRow.CellsUsed())
        {
            var normalizedHeader = NormalizeHeader(cell.GetFormattedString());
            if (!HeaderAliases.TryGetValue(normalizedHeader, out var propertyName))
                continue;
            if (!columns.TryAdd(propertyName, cell.Address.ColumnNumber))
                throw new AppValidationException($"'{cell.GetFormattedString()}' sütunu birden fazla kez tanımlanmış.");
        }

        return columns;
    }

    private static string? GetNullableValue(IXLCell cell)
    {
        var value = cell.GetFormattedString().Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }

    private static string? GetValue(IReadOnlyDictionary<string, string?> values, string key)
        => values.TryGetValue(key, out var value) ? value : null;

    private static IReadOnlyDictionary<string, string> CreateHeaderAliases()
    {
        var aliases = new Dictionary<string, string>(StringComparer.Ordinal);
        AddAliases(aliases, "FullName", "Ad Soyad", "Full Name", "FullName");
        AddAliases(aliases, "CompanyName", "Şirket Adı", "Firma Adı", "Company Name", "CompanyName");
        AddAliases(aliases, "PhoneNumber", "Telefon", "Telefon Numarası", "Phone Number", "PhoneNumber");
        AddAliases(aliases, "Email", "E-posta", "Eposta", "Email");
        AddAliases(aliases, "CustomerType", "Müşteri Tipi", "Customer Type", "CustomerType");
        AddAliases(aliases, "Sector", "Sektör", "Sector");
        AddAliases(aliases, "Website", "Web Sitesi", "Website");
        AddAliases(aliases, "SourceUrl", "Kaynak Adresi", "Kaynak URL", "Source URL", "SourceUrl");
        AddAliases(aliases, "Location", "Konum", "Location");
        AddAliases(aliases, "Note", "Not", "Note");
        return aliases;
    }

    private static void AddAliases(IDictionary<string, string> aliases, string propertyName, params string[] headers)
    {
        foreach (var header in headers)
            aliases[NormalizeHeader(header)] = propertyName;
    }

    private static string NormalizeHeader(string value)
    {
        var decomposed = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
                continue;
            if (char.IsLetterOrDigit(character))
                builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}
