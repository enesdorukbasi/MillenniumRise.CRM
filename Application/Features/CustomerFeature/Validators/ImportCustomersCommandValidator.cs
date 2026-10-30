using Application.Abstractions.Validators;
using Application.Common.Validators;
using Application.Features.CustomerFeature.Commands.Create;
using Application.Features.CustomerFeature.Commands.Import;

namespace Application.Features.CustomerFeature.Validators;

public sealed class ImportCustomersCommandValidator(IValidator<CreateCustomerCommand> customerValidator) : BaseValidator<ImportCustomersCommand>
{
    private const int MaximumReportedErrorCount = 100;

    public override bool Validate(ImportCustomersCommand model)
    {
        ClearErrors();

        if (model is null || model.Rows.Count == 0)
        {
            AddError("Excel dosyasında içe aktarılacak müşteri bulunamadı.");
            return false;
        }

        var reportedErrorCount = 0;
        var totalErrorCount = 0;

        foreach (var row in model.Rows)
        {
            var hasValidCustomerType = CustomerTypeParser.TryParse(row.CustomerType, out var customerType);
            var customerCommand = new CreateCustomerCommand(
                row.FullName ?? string.Empty,
                row.CompanyName,
                row.PhoneNumber,
                row.Email,
                customerType,
                row.Sector,
                row.Website,
                row.SourceUrl,
                row.Location,
                row.Note);

            customerValidator.Validate(customerCommand);
            var rowErrors = customerValidator.GetErrors
                .Select(error => new ImportValidationError(GetColumnName(error), error))
                .ToList();

            if (!hasValidCustomerType)
            {
                rowErrors.RemoveAll(error => error.Message == "Geçerli bir müşteri tipi seçiniz.");
                rowErrors.Add(new ImportValidationError(
                    "Müşteri Tipi",
                    string.IsNullOrWhiteSpace(row.CustomerType)
                        ? "Müşteri tipi zorunludur. Kabul edilen değerler: Bireysel veya Kurumsal."
                        : $"'{row.CustomerType}' geçerli bir müşteri tipi değildir. Kabul edilen değerler: Bireysel veya Kurumsal."));
            }

            totalErrorCount += rowErrors.Count;
            foreach (var error in rowErrors)
            {
                if (reportedErrorCount >= MaximumReportedErrorCount)
                    break;

                AddError($"Satır {row.RowNumber}, '{error.ColumnName}' kolonu: {error.Message}");
                reportedErrorCount++;
            }
        }

        if (totalErrorCount > MaximumReportedErrorCount)
            AddError($"Ayrıca gösterilmeyen {totalErrorCount - MaximumReportedErrorCount} doğrulama hatası daha var.");

        return IsValid;
    }

    private static string GetColumnName(string error)
    {
        if (error.StartsWith("Ad soyad", StringComparison.OrdinalIgnoreCase))
            return "Ad Soyad";
        if (error.StartsWith("Şirket adı", StringComparison.OrdinalIgnoreCase))
            return "Şirket Adı";
        if (error.StartsWith("Telefon", StringComparison.OrdinalIgnoreCase) || error.StartsWith("Geçerli bir telefon", StringComparison.OrdinalIgnoreCase))
            return "Telefon";
        if (error.StartsWith("E-posta", StringComparison.OrdinalIgnoreCase) || error.StartsWith("Geçerli bir e-posta", StringComparison.OrdinalIgnoreCase))
            return "E-posta";
        if (error.Contains("müşteri tipi", StringComparison.OrdinalIgnoreCase))
            return "Müşteri Tipi";
        if (error.StartsWith("Sektör", StringComparison.OrdinalIgnoreCase))
            return "Sektör";
        if (error.StartsWith("Web sitesi", StringComparison.OrdinalIgnoreCase) || error.StartsWith("Geçerli bir web sitesi", StringComparison.OrdinalIgnoreCase))
            return "Web Sitesi";
        if (error.StartsWith("Kaynak", StringComparison.OrdinalIgnoreCase) || error.StartsWith("Geçerli bir kaynak", StringComparison.OrdinalIgnoreCase))
            return "Kaynak Adresi";
        if (error.StartsWith("Konum", StringComparison.OrdinalIgnoreCase))
            return "Konum";
        if (error.StartsWith("Not", StringComparison.OrdinalIgnoreCase))
            return "Not";

        return "Bilinmeyen";
    }

    private sealed record ImportValidationError(string ColumnName, string Message);
}
