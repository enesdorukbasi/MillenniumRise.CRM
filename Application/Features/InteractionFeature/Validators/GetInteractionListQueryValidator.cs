using Application.Common.Validators;
using Application.Extensions;
using Application.Features.InteractionFeature.Queries.GetList;

namespace Application.Features.InteractionFeature.Validators;

public sealed class GetInteractionListQueryValidator : BaseValidator<GetInteractionListQuery>
{
    public override bool Validate(GetInteractionListQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Etkileşim listeleme bilgileri zorunludur.");
            return false;
        }

        if (model.CustomerProductId == Guid.Empty)
            AddError("Geçerli bir müşteri ürünü kimliği giriniz.");

        if (model.Search.IsNotNullOrEmpty() && !model.Search.MaxLengthControl(300))
            AddError("Arama metni en fazla 300 karakter olabilir.");

        if (model.Status.HasValue && !Enum.IsDefined(model.Status.Value))
            AddError("Geçerli bir etkileşim durumu seçiniz.");

        if (model.Channel.HasValue && !Enum.IsDefined(model.Channel.Value))
            AddError("Geçerli bir etkileşim kanalı seçiniz.");

        if (model.Outcome.HasValue && !Enum.IsDefined(model.Outcome.Value))
            AddError("Geçerli bir etkileşim sonucu seçiniz.");

        if (model.StartDate.HasValue && model.EndDate.HasValue && model.StartDate.Value > model.EndDate.Value)
            AddError("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        if (!Enum.IsDefined(model.SortBy))
            AddError("Geçerli bir etkileşim sıralama alanı seçiniz.");

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
