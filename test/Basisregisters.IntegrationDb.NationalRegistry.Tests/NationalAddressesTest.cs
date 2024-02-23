namespace Basisregisters.IntegrationDb.NationalRegistry.Tests
{
    using FluentAssertions;
    using Model;
    using Xunit;

    public class NationalAddressesTest
    {
        [Theory]
        [InlineData("0")]
        [InlineData("00")]
        [InlineData("000")]
        [InlineData("0000")]
        [InlineData("")]
        [InlineData(null)]
        public void WithNoIndex_ThenHouseNumber(string? index)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumber.Should().Be(record.HouseNumber);
            address.BoxNumber.Should().BeNull();
        }

        [Theory]
        [InlineData("A000", "A")]
        [InlineData("B000", "B")]
        [InlineData("AB00", "AB")]
        [InlineData("AB", "AB")]
        public void WithIndexLeftIsLetterAndRightOnlyZeroes_ThenHouseNumberHasLetters(string? index, string houseNumberSuffix)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumber.Should().Be(record.HouseNumber + houseNumberSuffix);
            address.BoxNumber.Should().BeNull();
        }

        [Theory]
        [InlineData("A001", "A", "1")]
        [InlineData("B010", "B", "10")]
        [InlineData("AB20", "AB", "20")]
        [InlineData("AB05", "AB", "5")]
        public void WithLeftIndexIsLetterAndRightIndexNumberHigherThanZero_ThenHouseNumberHasLettersAndBoxNumber(
            string? index,
            string houseNumberSuffix,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumber.Should().Be(record.HouseNumber + houseNumberSuffix);
            address.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("Ap.6", "6")]
        [InlineData("Vrd6", "6")]
        [InlineData("bus6", "6")]
        public void WithLeftIndexRefersToAppartment_ThenHouseNumberAndBoxNumber(
            string? index,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumber.Should().Be(record.HouseNumber);
            address.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("0003", "3")]
        [InlineData("03", "3")]
        [InlineData("3", "3")]
        public void WithOnlyLeftIndexWhichIsNumeric_ThenHouseNumberAndBoxNumber(
            string? index,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumber.Should().Be(record.HouseNumber);
            address.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("3.3")]
        [InlineData("00.3")]
        [InlineData("01.3")]
        [InlineData("02.3")]
        public void WithLeftIndexAndRightPartOneIsNumeric_ThenHouseNumberAndBoxNumber(
            string? index)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumber.Should().Be(record.HouseNumber);
            address.BoxNumber.Should().Be(index);
        }
    }
}
