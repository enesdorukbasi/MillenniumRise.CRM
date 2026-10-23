using Domain.Base;
using Domain.Enums;

namespace Domain.Entities;

public sealed class CustomerProduct : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public CustomerProductStatus Status { get; set; } = CustomerProductStatus.New;
    public DateTimeOffset? NextFollowUpAt { get; set; }
    public string? StatusNote { get; set; }

    public ICollection<Interaction> Interactions { get; set; } = [];
}
