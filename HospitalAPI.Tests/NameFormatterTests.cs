using HospitalAPI.Infrastructure.Formatting;

namespace HospitalAPI.Tests;

public class NameFormatterTests
{
    [Theory]
    [InlineData("juan", "Juan")]
    [InlineData("JUAN", "Juan")]
    [InlineData("  juan perez  ", "Juan Perez")]
    [InlineData("maria del carmen", "Maria Del Carmen")]
    [InlineData(" mMaria del Carmen ", "Mmaria Del Carmen")]
    public void ToTitleCase_ShouldNormalizeName(string value, string expected)
    {
        var result = NameFormatter.ToTitleCase(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ToTitleCase_ShouldThrowArgumentException_WhenValueIsEmpty(string? value)
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            NameFormatter.ToTitleCase(value!));

        Assert.Equal("value", exception.ParamName);
    }
}
