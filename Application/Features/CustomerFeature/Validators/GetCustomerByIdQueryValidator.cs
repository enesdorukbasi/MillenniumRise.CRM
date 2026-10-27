using Application.Common.Validators;
using Application.Extensions;
using Application.Features.CustomerFeature.Queries.GetById;

namespace Application.Features.CustomerFeature.Validators;

public sealed class GetCustomerByIdQueryValidator : BaseValidator<GetCustomerByIdQuery>
{
    public override bool Validate(GetCustomerByIdQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Müşteri sorgu bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir müşteri kimliği giriniz.");

        return IsValid;
    }
}
