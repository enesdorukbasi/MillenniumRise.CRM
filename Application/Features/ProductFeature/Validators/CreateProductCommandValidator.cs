using Application.Common.Validators;
using Application.Extensions;
using Application.Features.ProductFeature.Commands.Create;

namespace Application.Features.ProductFeature.Validators;

public sealed class CreateProductCommandValidator : BaseValidator<CreateProductCommand>
{
    public override bool Validate(CreateProductCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Ürün bilgileri zorunludur.");
            return false;
        }

        if (model.Name.IsNullOrEmpty())
            AddError("Ürün adı zorunludur.");
        else if (!model.Name.MaxLengthControl(200))
            AddError("Ürün adı en fazla 200 karakter olabilir.");

        if (model.Description.IsNotNullOrEmpty() && !model.Description.MaxLengthControl(4_000))
            AddError("Ürün açıklaması en fazla 4000 karakter olabilir.");

        if (model.ProductLogoId == Guid.Empty)
            AddError("Geçerli bir ürün logosu kimliği giriniz.");

        return IsValid;
    }
}
