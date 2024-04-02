namespace Basisregisters.IntegrationDb.NationalRegistry.Tests
{
    using System.Linq;
    using FluentAssertions;
    using Model;
    using Xunit;

    public class MunicipalityIndexTests
    {
        [Theory]
        [InlineData("0002", "b001", "2", "1")]
        [InlineData("0009", "0001", "9_1", null)]
        [InlineData("0011", "B000", "11", "B")]
        [InlineData("0011", "A001", "11", "A1")]
        public void Turnhout(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "13040",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0002", "B  0", "2", "0")]
        [InlineData("0002", "B 1 ", "2", "1")]
        [InlineData("0002", "B  1", "2", "1")]
        [InlineData("0002", "B 10", "2", "10")]
        [InlineData("0002", "B101", "2", "101")]
        [InlineData("0002", "AB 1", "2A", "1")]
        [InlineData("0002", "AB01", "2A", "1")]
        [InlineData("0002", "AB10", "2A", "10")]
        public void Lier(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "12021",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B001", "46", "B1")]
        [InlineData("0046", "D000", "46", "D")]
        public void Aartselaar(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11001",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0002", "V1R ", "2", "V1R")]
        [InlineData("0002", "GVLL", "2", "GVLL")]
        [InlineData("0002", "GVLR", "2", "GVLR")]
        [InlineData("0002", "GVL ", "2", "GVL")]
        [InlineData("0002", "V1L ", "2", "V1L")]
        [InlineData("0002", "V1R ", "2", "V1R")]
        [InlineData("0002", "V3  ", "2", "V3")]
        public void Wommelgem(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11052",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B1  ", "46", "B1")]
        public void Hemiksem(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11018",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "00/1", "46", "00/1")]
        [InlineData("0046", "05/1", "46", "05/1")]
        [InlineData("0046", "A1/1", "46A", "1/1")]
        public void Vilvoorde(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23088",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0002", "1e/A", "2", "1A")]
        [InlineData("0002", "2e/F", "2", "2F")]
        [InlineData("0002", "RCH ", "2", "RCH")]
        public void Drogenbos(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23098",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0002", "gelR", "2", "gelR")]
        [InlineData("0002", "gelL", "2", "gelL")]
        [InlineData("0002", "gelA", "2", "gelA")]
        [InlineData("0002", "gel0", "2", "0.0")]
        [InlineData("0002", "007L", "2", "007L")]
        [InlineData("0002", "007R", "2", "OO7R")]
        [InlineData("0002", "02LA", "2", "02LA")]
        public void Borsbeek(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11007",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B001", "46", "B001")]
        [InlineData("0046", "Ab01", "46", "Ab01")]
        [InlineData("0046", "Db05", "46", "Db05")]
        [InlineData("0046", "Cb03", "46", "Cb03")]
        public void Grimbergen(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23025",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers!.GetValues();

            result.Should().ContainSingle();
            result.Single().HouseNumber.Should().Be(expectedHouseNumber);
            result.Single().BoxNumber.Should().Be(expectedBoxNumber);
        }
    }
}
