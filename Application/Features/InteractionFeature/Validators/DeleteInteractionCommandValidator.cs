using Application.Common.Validators;
using Application.Extensions;
using Application.Features.InteractionFeature.Commands.Delete;

namespace Application.Features.InteractionFeature.Validators;

public sealed class DeleteInteractionCommandValidator : BaseValidator<DeleteInteractionCommand>
{
    public override bool Validate(DeleteInteractionCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Etkileşim silme bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir etkileşim kimliği giriniz.");

        return IsValid;
    }
}
