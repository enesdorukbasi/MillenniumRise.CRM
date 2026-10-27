using Application.Common.Validators;
using Application.Extensions;
using Application.Features.UploadedFileFeature.Queries.GetById;

namespace Application.Features.UploadedFileFeature.Validators;

public sealed class GetUploadedFileByIdQueryValidator : BaseValidator<GetUploadedFileByIdQuery>
{
    public override bool Validate(GetUploadedFileByIdQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Dosya sorgu bilgileri zorunludur.");
            return false;
        }

        if (model.Id == Guid.Empty)
            AddError("Geçerli bir dosya kimliği giriniz.");

        return IsValid;
    }
}
