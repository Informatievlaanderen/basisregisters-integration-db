namespace Basisregisters.IntegrationDb.NationalRegistry.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Model;
    using Model.HouseNumberBoxNumberImplementations;
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
        public void NoIndex(string? index)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<NoIndex>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().BeNull();
        }

        [Theory]
        [InlineData("bis1", "1")]
        [InlineData("bisA", "A")]
        [InlineData("Abis", "A")]
        [InlineData("BISa", "a")]
        [InlineData("terA", "A")]
        public void BisIndication(string? index, string bisNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<BisIndication>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber + bisNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().BeNull();
        }

        [Theory]
        [InlineData("A000", "A")]
        [InlineData("B000", "B")]
        [InlineData("AB00", "AB")]
        [InlineData("AB  ", "AB")]
        public void NonNumericFollowedByZeros(string? index, string bisNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<NonNumericFollowedByZeros>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber + bisNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().BeNull();
        }

        [Theory]
        [InlineData("A001", "A", "1")]
        [InlineData("B010", "B", "10")]
        [InlineData("AB20", "AB", "20")]
        [InlineData("AB05", "AB", "5")]
        [InlineData("ABU2", "A", "2")]
        [InlineData("Abu2", "A", "2")]
        [InlineData("AbT2", "A", "2")]
        [InlineData("Aap2", "A", "2")]
        public void NonNumericFollowedByNumberGreaterThanZero(
            string? index,
            string bisNumber,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<NonNumericFollowedByNumberGreaterThanZero>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber + bisNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("Ap.6", "6")]
        [InlineData("Ap06", "6")]
        [InlineData("App6", "6")]
        [InlineData("Apt6", "6")]
        [InlineData("Vrd6", "Vrd6")]
        [InlineData("Vd06", "Vd06")]
        [InlineData("Vd6L", "Vd6L")]
        [InlineData("eme6", "eme6")]
        [InlineData("dev6", "dev6")]
        [InlineData("gvl6", "gvl6")]
        [InlineData("glv6", "glv6")]
        [InlineData("hal6", "hal6")]
        [InlineData("bus6", "6")]
        [InlineData("bte6", "6")]
        [InlineData("bt06", "6")]
        [InlineData("bu06", "6")]
        public void SpecificPrefix(
            string? index,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<SpecificPrefix>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("bus6", "6")]
        [InlineData("bte6", "6")]
        [InlineData("bt06", "6")]
        [InlineData("bu06", "6")]
        public void SpecificPrefix_BoxNumberIndication_ReturnsTwoHouseNumberBoxNumberValues(
            string? index,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<SpecificPrefix>();
            var houseNumberWithBoxNumbers = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();
            houseNumberWithBoxNumbers.Should().HaveCount(2);
            houseNumberWithBoxNumbers.First().HouseNumber.Should().Be(record.HouseNumber);
            houseNumberWithBoxNumbers.First().BoxNumber.Should().Be(boxNumber);
            houseNumberWithBoxNumbers.Last().HouseNumber.Should().Be(record.HouseNumber);
            houseNumberWithBoxNumbers.Last().BoxNumber.Should().Be(index);
        }

        [Theory]
        [InlineData("0003", "3")]
        [InlineData("03", "3")]
        [InlineData("3", "3")]
        public void NumbersOnly(
            string? index,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<NumbersOnly>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("3.3")]
        [InlineData("00.3")]
        [InlineData("01.3")]
        [InlineData("02.3")]
        public void SeparatorBetweenNumbers(
            string? index)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<SeparatorBetweenNumbers>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().Be(index);
        }

        [Theory]
        [InlineData("1B01", "B", "1")]
        [InlineData("1A01", "A", "1")]
        [InlineData("A01", "A", "1")]
        [InlineData("A00", "A", "0")]
        public void NonNumericBetweenNumbers(
            string? index,
            string bisNumber,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<NonNumericBetweenNumbers>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber + bisNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("1V.2", "1.2")]
        [InlineData("2V.1", "2.1")]
        [InlineData("2VR1", "2.1")]
        public void NonNumericBetweenNumbers_FloorNumberWithDot(
            string? index,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<NonNumericBetweenNumbers>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("001B", "_1", "B")]
        [InlineData("2L", "_2", "L")]
        public void NumericFollowedByNonNumeric(
            string? index,
            string bisNumber,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<NumericFollowedByNonNumeric>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber + bisNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().Be(boxNumber);
        }

        [Theory]
        [InlineData("1ev", "1.0")]
        [InlineData("1vrd", "1.0")]
        public void NumericFollowedBySpecificSuffix(
            string? index,
            string boxNumber)
        {
            var record = new FlatFileRecord
            {
                HouseNumber = "123",
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            address.HouseNumberBoxNumbers.First().Should().BeOfType<NumericFollowedBySpecificSuffix>();
            var houseNumberWithBoxNumber = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).First();
            houseNumberWithBoxNumber.HouseNumber.Should().Be(record.HouseNumber);
            houseNumberWithBoxNumber.BoxNumber.Should().Be(boxNumber);
        }
    }
}
