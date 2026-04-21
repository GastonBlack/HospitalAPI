using System.Globalization;

namespace HospitalAPI.Infrastructure.Formatting;

public static class NameFormatter
{
    public static string ToTitleCase(string value)
    {
        var normalizedValue = value.Trim().ToLowerInvariant();
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalizedValue);
    }
}
