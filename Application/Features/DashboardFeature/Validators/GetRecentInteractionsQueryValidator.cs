using Application.Common.Validators;
using Application.Extensions;
using Application.Features.DashboardFeature.Queries.GetRecentInteractions;

namespace Application.Features.DashboardFeature.Validators;

public sealed class GetRecentInteractionsQueryValidator : BaseValidator<GetRecentInteractionsQuery>
{
    public override bool Validate(GetRecentInteractionsQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Son etkileşim sorgu bilgileri zorunludur.");
            return false;
        }

        if (model.PageNumber < 1)
            AddError("Sayfa numarası en az 1 olmalıdır.");

        if (model.PageSize is < 1 or > 100)
            AddError("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

        if (model.PageNumber > 0 && model.PageSize > 0 && (long)(model.PageNumber - 1) * model.PageSize > int.MaxValue)
            AddError("İstenen sayfa aralığı desteklenen sınırı aşıyor.");

        return IsValid;
    }
}
