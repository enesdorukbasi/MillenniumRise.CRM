using Application.Abstractions.Persistence.Repository;
using Application.Abstractions.Validators;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.ProductFeature.Commands.Delete;

public sealed class DeleteProductCommandHandler(IRepository<Product> productRepository, IValidator<DeleteProductCommand> validator) : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        if (!validator.Validate(request))
            throw new AppValidationException(validator.GetErrors);

        var product = await productRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Ürün", request.Id);

        productRepository.Remove(product);
        await productRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
