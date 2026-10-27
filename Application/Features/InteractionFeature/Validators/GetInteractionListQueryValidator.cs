using Application.Common.Validators;
using Application.Extensions;
using Application.Features.InteractionFeature.Queries.GetList;

namespace Application.Features.InteractionFeature.Validators;

public sealed class GetInteractionListQueryValidator : BaseValidator<GetInteractionListQuery>
{
    public override bool Validate(GetInteractionListQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Etkileşim listeleme bilgileri zorunludur.");
            return false;
        }

        if (model.CustomerProductId == Guid.Empty)
            AddError("Geçerli bir müşteri ürünü kimliği giriniz.");

        if (model.Status.HasValue && !Enum.IsDefined(model.Status.Value))
            AddError("Geçerli bir etkileşim durumu seçiniz.");

        if (model.Channel.HasValue && !Enum.IsDefined(model.Channel.Value))
            AddError("Geçerli bir etkileşim kanalı seçiniz.");

        if (model.StartDate.HasValue && model.EndDate.HasValue && model.StartDate.Value > model.EndDate.Value)
            AddError("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        return IsValid;
    }
}
