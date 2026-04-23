using System.Globalization;

namespace HospitalAPI.Infrastructure.Formatting;

public static class NameFormatter
{
    public static string ToTitleCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be empty.", nameof(value));

        var normalizedValue = value.Trim().ToLowerInvariant();
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalizedValue);
    }
}
