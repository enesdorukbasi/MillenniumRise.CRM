using Application.Common.Validators;
using Application.Extensions;
using Application.Features.UploadedFileFeature.Commands.Delete;

namespace Application.Features.UploadedFileFeature.Validators;

public sealed class DeleteUploadedFileCommandValidator : BaseValidator<DeleteUploadedFileCommand>
{
    public override bool Validate(DeleteUploadedFileCommand model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Dosya silme bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir dosya kimliği giriniz.");

        return IsValid;
    }
}
