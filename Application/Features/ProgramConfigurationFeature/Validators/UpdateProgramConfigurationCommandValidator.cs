using Application.Common.Validators;
using Application.Extensions;
using Application.Features.ProgramConfigurationFeature.Commands.Update;

namespace Application.Features.ProgramConfigurationFeature.Validators;

public sealed class UpdateProgramConfigurationCommandValidator : BaseValidator<UpdateProgramConfigurationCommand>
{
    public override bool Validate(UpdateProgramConfigurationCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Program ayarları zorunludur.");
            return false;
        }

        if (model.Name.IsNullOrEmpty())
            AddError("Program adı zorunludur.");
        else if (!model.Name.MaxLengthControl(200))
            AddError("Program adı en fazla 200 karakter olabilir.");

        if (model.LogoId == Guid.Empty)
            AddError("Geçerli bir logo dosyası kimliği giriniz.");

        if (model.AuthPageImageId == Guid.Empty)
            AddError("Geçerli bir giriş sayfası görseli kimliği giriniz.");

        return IsValid;
    }
}
