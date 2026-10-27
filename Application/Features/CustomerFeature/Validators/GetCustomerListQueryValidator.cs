using Application.Common.Validators;
using Application.Extensions;
using Application.Features.CustomerFeature.Queries.GetList;

namespace Application.Features.CustomerFeature.Validators;

public sealed class GetCustomerListQueryValidator : BaseValidator<GetCustomerListQuery>
{
    public override bool Validate(GetCustomerListQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Müşteri listeleme bilgileri zorunludur.");
            return false;
        }

        if (model.Search.IsNotNullOrEmpty() && !model.Search.MaxLengthControl(300))
            AddError("Arama metni en fazla 300 karakter olabilir.");

        if (model.CustomerType.HasValue && !Enum.IsDefined(model.CustomerType.Value))
            AddError("Geçerli bir müşteri tipi seçiniz.");

        if (model.ProductId == Guid.Empty)
            AddError("Geçerli bir ürün kimliği giriniz.");

        if (model.HasProduct.HasValue && !model.ProductId.HasValue)
            AddError("Ürün sahipliği filtresi için ürün kimliği zorunludur.");

        if (model.CreatedFrom.HasValue && model.CreatedTo.HasValue && model.CreatedFrom.Value > model.CreatedTo.Value)
            AddError("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        if (!Enum.IsDefined(model.SortBy))
            AddError("Geçerli bir müşteri sıralama alanı seçiniz.");

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
