using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.CustomerFeature.Commands.Create;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.CustomerFeature.Commands.Import;

public sealed class ImportCustomersCommandHandler(IRepository<Customer> customerRepository, IValidator<CreateCustomerCommand> customerValidator) : IRequestHandler<ImportCustomersCommand, ImportCustomersResultDto>
{
    private const int MaximumReportedErrorCount = 100;

    public async Task<ImportCustomersResultDto> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
    {
        if (request.Rows.Count == 0)
            throw new AppValidationException("Excel dosyasında içe aktarılacak müşteri bulunamadı.");

        var errors = new List<string>();
        var totalErrorCount = 0;
        var customers = new List<Customer>(request.Rows.Count);

        foreach (var row in request.Rows)
        {
            var hasValidCustomerType = TryParseCustomerType(row.CustomerType, out var customerType);
            var command = new CreateCustomerCommand(
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

            customerValidator.Validate(command);
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

            if (rowErrors.Count > 0)
            {
                totalErrorCount += rowErrors.Count;
                foreach (var error in rowErrors)
                {
                    if (errors.Count >= MaximumReportedErrorCount)
                        break;
                    errors.Add($"Satır {row.RowNumber}, '{error.ColumnName}' kolonu: {error.Message}");
                }
                continue;
            }

            customers.Add(new Customer
            {
                FullName = command.FullName,
                CompanyName = command.CompanyName,
                PhoneNumber = command.PhoneNumber,
                Email = command.Email,
                CustomerType = command.CustomerType,
                Sector = command.Sector,
                Website = command.Website,
                SourceUrl = command.SourceUrl,
                Location = command.Location,
                Note = command.Note
            });
        }

        if (totalErrorCount > MaximumReportedErrorCount)
            errors.Add($"Ayrıca gösterilmeyen {totalErrorCount - MaximumReportedErrorCount} doğrulama hatası daha var.");

        if (totalErrorCount > 0)
            throw new AppValidationException(errors);

        foreach (var customer in customers)
            await customerRepository.AddAsync(customer);

        await customerRepository.SaveChangesAsync();
        return new ImportCustomersResultDto(customers.Count);
    }

    private static bool TryParseCustomerType(string? value, out CustomerType customerType)
    {
        customerType = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        switch (value.Trim().ToUpperInvariant())
        {
            case "BİREYSEL":
            case "BIREYSEL":
            case "INDIVIDUAL":
            case "0":
                customerType = CustomerType.Individual;
                return true;
            case "KURUMSAL":
            case "BUSINESS":
            case "1":
                customerType = CustomerType.Business;
                return true;
            default:
                return false;
        }
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
