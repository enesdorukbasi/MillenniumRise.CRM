using Application.Abstractions.Persistence.Repository;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.CustomerFeature.Commands.Delete;

public sealed class DeleteCustomerCommandHandler(IRepository<Customer> customerRepository) : IRequestHandler<DeleteCustomerCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await customerRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Müşteri", request.Id);

        customerRepository.Remove(customer);
        await customerRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
