using Application.Abstractions.Persistence.Repository;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.CustomerProductFeature.Commands.Delete;

public sealed class DeleteCustomerProductCommandHandler(IRepository<CustomerProduct> customerProductRepository) : IRequestHandler<DeleteCustomerProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCustomerProductCommand request, CancellationToken cancellationToken)
    {
        var customerProduct = await customerProductRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Müşteri ürünü", request.Id);

        customerProductRepository.Remove(customerProduct);
        await customerProductRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
