using Application.Common.Validators;
using Application.Extensions;
using Application.Features.InteractionFeature.Commands.Create;

namespace Application.Features.InteractionFeature.Validators;

public sealed class CreateInteractionCommandValidator : BaseValidator<CreateInteractionCommand>
{
    public override bool Validate(CreateInteractionCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Etkileşim bilgileri zorunludur.");
            return false;
        }

        if (model.CustomerProductId == Guid.Empty)
            AddError("Geçerli bir müşteri ürünü kimliği giriniz.");

        if (!Enum.IsDefined(model.Channel))
            AddError("Geçerli bir etkileşim kanalı seçiniz.");

        if (!Enum.IsDefined(model.Status))
            AddError("Geçerli bir etkileşim durumu seçiniz.");

        if (model.Outcome.HasValue && !Enum.IsDefined(model.Outcome.Value))
            AddError("Geçerli bir etkileşim sonucu seçiniz.");

        if (model.Notes.IsNullOrEmpty())
            AddError("Etkileşim notu zorunludur.");
        else if (!model.Notes.MaxLengthControl(4_000))
            AddError("Etkileşim notu en fazla 4000 karakter olabilir.");

        return IsValid;
    }
}
