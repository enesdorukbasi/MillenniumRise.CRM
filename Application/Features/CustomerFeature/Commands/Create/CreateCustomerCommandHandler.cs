using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.CustomerFeature.Commands.Create;

public sealed class CreateCustomerCommandHandler(IRepository<Customer> customerRepository, IValidator<CreateCustomerCommand> validator, IMapper<Customer, CustomerDto> customerMapper) : IRequestHandler<CreateCustomerCommand, CustomerDto>
{
    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var customer = new Customer
        {
            FullName = request.FullName,
            CompanyName = request.CompanyName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            CustomerType = request.CustomerType,
            Sector = request.Sector,
            Website = request.Website,
            SourceUrl = request.SourceUrl,
            Location = request.Location,
            Note = request.Note
        };

        await customerRepository.AddAsync(customer);
        await customerRepository.SaveChangesAsync();

        return customerMapper.Map(customer);
    }
}
