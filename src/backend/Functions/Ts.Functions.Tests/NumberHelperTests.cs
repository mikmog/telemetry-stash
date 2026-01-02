using TelemetryStash.Functions.TelemetryTrigger.Logic;

namespace TelemetryStash.Functions.Tests;

public class NumberHelperTests
{
    [Theory]
    [InlineData(1.00000000000000000000000000000000, "1")]
    [InlineData(10.10000, "10.1")]
    [InlineData(10.00001, "10.00001")]
    [InlineData(10.10001, "10.10001")]
    [InlineData(00.0000, "0")]
    [InlineData(10, "10")]
    [InlineData(-10, "-10")]
    [InlineData(1.12345, "1.12345")]
    public void RemoveTrailingZeroes_Should_remove_trailing_zeroes(decimal input, string expected)
    {
        // Act
        var result = NumberHelper.ToStringWithoutTrailingZeroes(input);

        // Assert
        Assert.Equal(expected, result);
    }
}
