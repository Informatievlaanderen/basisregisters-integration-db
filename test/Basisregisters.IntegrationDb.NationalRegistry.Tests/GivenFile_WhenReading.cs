namespace Basisregisters.IntegrationDb.NationalRegistry.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using Model;
    using Xunit;

    public class GivenFile_WhenReading
    {
        public static readonly string ExamplesFilePath = AppDomain.CurrentDomain.BaseDirectory + "Assets/examples";

        private readonly List<FlatFileRecord> _records;

        public GivenFile_WhenReading()
        {
            using var s = File.OpenRead(ExamplesFilePath);
            using var sr = new StreamReader(s);

            _records = FlatFileRecord.Mapper.Read(sr).ToList();
        }

        [Fact]
        public void ThenTheNumberOfRecordsIsCorrect()
        {
            _records.Count.Should().Be(131);
        }

        [Fact]
        public void ThenLastRecordHasCorrectRecordNumber()
        {
            _records.Last().RecordNumber.Should().Be(131);
        }
    }
}
