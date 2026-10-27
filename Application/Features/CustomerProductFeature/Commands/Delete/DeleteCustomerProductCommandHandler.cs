using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.CustomerProductFeature.Commands.Delete;

public sealed class DeleteCustomerProductCommandHandler(IRepository<CustomerProduct> customerProductRepository, IValidator<DeleteCustomerProductCommand> validator) : IRequestHandler<DeleteCustomerProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCustomerProductCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var customerProduct = await customerProductRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Müşteri ürünü", request.Id);

        customerProductRepository.Remove(customerProduct);
        await customerProductRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
