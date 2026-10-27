using Application.Common.Validators;
using Application.Extensions;
using Application.Features.UploadedFileFeature.Queries.GetList;

namespace Application.Features.UploadedFileFeature.Validators;

public sealed class GetUploadedFileListQueryValidator : BaseValidator<GetUploadedFileListQuery>
{
    public override bool Validate(GetUploadedFileListQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Dosya listeleme bilgileri zorunludur.");
            return false;
        }

        if (model.Search.IsNotNullOrEmpty() && !model.Search.MaxLengthControl(300))
            AddError("Arama metni en fazla 300 karakter olabilir.");

        if (model.Extension.IsNotNullOrEmpty() && !model.Extension.MaxLengthControl(32))
            AddError("Dosya uzantısı en fazla 32 karakter olabilir.");

        if (model.ContentType.IsNotNullOrEmpty() && !model.ContentType.MaxLengthControl(255))
            AddError("İçerik tipi en fazla 255 karakter olabilir.");

        if (model.MinimumFileSize < 0)
            AddError("Minimum dosya boyutu negatif olamaz.");

        if (model.MaximumFileSize < 0)
            AddError("Maksimum dosya boyutu negatif olamaz.");

        if (model.MinimumFileSize.HasValue && model.MaximumFileSize.HasValue && model.MinimumFileSize.Value > model.MaximumFileSize.Value)
            AddError("Minimum dosya boyutu maksimum dosya boyutundan büyük olamaz.");

        if (model.CreatedFrom.HasValue && model.CreatedTo.HasValue && model.CreatedFrom.Value > model.CreatedTo.Value)
            AddError("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        if (!Enum.IsDefined(model.SortBy))
            AddError("Geçerli bir dosya sıralama alanı seçiniz.");

        if (!Enum.IsDefined(model.SortDirection))
            AddError("Geçerli bir sıralama yönü seçiniz.");

        if (model.PageNumber < 1)
            AddError("Sayfa numarası en az 1 olmalıdır.");

        if (model.PageSize is < 1 or > 100)
            AddError("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

        if (model.PageNumber > 0 && model.PageSize > 0 && (long)(model.PageNumber - 1) * model.PageSize > int.MaxValue)
            AddError("İstenen sayfa aralığı desteklenen sınırı aşıyor.");

        return IsValid;
    }
}
