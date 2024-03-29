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
    }
}
