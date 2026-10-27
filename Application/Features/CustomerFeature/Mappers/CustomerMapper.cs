using Application.Abstractions.Mappers;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;

namespace Application.Features.CustomerFeature.Mappers;

public sealed class CustomerMapper : IMapper<Customer, CustomerDto>
{
    public CustomerDto Map(Customer entity)
    {
        return new CustomerDto
        {
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            FullName = entity.FullName,
            CompanyName = entity.CompanyName,
            PhoneNumber = entity.PhoneNumber,
            Email = entity.Email,
            CustomerType = entity.CustomerType,
            Sector = entity.Sector,
            Website = entity.Website,
            SourceUrl = entity.SourceUrl,
            Location = entity.Location,
            Note = entity.Note
        };
    }

    public List<CustomerDto> Map(List<Customer> entities)
    {
        return entities.Select(Map).ToList();
    }
}