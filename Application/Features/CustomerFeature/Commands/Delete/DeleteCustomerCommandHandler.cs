using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.CustomerFeature.Commands.Delete;

public sealed class DeleteCustomerCommandHandler(IRepository<Customer> customerRepository, IValidator<DeleteCustomerCommand> validator) : IRequestHandler<DeleteCustomerCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var customer = await customerRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Müşteri", request.Id);

        customerRepository.Remove(customer);
        await customerRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
