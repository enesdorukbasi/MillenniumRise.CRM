using Application.Common.Validators;
using Application.Extensions;
using Application.Features.CustomerFeature.Commands.Create;

namespace Application.Features.CustomerFeature.Validators;

public sealed class CreateCustomerCommandValidator : BaseValidator<CreateCustomerCommand>
{
    public override bool Validate(CreateCustomerCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Müşteri bilgileri zorunludur.");
            return false;
        }

        if (model.FullName.IsNullOrEmpty())
            AddError("Ad soyad zorunludur.");
        else if (!model.FullName.MaxLengthControl(200))
            AddError("Ad soyad en fazla 200 karakter olabilir.");

        if (model.CompanyName.IsNotNullOrEmpty() && !model.CompanyName.MaxLengthControl(200))
            AddError("Şirket adı en fazla 200 karakter olabilir.");

        if (model.PhoneNumber.IsNotNullOrEmpty())
        {
            if (!model.PhoneNumber.MaxLengthControl(32))
                AddError("Telefon numarası en fazla 32 karakter olabilir.");
            else if (!model.PhoneNumber.IsValidPhoneNumber())
                AddError("Geçerli bir telefon numarası giriniz.");
        }

        if (model.Email.IsNotNullOrEmpty())
        {
            if (!model.Email.MaxLengthControl(320))
                AddError("E-posta adresi en fazla 320 karakter olabilir.");
            else if (!model.Email.IsValidEmail())
                AddError("Geçerli bir e-posta adresi giriniz.");
        }

        if (!Enum.IsDefined(model.CustomerType))
            AddError("Geçerli bir müşteri tipi seçiniz.");

        if (model.Sector.IsNotNullOrEmpty() && !model.Sector.MaxLengthControl(150))
            AddError("Sektör en fazla 150 karakter olabilir.");

        if (model.Website.IsNotNullOrEmpty())
        {
            if (!model.Website.MaxLengthControl(2_048))
                AddError("Web sitesi adresi en fazla 2048 karakter olabilir.");
            else if (!model.Website.IsValidUrl())
                AddError("Geçerli bir web sitesi adresi giriniz.");
        }

        if (model.SourceUrl.IsNotNullOrEmpty())
        {
            if (!model.SourceUrl.MaxLengthControl(2_048))
                AddError("Kaynak adresi en fazla 2048 karakter olabilir.");
            else if (!model.SourceUrl.IsValidUrl())
                AddError("Geçerli bir kaynak adresi giriniz.");
        }

        if (model.Location.IsNotNullOrEmpty() && !model.Location.MaxLengthControl(300))
            AddError("Konum en fazla 300 karakter olabilir.");

        if (model.Note.IsNotNullOrEmpty() && !model.Note.MaxLengthControl(4_000))
            AddError("Not en fazla 4000 karakter olabilir.");

        return IsValid;
    }
}
