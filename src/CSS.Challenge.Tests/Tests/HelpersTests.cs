using CSS.Challenge.Domain.Utilities;
using Xunit;

namespace CSS.Challenge.UnitTests.Tests
{
    public class HelpersTests
    {
        public class HelperTests
        {
            [Theory]
            [InlineData(1, 10)]
            [InlineData(5, 6)]
            [InlineData(20, 25)]
            public void SetPickupTime_ReturnsValueWithinRange(int min, int max)
            {
                // Act
                int result = Helpers.GeneratePickupTime(min, max);

                // Assert
                Assert.InRange(result, min, max - 1);
            }

            [Fact]
            public void SetPickupTime_ThrowsException_WhenMinGreaterThanMax()
            {
                // Arrange
                int min = 20;
                int max = 10;

                // Act & Assert
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    Helpers.GeneratePickupTime(min, max)
                );
            }
        }

}
}
