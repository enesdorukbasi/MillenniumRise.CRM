using Application.Common.Validators;
using Application.Extensions;
using Application.Features.CustomerProductFeature.Queries.GetList;

namespace Application.Features.CustomerProductFeature.Validators;

public sealed class GetCustomerProductListQueryValidator : BaseValidator<GetCustomerProductListQuery>
{
    public override bool Validate(GetCustomerProductListQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Müşteri ürünü listeleme bilgileri zorunludur.");
            return false;
        }

        if (model.Search.IsNotNullOrEmpty() && !model.Search.MaxLengthControl(300))
            AddError("Arama metni en fazla 300 karakter olabilir.");

        if (model.CustomerId == Guid.Empty)
            AddError("Geçerli bir müşteri kimliği giriniz.");

        if (model.ProductId == Guid.Empty)
            AddError("Geçerli bir ürün kimliği giriniz.");

        if (model.Status.HasValue && !Enum.IsDefined(model.Status.Value))
            AddError("Geçerli bir müşteri ürünü durumu seçiniz.");

        if (model.NextFollowUpFrom.HasValue && model.NextFollowUpTo.HasValue && model.NextFollowUpFrom.Value > model.NextFollowUpTo.Value)
            AddError("Takip başlangıç tarihi takip bitiş tarihinden sonra olamaz.");

        if (!Enum.IsDefined(model.SortBy))
            AddError("Geçerli bir müşteri ürünü sıralama alanı seçiniz.");

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
