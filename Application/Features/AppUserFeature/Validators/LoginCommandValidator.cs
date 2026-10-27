using Application.Common.Validators;
using Application.Extensions;
using Application.Features.AppUserFeature.Commands;

namespace Application.Features.AppUserFeature.Validators;

public sealed class LoginCommandValidator : BaseValidator<LoginCommand>
{
    public override bool Validate(LoginCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Giriş bilgileri zorunludur.");
            return false;
        }

        if (model.Email.IsNullOrEmpty())
            AddError("E-posta adresi zorunludur.");
        else if (!model.Email.IsValidEmail())
            AddError("Geçerli bir e-posta adresi giriniz.");

        if (model.Password.IsNullOrEmpty())
            AddError("Parola zorunludur.");
        else if (!model.Password.IsValidPassword())
            AddError("Parola en az 8 karakter olmalı; büyük harf, küçük harf ve rakam içermelidir.");

        return IsValid;
    }
}
