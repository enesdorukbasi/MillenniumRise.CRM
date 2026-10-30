using Application.Abstractions.Persistence.Repository;
using Domain.Entities;
using Domain.Exceptions;
using MediatR;

namespace Application.Features.ProductFeature.Commands.Delete;

public sealed class DeleteProductCommandHandler(IRepository<Product> productRepository) : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("Ürün", request.Id);

        productRepository.Remove(product);
        await productRepository.SaveChangesAsync();

        return Unit.Value;
    }
}
