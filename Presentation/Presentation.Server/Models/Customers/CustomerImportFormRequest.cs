namespace Presentation.Server.Models.Customers;

public sealed class CustomerImportFormRequest
{
    public IFormFile File { get; init; } = default!;
}
