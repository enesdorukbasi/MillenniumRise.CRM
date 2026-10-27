using Application.Common.Validators;
using Application.Extensions;
using Application.Features.UploadedFileFeature.Commands.Upload;

namespace Application.Features.UploadedFileFeature.Validators;

public sealed class UploadUploadedFileCommandValidator : BaseValidator<UploadUploadedFileCommand>
{
    public override bool Validate(UploadUploadedFileCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Dosya yükleme bilgileri zorunludur.");
            return false;
        }

        if (model.File.IsNull())
        {
            AddError("Dosya zorunludur.");
            return false;
        }

        if (model.File.FileName.IsNullOrEmpty())
            AddError("Dosya adı zorunludur.");

        if (model.File.Length <= 0)
            AddError("Dosya boş olamaz.");

        if (model.File.OpenReadStream is null)
            AddError("Dosya içeriği okunamıyor.");

        if (Path.GetExtension(model.File.FileName).Length > 32)
            AddError("Dosya uzantısı en fazla 32 karakter olabilir.");

        return IsValid;
    }
}
