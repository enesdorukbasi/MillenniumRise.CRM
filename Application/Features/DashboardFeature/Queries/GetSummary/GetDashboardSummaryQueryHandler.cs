using Application.Abstractions.Persistence.Repository;
using Application.Features.DashboardFeature.DTOs;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.DashboardFeature.Queries.GetSummary;

public sealed class GetDashboardSummaryQueryHandler(IRepository<Customer> customerRepository, IRepository<Product> productRepository, IRepository<CustomerProduct> customerProductRepository) : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var currentDate = DateTimeOffset.UtcNow;
        var totalCustomerCount = await customerRepository.GetCount();
        var activeProductCount = await productRepository.GetCount(product => product.IsActive);
        var customerProductCount = await customerProductRepository.GetCount();
        var pendingFollowUpCount = await customerProductRepository.GetCount(customerProduct =>
            customerProduct.NextFollowUpAt.HasValue &&
            customerProduct.NextFollowUpAt.Value <= currentDate &&
            customerProduct.Status != CustomerProductStatus.Successful &&
            customerProduct.Status != CustomerProductStatus.Unsuccessful);

        return new DashboardSummaryDto
        {
            TotalCustomerCount = totalCustomerCount,
            ActiveProductCount = activeProductCount,
            CustomerProductCount = customerProductCount,
            PendingFollowUpCount = pendingFollowUpCount
        };
    }
}
