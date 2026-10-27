using Application.Abstractions.Mappers;
using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Application.Features.CustomerFeature.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.CustomerFeature.Commands.Update;

public sealed class UpdateCustomerCommandHandler(IRepository<Customer> customerRepository, IValidator<UpdateCustomerCommand> validator, IMapper<Customer, CustomerDto> customerMapper) : IRequestHandler<UpdateCustomerCommand, CustomerDto>
{
    public async Task<CustomerDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var customer = await customerRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Müşteri", request.Id);

        customer.FullName = request.FullName;
        customer.CompanyName = request.CompanyName;
        customer.PhoneNumber = request.PhoneNumber;
        customer.Email = request.Email;
        customer.CustomerType = request.CustomerType;
        customer.Sector = request.Sector;
        customer.Website = request.Website;
        customer.SourceUrl = request.SourceUrl;
        customer.Location = request.Location;
        customer.Note = request.Note;
        customer.UpdatedAt = DateTimeOffset.UtcNow;

        customerRepository.Update(customer);
        await customerRepository.SaveChangesAsync();

        return customerMapper.Map(customer);
    }
}
