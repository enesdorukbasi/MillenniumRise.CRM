using Application.Abstractions.Persistence.Repository;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.Features.CustomerFeature.Commands.Import;

public sealed class ImportCustomersCommandHandler(IRepository<Customer> customerRepository) : IRequestHandler<ImportCustomersCommand, ImportCustomersResultDto>
{
    public async Task<ImportCustomersResultDto> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
    {
        var customers = new List<Customer>(request.Rows.Count);

        foreach (var row in request.Rows)
        {
            CustomerTypeParser.TryParse(row.CustomerType, out var customerType);

            customers.Add(new Customer
            {
                FullName = row.FullName!,
                CompanyName = row.CompanyName,
                PhoneNumber = row.PhoneNumber,
                Email = row.Email,
                CustomerType = customerType,
                Sector = row.Sector,
                Website = row.Website,
                SourceUrl = row.SourceUrl,
                Location = row.Location,
                Note = row.Note
            });
        }

        foreach (var customer in customers)
            await customerRepository.AddAsync(customer);

        await customerRepository.SaveChangesAsync();
        return new ImportCustomersResultDto(customers.Count);
    }
}
