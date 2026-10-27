using Application.Common.Validators;
using Application.Extensions;
using Application.Features.DashboardFeature.Queries.GetUpcomingFollowUps;

namespace Application.Features.DashboardFeature.Validators;

public sealed class GetUpcomingFollowUpsQueryValidator : BaseValidator<GetUpcomingFollowUpsQuery>
{
    public override bool Validate(GetUpcomingFollowUpsQuery model)
    {
        ClearErrors();

        if (model.IsNull())
        {
            AddError("Yaklaşan takip sorgu bilgileri zorunludur.");
            return false;
        }

        if (model.StartDate.HasValue && model.EndDate.HasValue && model.StartDate.Value > model.EndDate.Value)
            AddError("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        if (model.PageNumber < 1)
            AddError("Sayfa numarası en az 1 olmalıdır.");

        if (model.PageSize is < 1 or > 100)
            AddError("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

        if (model.PageNumber > 0 && model.PageSize > 0 && (long)(model.PageNumber - 1) * model.PageSize > int.MaxValue)
            AddError("İstenen sayfa aralığı desteklenen sınırı aşıyor.");

        return IsValid;
    }
}
