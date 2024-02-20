namespace Basisregisters.IntegrationDb.NationalRegistry.Model
{
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using FlatFiles;
    using FlatFiles.TypeMapping;

    public sealed class FlatFileRecord
    {
        public string NisCode { get; set; }
        public string PostalCode { get; set; }
        public string StreetCode { get; set; }
        public string HouseNumber { get; set; }

        public NationalRegistryIndex Index { get; set; }

        public string StreetName { get; set; }
        public int RegisteredCount { get; set; }

        public bool HasIndex => !string.IsNullOrEmpty(Index) && Index != "0000";

        public int RecordNumber { get; set; }

        // public static FixedLengthSchema Schema
        // {
        //     get
        //     {
        //         var schema = new FixedLengthSchema();
        //         schema.AddColumn(new StringColumn("NisCode"), 5);
        //         schema.AddColumn(new StringColumn("PostalCode"), 4);
        //         schema.AddColumn(new StringColumn("StreetCode"), 4);
        //         schema.AddColumn(new StringColumn("HouseNumber"), 4);
        //         schema.AddColumn(new StringColumn("Index"), 4);
        //         schema.AddColumn(new StringColumn("StreetName"), 32);
        //         schema.AddColumn(new Int32Column("RegisteredCount"), 4);
        //         return schema;
        //     }
        // }

        public static IFixedLengthTypeMapper<FlatFileRecord> Mapper
        {
            get
            {
                var mapper = FixedLengthTypeMapper.Define<FlatFileRecord>();
                mapper.Property(x => x.NisCode, 5);
                mapper.Property(x => x.PostalCode, 4);
                mapper.Property(x => x.StreetCode, 4);
                mapper.Property(x => x.HouseNumber, 4);
                mapper
                    .CustomMapping(new StringColumn("Index"), 4)
                    .WithReader((x, y) => x.Index = new NationalRegistryIndex(y?.ToString()));
                mapper.Property(x => x.StreetName, 32);
                mapper.Property(x => x.RegisteredCount, 4);
                mapper
                    .CustomMapping(new RecordNumberColumn("RecordNumber"), Window.Trailing)
                    .WithReader(x => x.RecordNumber);
                return mapper;
            }
        }
    }

    [DebuggerDisplay("{Value}; Source={SourceValue}")]
    public sealed class NationalRegistryIndex
    {
        private static readonly Regex BeginAlfaNumericRegex = new Regex("^[0-9]+|^[a-zA-Z]+", RegexOptions.Compiled);
        private static readonly Regex IsNumericRegex = new Regex("^[0-9]+$", RegexOptions.Compiled);
        private static readonly Regex BeginCharRegex = new Regex("^[a-zA-Z]+");
        public string? SourceValue { get; }

        public string? Value { get; }

        public string? Left { get; }
        public string? Right { get; }
        public string? RightPartOne { get; }
        public string? RightPartTwo { get; }

        public bool HasIndex => Value is not null;

        public NationalRegistryIndex(string? value)
        {
            SourceValue = value;
            if (string.IsNullOrEmpty(value) || value == "0000") //TODO: possible improvement if we consider 0000 as value and not clear values.
            {
                Value = null;
                Left = null;
                Right = null;
                RightPartOne = null;
                RightPartTwo = null;
            }
            else
            {
                var formatted = value.TrimStart('0').Trim();
                Value = formatted.Length > 0 && IsNumericRegex.IsMatch(formatted[0].ToString())
                    ? formatted.PadLeft(4, '0')
                    : formatted.PadRight(4, '0');

                Left = GetLeft(Value);
                Right = GetRight(Value);

                if (!string.IsNullOrEmpty(Right))
                {
                    var tempP1 = GetLeft(Right);
                    var tempP2 = GetRight(Right);

                    if (IsNumericRegex.IsMatch(Value[0].ToString()) &&
                        (string.IsNullOrEmpty(tempP1) ||
                         (IsNumericRegex.IsMatch(tempP1) && string.IsNullOrEmpty(tempP2)) ||
                         (BeginCharRegex.IsMatch(tempP1) && IsNumericRegex.IsMatch(tempP2)) ||
                         (BeginCharRegex.IsMatch(tempP1) && string.IsNullOrEmpty(tempP2))))
                    {
                        RightPartOne = tempP1;
                        RightPartTwo = tempP2;
                    }
                }
            }
        }

        private static string? GetLeft(string input) =>
            BeginAlfaNumericRegex.IsMatch(input) ? BeginAlfaNumericRegex.Match(input).Value : null;

        private static string? GetRight(string input)
        {
            string? result = null;
            if (BeginAlfaNumericRegex.IsMatch(input))
                result = input.Substring(BeginAlfaNumericRegex.Match(input).Value.Length);

            if (!string.IsNullOrEmpty(result) && result.Length > 0 && !BeginAlfaNumericRegex.IsMatch(result))
                result = result.Substring(1);

            return result;
        }

        public static implicit operator string?(NationalRegistryIndex index)
        {
            return index.Value;
        }

        public override string ToString()
        {
            return Value ?? string.Empty;
        }
    }
}
