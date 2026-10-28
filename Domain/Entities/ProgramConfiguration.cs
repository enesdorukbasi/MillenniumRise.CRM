using Domain.Base;

namespace Domain.Entities;

public sealed class ProgramConfiguration : BaseEntity
{
    public required string Name { get; set; } = "MillenniumRise | CRM";
    public Guid? LogoId { get; set; }
    public UploadedFile? Logo { get; set; }
    public Guid? AuthPageImageId { get; set; }
    public UploadedFile? AuthPageImage { get; set; }
}
