namespace Basisregisters.IntegrationDb.NationalRegistry.Tests
{
    using System;
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

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
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

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
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

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
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

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
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

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "00/1", "46", "00/1")]
        [InlineData("0046", "05/1", "46", "05/1")]
        [InlineData("0046", "A1/1", "46A", "1/1")]
        [InlineData("0046", "O/01", "46", "O/01")]
        public void Vilvoorde(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23088",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0002", "1e/A", "2", "1A")]
        [InlineData("0002", "2e/F", "2", "2F")]
        [InlineData("0002", "RCH ", "2", "RCH ")]
        [InlineData("0002", "1e/E", "2", "1.0")]
        [InlineData("0002", "2e/E", "2", "2.0")]
        [InlineData("0002", "0201", "2", "0201")]
        [InlineData("0002", " RCH", "2", " RCH")]
        public void Drogenbos(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23098",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0002", "gelR", "2", "gelR")]
        [InlineData("0002", "gelL", "2", "gelL")]
        [InlineData("0002", "gelA", "2", "gelA")]
        [InlineData("0002", "gel0", "2", "0.0")]
        [InlineData("0002", "007L", "2", "007L")]
        [InlineData("0002", "007R", "2", "007R")]
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

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B001", "46", "1")]
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

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "b  1", "46", "001")]
        [InlineData("0046", "b 4 ", "46", "004")]
        [InlineData("0002", "b101", "2", "101")]
        [InlineData("0002", "b201", "2", "201")]
        [InlineData("0002", "Fb01", "2F", "001")]
        [InlineData("0002", "Eb11", "2E", "011")]
        [InlineData("0002", "1b 7", "2_1", "007")]
        [InlineData("0002", "CB01", "2C", "001")]
        [InlineData("0002", "CB13", "2C", "013")]
        [InlineData("0002", "0001", "2_1", null)]
        [InlineData("0002", "0002", "2_2", null)]
        public void Beveren(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "46003",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B001", "46", "1")]
        [InlineData("0046", "B001", "46", "B001")]
        [InlineData("0046", "V001", "46", "V1")]
        [InlineData("0046", "Glv0", "46", "0.0")]
        [InlineData("0046", "Glv ", "46", "0.0")]
        public void Zwijndrecht(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11056",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "1B", "46", "1B")]
        [InlineData("0046", "2A", "46", "2A")]
        public void Wemmel(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23102",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "01.1", "46", "01.1")]
        [InlineData("0046", "02.1", "46", "02.1")]
        [InlineData("0046", "0O.3", "46", "0.3")]
        public void Boom(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11005",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "1A  ", "46", "1A")]
        [InlineData("0046", "GLVA", "46", "GLVA")]
        [InlineData("0046", "GLVB", "46", "GLVB")]
        public void Ieper(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "33011",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "00b1", "46", "1")]
        [InlineData("0046", "00b5", "46", "5")]
        [InlineData("0046", "0b52", "46", "52")]
        public void Tongeren(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "73083",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "GL-L", "46", "GL-L")]
        [InlineData("0046", "GL-R", "46", "GL-R")]
        [InlineData("0046", "2R  ", "46", "2R")]
        [InlineData("0046", "2L  ", "46", "2L")]
        public void Lint(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11025",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "BS22", "46", "BS22")]
        public void Machelen(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23047",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B102", "46", "B102")]
        [InlineData("0046", "B10 ", "46", "B10")]
        [InlineData("0046", "B1  ", "46", "B1")]
        public void Beerse(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "13004",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "b1  ", "46", "b1")]
        [InlineData("0046", "b5  ", "46", "b5")]
        public void Zoersel(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11055",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "A000", "46", "A000")]
        [InlineData("0046", "B004", "46", "B004")]
        public void Diest(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "24020",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B001", "46", "1")]
        [InlineData("0046", "E000", "46E", null)]
        [InlineData("0046", "E201", "46E", "0201")]
        [InlineData("0046", "AB06", "46A", "6")]
        [InlineData("0046", "BB02", "46B", "2")]
        [InlineData("0046", "CB02", "46C", "2")]
        public void SintNiklaas(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "46021",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "b003", "46", "3")]
        [InlineData("0046", "B003", "46", "3")]
        [InlineData("0046", "b000", "46", "0")]
        [InlineData("0046", "b010", "46", "10")]
        [InlineData("0046", "b100", "46", "100")]
        public void Vosselaar(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "13046",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "G001", "46", "G001")]
        [InlineData("0046", "0234", "46_234", null)]
        public void DePanne(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "38008",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "V002", "46", "V2")]
        [InlineData("0046", "AV01", "46", "AV01")]
        public void Torhout(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "31033",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B002", "46", "B002")]
        [InlineData("0046", "A1.1", "46", "A1.1")]
        [InlineData("0046", "W001", "46", "W1")]
        [InlineData("0046", "W012", "46", "W012")]
        [InlineData("0046", "AW01", "46A", "W1")]
        [InlineData("0046", "BW01", "46B", "W1")]
        [InlineData("0046", "A000", "46A", null)]
        [InlineData("0046", "G/02", "46G", "02")]
        [InlineData("0046", "G/2 ", "46G", "2")]
        public void Zele(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "42028",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "B002", "46", "B002")]
        [InlineData("0046", "A1.1", "46", "A1.1")]
        [InlineData("0046", "W001", "46", "W1")]
        [InlineData("0046", "W012", "46", "W012")]
        [InlineData("0046", "AW01", "46A", "W1")]
        [InlineData("0046", "BW01", "46B", "W1")]
        [InlineData("0046", "A000", "46A", null)]
        [InlineData("0046", "G/02", "46G", "02")]
        [InlineData("0046", "G/2 ", "46G", "2")]
        public void Halen(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "71020",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "00OB", "46", "00OB")]
        [InlineData("0046", "001A", "46", "1A")]
        [InlineData("0046", "E000", "46E", null)]
        public void Aalter(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "44084",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "02-1", "46", "02-1")]
        public void Ravels(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "13035",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "E000", "46E", null)]
        [InlineData("0046", "B002", "46", "2")]
        [InlineData("0046", "b002", "46", "2")]
        [InlineData("0046", "B012", "46", "12")]
        [InlineData("0046", "B/B2", "46B", "2")]
        [InlineData("0046", "A/B2", "46A", "2")]
        [InlineData("0046", "A/02", "46A", "2")]
        [InlineData("0046", "A/12", "46A", "12")]
        [InlineData("0046", "AB03", "46A", "3")]
        [InlineData("0046", "AB23", "46A", "23")]
        [InlineData("0046", "BB23", "46B", "23")]
        public void SintGillisWaas(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "46020",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "E000", "46E", null)]
        [InlineData("0046", "B002", "46", "2")]
        [InlineData("0046", "b002", "46", "2")]
        [InlineData("0046", "B012", "46", "12")]
        [InlineData("0046", "B/B2", "46B", "2")]
        [InlineData("0046", "A/B2", "46A", "2")]
        [InlineData("0046", "A/02", "46A", "2")]
        [InlineData("0046", "A/12", "46A", "12")]
        [InlineData("0046", "AB03", "46A", "3")]
        [InlineData("0046", "AB23", "46A", "23")]
        [InlineData("0046", "BB23", "46B", "23")]
        public void Geraardsbergen(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "41018",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "E000", "46", "E000")]
        [InlineData("0046", "B015", "46", "B015")]
        public void Bornem(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "12007",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "A/1V", "46A", "1V")]
        [InlineData("0046", "GV1 ", "46", "GV1")]
        [InlineData("0046", "001A", "46", "1A")]
        [InlineData("0046", "GV.L", "46", "GV.L")]
        [InlineData("0046", "GV.R", "46", "GV.R")]
        [InlineData("0046", "GLVL", "46", "GLVL")]
        [InlineData("0046", "GV00", "46", "GV")]
        [InlineData("0046", "1V2 ", "46", "1V2")]
        public void Brecht(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11009",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "01-3", "46", "01-3")]
        [InlineData("0046", "E000", "46E", null)]
        [InlineData("0046", "0001", "46", "1")]
        public void Houthalen(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "72039",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "A   ", "46", "A")]
        public void Olen(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "13029",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "KV03", "46", "KV03")]
        [InlineData("0046", "GV01", "46", "001")]
        [InlineData("0046", "XGV1", "46", "XGV1")]
        [InlineData("0046", "K999", "46", "K999")]
        [InlineData("0010", "K012", "10", "012")]
        [InlineData("0010", "GV01", "10", "001")]
        public void Koksijde(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "38014",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            
            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "K999", "46", "K999")]
        public void Lokeren(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "46014",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "X000", "46", "X")]
        public void Nijlen(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "12026",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "glvl", "46", "glv")]
        [InlineData("0046", "glvl", "46", "glvl")]
        [InlineData("0046", "glvl", "46", "GLVL")]
        public void Mortsel(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11029",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "A000", "46", "A")]
        [InlineData("0046", "A   ", "46", "A")]
        [InlineData("0046", "   A", "46", "A")]
        public void Vorselaar(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "13044",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "GLVL", "46", "GLVL")]
        [InlineData("0046", "BVD1", "46", "BVD1")]
        [InlineData("0046", "GLLI", "46", "GLLI")]
        public void Stabroek(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11044",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", " D1 ", "46", "D1")]
        [InlineData("0046", "  1", "46", "1")]
        public void Antwerpen(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "11002",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "D1.1", "46D", "1.01")]
        [InlineData("0046", "D1.1", "46D", "1.1")]
        [InlineData("0046", "B0.1", "46B", "0.01")]
        [InlineData("0046", "B0.1", "46B", "0.1")]
        public void Hasselt(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "71022",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0010", "0803", "10", "0803")]
        public void Oostende(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "35013",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "GV  ", "46", "GV")]
        [InlineData("0046", "gv  ", "46", "gv")]
        [InlineData("0046", "GV-L", "46", "GV-L")]
        [InlineData("0046", "GV-R", "46", "GV-R")]
        [InlineData("0046", "A-b3", "46A", "3")]
        [InlineData("0046", "bs 1", "46", "1")]
        [InlineData("0046", "bs12", "46", "12")]
        [InlineData("0046", "bs01", "46", "1")]
        public void SintPietersLeeuw(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23077",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "0001", "46", "0001")]
        [InlineData("0046", "A   ", "46", "A")]
        public void Genk(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "71016",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };
            var address = new NationalRegistryAddress(record);

            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.First().HouseNumber.Should().Be(expectedHouseNumber);
            result.First().BoxNumber.Should().Be(expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "glvl", "46", "glvl")]
        [InlineData("0046", "glvl", "46", "GLVL")]
        public void Blankenberge(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "31004",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "a000", "46", "a")]
        [InlineData("0046", "A000", "46", "A")]
        [InlineData("0046", "B000", "46", "B")]
        public void Herentals(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "13011",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0046", "0001", "46", "0001")]
        [InlineData("0046", "001 ", "46", "001")]
        public void HeusdenZolder(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "71070",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0045", "0001", "45_1", null)]
        public void Izegem(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "36008",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0045", "B   ", "45", "B")]
        public void Maldegem(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "43010",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        [Theory]
        [InlineData("0056", "X123", "56", "X123")]
        public void Zaventem(string houseNumber, string index, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var record = new FlatFileRecord
            {
                NisCode = "23094",
                HouseNumber = houseNumber,
                Index = new NationalRegistryIndex(index)
            };

            OneAddressShouldMatchExpected(record, expectedHouseNumber, expectedBoxNumber);
        }

        private void OneAddressShouldMatchExpected(FlatFileRecord record, string expectedHouseNumber, string? expectedBoxNumber)
        {
            var address = new NationalRegistryAddress(record);
            var result = address.HouseNumberBoxNumbers.SelectMany(x => x.GetValues()).ToList();

            result.Should().NotBeEmpty();
            result.Count(x => x.HouseNumber == expectedHouseNumber && x.BoxNumber == expectedBoxNumber).Should().Be(1);
        }
    }
}
