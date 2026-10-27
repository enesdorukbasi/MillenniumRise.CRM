using Application.Common.Validators;
using Application.Extensions;
using Application.Features.ProductFeature.Queries.GetById;

namespace Application.Features.ProductFeature.Validators;

public sealed class GetProductByIdQueryValidator : BaseValidator<GetProductByIdQuery>
{
    public override bool Validate(GetProductByIdQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Ürün sorgu bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir ürün kimliği giriniz.");

        return IsValid;
    }
}
