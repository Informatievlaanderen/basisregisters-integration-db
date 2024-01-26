namespace Basisregisters.IntegrationDb.NationalRegistry.Tests
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using FluentAssertions;
    using Model;
    using Moq;
    using Repositories;
    using Xunit;

    public class RecordValidatorTests
    {
        private const string ValidPostalCode = "1000";

        private readonly FlatFileRecordValidator _sut;
        private readonly Fixture _fixture;

        public RecordValidatorTests()
        {
            var postalCodeRepository = new Mock<IPostalCodeRepository>();
            postalCodeRepository.Setup(x => x.GetAllPostalCodes()).Returns(new List<string> { ValidPostalCode });

            _sut = new FlatFileRecordValidator(postalCodeRepository.Object);
            _fixture = new Fixture();
            _fixture.Customize(new FlatFileRecordFixture());
        }

        [Fact]
        public void GivenRecordWithMissingPostalCode_WhenValidating_ThenMissingPostalCodeIsReturned()
        {
            var record = new FlatFileRecordBuilder()
                .WithPostalCode(null)
                .Build();

            var result = _sut.Validate(record);

            result.Should().Be(FlatFileRecordErrorType.MissingPostalCode);
        }

        [Theory]
        [InlineData("100")]
        [InlineData("100A")]
        public void GivenRecordWithInvalidPostalCode_WhenValidating_ThenInvalidPostalCodeIsReturned(string postalCode)
        {
            var record = new FlatFileRecordBuilder()
                .WithPostalCode(postalCode)
                .Build();

            var result = _sut.Validate(record);

            result.Should().Be(FlatFileRecordErrorType.InvalidPostalCode);
        }

        [Fact]
        public void GivenRecordWithPostalCodeNotFound_WhenValidating_ThenPostalCodeNotFoundIsReturned()
        {
            var record = new FlatFileRecordBuilder()
                .WithPostalCode("9999")
                .Build();

            var result = _sut.Validate(record);

            result.Should().Be(FlatFileRecordErrorType.PostalCodeNotFound);
        }

        [Fact]
        public void GivenRecordWithMissingStreetCode_WhenValidating_ThenMissingStreetCodeIsReturned()
        {
            var record = new FlatFileRecordBuilder()
                .WithPostalCode(ValidPostalCode)
                .WithStreetCode(null)
                .Build();

            var result = _sut.Validate(record);

            result.Should().Be(FlatFileRecordErrorType.MissingStreetCode);
        }

        [Theory]
        [InlineData("100")]
        [InlineData("100A")]
        public void GivenRecordWithInvalidStreetCode_WhenValidating_ThenInvalidStreetCodeIsReturned(string streetCode)
        {
            var record = new FlatFileRecordBuilder()
                .WithPostalCode(ValidPostalCode)
                .WithStreetCode(streetCode)
                .Build();

            var result = _sut.Validate(record);

            result.Should().Be(FlatFileRecordErrorType.InvalidStreetCode);
        }

        [Fact]
        public void GivenValidRecord_WhenValidating_ThenNullIsReturned()
        {
            var record = new FlatFileRecordBuilder()
                .WithPostalCode(ValidPostalCode)
                .Build();

            var result = _sut.Validate(record);

            result.Should().BeNull();
        }
    }

    public sealed class FlatFileRecordBuilder
    {
        private readonly FlatFileRecord _record;

        public FlatFileRecordBuilder()
        {
            var fixture = new Fixture();
            fixture.Customize(new FlatFileRecordFixture());

            _record = fixture.Create<FlatFileRecord>();
        }

        public FlatFileRecordBuilder WithNisCode(string nisCode)
        {
            _record.NisCode = nisCode;
            return this;
        }

        public FlatFileRecordBuilder WithPostalCode(string postalCode)
        {
            _record.PostalCode = postalCode;
            return this;
        }

        public FlatFileRecordBuilder WithStreetCode(string streetCode)
        {
            _record.StreetCode = streetCode;
            return this;
        }

        public FlatFileRecordBuilder WithHouseNumber(string houseNumber)
        {
            _record.HouseNumber = houseNumber;
            return this;
        }

        public FlatFileRecordBuilder WithIndex(string index)
        {
            _record.Index = index;
            return this;
        }

        public FlatFileRecordBuilder WithStreetName(string streetName)
        {
            _record.StreetName = streetName;
            return this;
        }

        public FlatFileRecordBuilder WithRegisteredCount(int registeredCount)
        {
            _record.RegisteredCount = registeredCount;
            return this;
        }

        public FlatFileRecord Build()
        {
            return _record;
        }
    }

    public sealed class FlatFileRecordFixture : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<FlatFileRecord>(composer =>
                composer
                    .With(x => x.NisCode, () => GenerateDigitNumberString(5))
                    .With(x => x.PostalCode, () => GenerateDigitNumberString(4))
                    .With(x => x.StreetCode, () => GenerateDigitNumberString(4))
                    .With(x => x.HouseNumber, () => GenerateDigitNumberString(2))
                    .With(x => x.Index, "A" + GenerateDigitNumberString(3))
                    .With(x => x.StreetName, fixture.Create<string>())
                    .With(x => x.RegisteredCount, fixture.Create<int>())
            );
        }

        private static string GenerateDigitNumberString(int digitCount)
        {
            var random = new Random();
            var value = random.Next((int)Math.Pow(10, digitCount - 1), (int)Math.Pow(10, digitCount) - 1);
            return value.ToString(new string('0', digitCount));
        }
    }
}
