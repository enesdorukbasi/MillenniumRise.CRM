using Domain.Base;

namespace Domain.Entities;

public sealed class Product : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid? ProductLogoId { get; set; }
    public UploadedFile? ProductLogo { get; set; }

    public ICollection<CustomerProduct> CustomerProducts { get; set; } = [];
}
