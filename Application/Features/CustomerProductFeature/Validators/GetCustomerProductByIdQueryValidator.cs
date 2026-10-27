using Application.Common.Validators;
using Application.Extensions;
using Application.Features.CustomerProductFeature.Queries.GetById;

namespace Application.Features.CustomerProductFeature.Validators;

public sealed class GetCustomerProductByIdQueryValidator : BaseValidator<GetCustomerProductByIdQuery>
{
    public override bool Validate(GetCustomerProductByIdQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Müşteri ürünü sorgu bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir müşteri ürünü kimliği giriniz.");

        return IsValid;
    }
}
