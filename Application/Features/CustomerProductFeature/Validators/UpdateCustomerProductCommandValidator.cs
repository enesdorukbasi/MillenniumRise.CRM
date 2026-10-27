using Application.Common.Validators;
using Application.Extensions;
using Application.Features.CustomerProductFeature.Commands.Update;

namespace Application.Features.CustomerProductFeature.Validators;

public sealed class UpdateCustomerProductCommandValidator : BaseValidator<UpdateCustomerProductCommand>
{
    public override bool Validate(UpdateCustomerProductCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Müşteri ürün bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir müşteri ürünü kimliği giriniz.");

        if (model.CustomerId == Guid.Empty)
            AddError("Geçerli bir müşteri kimliği giriniz.");

        if (model.ProductId == Guid.Empty)
            AddError("Geçerli bir ürün kimliği giriniz.");

        if (!Enum.IsDefined(model.Status))
            AddError("Geçerli bir müşteri ürün durumu seçiniz.");

        if (model.StatusNote.IsNotNullOrEmpty() && !model.StatusNote.MaxLengthControl(4_000))
            AddError("Durum notu en fazla 4000 karakter olabilir.");

        return IsValid;
    }
}
