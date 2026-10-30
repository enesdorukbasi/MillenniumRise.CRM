using Domain.Enums;

namespace Application.Features.CustomerFeature.Commands.Import;

internal static class CustomerTypeParser
{
    public static bool TryParse(string? value, out CustomerType customerType)
    {
        customerType = default;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        switch (value.Trim().ToUpperInvariant())
        {
            case "BİREYSEL":
            case "BIREYSEL":
            case "INDIVIDUAL":
            case "0":
                customerType = CustomerType.Individual;
                return true;
            case "KURUMSAL":
            case "BUSINESS":
            case "1":
                customerType = CustomerType.Business;
                return true;
            default:
                return false;
        }
    }
}
