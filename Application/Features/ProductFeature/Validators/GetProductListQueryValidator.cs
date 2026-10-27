using Application.Common.Validators;
using Application.Extensions;
using Application.Features.ProductFeature.Queries.GetList;

namespace Application.Features.ProductFeature.Validators;

public sealed class GetProductListQueryValidator : BaseValidator<GetProductListQuery>
{
    public override bool Validate(GetProductListQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Ürün listeleme bilgileri zorunludur.");
            return false;
        }

        if (model.Search.IsNotNullOrEmpty() && !model.Search.MaxLengthControl(300))
            AddError("Arama metni en fazla 300 karakter olabilir.");

        if (model.CreatedFrom.HasValue && model.CreatedTo.HasValue && model.CreatedFrom.Value > model.CreatedTo.Value)
            AddError("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        if (!Enum.IsDefined(model.SortBy))
            AddError("Geçerli bir ürün sıralama alanı seçiniz.");

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
