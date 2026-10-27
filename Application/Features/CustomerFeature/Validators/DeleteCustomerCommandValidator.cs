using Application.Common.Validators;
using Application.Extensions;
using Application.Features.CustomerFeature.Commands.Delete;

namespace Application.Features.CustomerFeature.Validators;

public sealed class DeleteCustomerCommandValidator : BaseValidator<DeleteCustomerCommand>
{
    public override bool Validate(DeleteCustomerCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Silme bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir müşteri kimliği giriniz.");

        return IsValid;
    }
}