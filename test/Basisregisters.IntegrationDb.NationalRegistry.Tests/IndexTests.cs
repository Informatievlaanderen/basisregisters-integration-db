namespace Basisregisters.IntegrationDb.NationalRegistry.Tests
{
    using FluentAssertions;
    using Model;
    using Xunit;

    public class NationalRegistryIndexTests
    {
        [Theory]
        [InlineData("1", "0001")]
        [InlineData("10", "0010")]
        [InlineData("101", "0101")]
        [InlineData("1058", "1058")]
        [InlineData("1000", "1000")]
        [InlineData("01", "0001")]
        [InlineData("0", "0000")]
        [InlineData("00", "0000")]
        [InlineData("000", "0000")]
        public void IndexIsLeftPaddedWithZeros(string index, string expected)
        {
            var record = new NationalRegistryIndex(index);
            record.Value.Should().Be(expected);
            record.SourceValue.Should().Be(index);
        }

        [Theory]
        [InlineData("0A", "A000")]
        [InlineData("00AB", "AB00")]
        [InlineData("A", "A000")]
        [InlineData("AB", "AB00")]
        [InlineData("A000", "A000")]
        [InlineData("A001", "A001")]
        public void IndexIsRightPaddedWithZeros(string index, string expected)
        {
            var record = new NationalRegistryIndex(index);
            record.Value.Should().Be(expected);
            record.SourceValue.Should().Be(index);
        }

        [Fact]
        public void IndexHasNoValueWithEmptyString()
        {
            var record = new NationalRegistryIndex(string.Empty);
            record.HasIndex.Should().BeFalse();
            record.SourceValue.Should().BeNullOrEmpty();
            record.Value.Should().BeNullOrEmpty();
            record.Left.Should().BeNullOrEmpty();
            record.Right.Should().BeNullOrEmpty();
            record.RightPartOne.Should().BeNullOrEmpty();
            record.RightPartTwo.Should().BeNullOrEmpty();
        }

        [Fact]
        public void IndexHasNoValueWithAllZeroes()
        {
            var record = new NationalRegistryIndex("0000");
            record.HasIndex.Should().BeFalse();
            record.SourceValue.Should().Be("0000");
            record.Value.Should().BeNullOrEmpty();
            record.Left.Should().BeNullOrEmpty();
            record.Right.Should().BeNullOrEmpty();
            record.RightPartOne.Should().BeNullOrEmpty();
            record.RightPartTwo.Should().BeNullOrEmpty();
        }

        [Fact]
        public void IndexHasValueWithNonZeroes()
        {
            var record = new NationalRegistryIndex("0001");
            record.HasIndex.Should().BeTrue();
            record.SourceValue.Should().Be("0001");
        }

        [Theory]
        [InlineData("0(1)", "0(1)")]
        public void IndexHasBrackets(string index, string expected)
        {
            var record = new NationalRegistryIndex(index);
            record.SourceValue.Should().Be(index);
            record.Value.Should().Be(expected);
            record.Left.Should().Be("0");
            record.Right.Should().Be("1)");
            record.RightPartOne.Should().Be("1");
            record.RightPartTwo.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("0001", "0001")]
        [InlineData("1", "0001")]
        public void IndexIsOnePart(string index, string expected)
        {
            var record = new NationalRegistryIndex(index);
            record.SourceValue.Should().Be(index);
            record.Value.Should().Be(expected);
            record.Left.Should().Be(expected);
            record.Right.Should().BeNullOrEmpty();
            record.RightPartOne.Should().BeNullOrEmpty();
            record.RightPartTwo.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("A000", "A000", "A", "000")]
        [InlineData("A001", "A001", "A", "001")]
        [InlineData("B002", "B002", "B", "002")]
        [InlineData("Ap.6", "Ap.6", "Ap", "6")]
        [InlineData("Vrd3", "Vrd3", "Vrd", "3")]
        [InlineData("bus2", "bus2", "bus", "2")]
        public void IndexWithTwoParts(
            string index,
            string expectedValue,
            string expectedLeft,
            string expectedRight)
        {
            var record = new NationalRegistryIndex(index);
            record.SourceValue.Should().Be(index);
            record.Value.Should().Be(expectedValue);
            record.Left.Should().Be(expectedLeft);
            record.Right.Should().Be(expectedRight);
            record.RightPartOne.Should().BeNullOrEmpty();
            record.RightPartTwo.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("01.2", "01.2", "01", "2", "2")]
        [InlineData("3.3",  "03.3", "03", "3", "3")]
        [InlineData("01.3", "01.3", "01", "3", "3")]
        [InlineData("001B", "001B", "001", "B", "B")]
        [InlineData("2L", "002L", "002", "L", "L")]
        [InlineData("1ev", "01ev", "01", "ev", "ev")]
        [InlineData("1vrd", "1vrd", "1", "vrd", "vrd")]
        [InlineData("1stR", "1stR", "1", "stR", "stR")]
        [InlineData("00.3", "00.3", "00", "3", "3")]
        public void IndexWithThreeParts(
            string index,
            string expectedValue,
            string expectedLeft,
            string expectedRight,
            string expectedRightPartOne)
        {
            var record = new NationalRegistryIndex(index);
            record.SourceValue.Should().Be(index);
            record.Value.Should().Be(expectedValue);
            record.Left.Should().Be(expectedLeft);
            record.Right.Should().Be(expectedRight);
            record.RightPartOne.Should().Be(expectedRightPartOne);
            record.RightPartTwo.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData("2bu1", "2bu1", "2", "bu1", "bu", "1")]
        [InlineData("1B01", "1B01", "1", "B01", "B", "01")]
        [InlineData("1V.1", "1V.1", "1", "V.1", "V", "1")]
        [InlineData("1B1", "01B1", "01", "B1", "B", "1")]
        [InlineData("A01", "0A01", "0", "A01", "A", "01")]
        [InlineData("A00", "0A00", "0", "A00", "A", "00")]
        public void IndexWithFourParts(
            string index,
            string expectedValue,
            string expectedLeft,
            string expectedRight,
            string expectedRightPartOne,
            string expectedRightPartTwo)
        {
            var record = new NationalRegistryIndex(index);
            record.SourceValue.Should().Be(index);
            record.Value.Should().Be(expectedValue);
            record.Left.Should().Be(expectedLeft);
            record.Right.Should().Be(expectedRight);
            record.RightPartOne.Should().Be(expectedRightPartOne);
            record.RightPartTwo.Should().Be(expectedRightPartTwo);
        }
    }
}
