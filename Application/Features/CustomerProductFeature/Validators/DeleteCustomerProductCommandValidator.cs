using Application.Common.Validators;
using Application.Extensions;
using Application.Features.CustomerProductFeature.Commands.Delete;

namespace Application.Features.CustomerProductFeature.Validators;

public sealed class DeleteCustomerProductCommandValidator : BaseValidator<DeleteCustomerProductCommand>
{
    public override bool Validate(DeleteCustomerProductCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Müşteri ürünü silme bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir müşteri ürünü kimliği giriniz.");

        return IsValid;
    }
}
