using Application.Common.Validators;
using Application.Extensions;
using Application.Features.ProductFeature.Commands.Delete;

namespace Application.Features.ProductFeature.Validators;

public sealed class DeleteProductCommandValidator : BaseValidator<DeleteProductCommand>
{
    public override bool Validate(DeleteProductCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Ürün silme bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir ürün kimliği giriniz.");

        return IsValid;
    }
}
