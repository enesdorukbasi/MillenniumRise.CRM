using Application.Common.Validators;
using Application.Extensions;
using Application.Features.UploadedFileFeature.Commands.UploadMany;

namespace Application.Features.UploadedFileFeature.Validators;

public sealed class UploadUploadedFilesCommandValidator : BaseValidator<UploadUploadedFilesCommand>
{
    public override bool Validate(UploadUploadedFilesCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Dosya yükleme bilgileri zorunludur.");
            return false;
        }

        if (model.Files.IsNullOrEmpty())
        {
            AddError("En az bir dosya seçilmelidir.");
            return false;
        }

        if (model.Files.Any(file => file.IsNull()))
            AddError("Dosya listesinde geçersiz bir dosya bulunuyor.");

        if (model.Files.Any(file => file is not null && file.FileName.IsNullOrEmpty()))
            AddError("Bütün dosyaların adı bulunmalıdır.");

        if (model.Files.Any(file => file is not null && file.Length <= 0))
            AddError("Boş dosya yüklenemez.");

        if (model.Files.Any(file => file is not null && file.OpenReadStream is null))
            AddError("Dosyalardan en az birinin içeriği okunamıyor.");

        if (model.Files.Any(file => file is not null && Path.GetExtension(file.FileName).Length > 32))
            AddError("Dosya uzantısı en fazla 32 karakter olabilir.");

        return IsValid;
    }
}
