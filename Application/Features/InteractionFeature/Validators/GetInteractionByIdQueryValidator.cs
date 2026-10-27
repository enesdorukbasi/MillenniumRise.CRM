using Application.Common.Validators;
using Application.Extensions;
using Application.Features.InteractionFeature.Queries.GetById;

namespace Application.Features.InteractionFeature.Validators;

public sealed class GetInteractionByIdQueryValidator : BaseValidator<GetInteractionByIdQuery>
{
    public override bool Validate(GetInteractionByIdQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Etkileşim sorgu bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir etkileşim kimliği giriniz.");

        return IsValid;
    }
}
