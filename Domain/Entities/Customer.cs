using Domain.Base;
using Domain.Enums;

namespace Domain.Entities;

public sealed class Customer : BaseEntity
{
    public required string FullName { get; set; }
    public string? CompanyName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public CustomerType CustomerType { get; set; }
    public string? Sector { get; set; }
    public string? Website { get; set; }
    public string? SourceUrl { get; set; }
    public string? Location { get; set; }
    public string? Note { get; set; }

    public ICollection<CustomerProduct> CustomerProducts { get; set; } = [];
}
