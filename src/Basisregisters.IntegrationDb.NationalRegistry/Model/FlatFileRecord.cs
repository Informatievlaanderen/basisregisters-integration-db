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
            if (string.IsNullOrEmpty(value) || value == "0000")
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
                var numeric = new Regex("^[0-9]+$");
                Value = numeric.IsMatch(formatted[0].ToString())
                    ? formatted.PadLeft(4, '0')
                    : formatted.PadRight(4, '0');

                var beginAlfaNumeric = new Regex("^[0-9]+|^[a-zA-Z]+");
                if (beginAlfaNumeric.IsMatch(Value))
                    Left = beginAlfaNumeric.Match(Value).Value;

                if (beginAlfaNumeric.IsMatch(Value))
                    Right = Value.Substring(beginAlfaNumeric.Match(Value).Value.Length);

                if (!string.IsNullOrEmpty(Right) && Right.Length > 0 && !beginAlfaNumeric.IsMatch(Right))
                    Right = Right.Substring(1);

                if (!string.IsNullOrEmpty(Right))
                {
                    var beginChar = new Regex("^[a-zA-Z]+");
                    var tempP1 = string.Empty;
                    var tempP2 = string.Empty;

                    if (beginAlfaNumeric.IsMatch(Right))
                        tempP1 = beginAlfaNumeric.Match(Right).Value;

                    if (beginAlfaNumeric.IsMatch(Right))
                        tempP2 = Right.Substring(beginAlfaNumeric.Match(Right).Value.Length);

                    if (!string.IsNullOrEmpty(tempP2) && tempP2.Length > 0 && !beginAlfaNumeric.IsMatch(tempP2))
                        tempP2 = tempP2.Substring(1);

                    if (numeric.IsMatch(Value[0].ToString()) &&
                        (string.IsNullOrEmpty(tempP1) ||
                         (numeric.IsMatch(tempP1) && string.IsNullOrEmpty(tempP2)) ||
                         (beginChar.IsMatch(tempP1) && numeric.IsMatch(tempP2)) ||
                         (beginChar.IsMatch(tempP1) && string.IsNullOrEmpty(tempP2))))
                    {
                        if (beginAlfaNumeric.IsMatch(Right))
                            RightPartOne = beginAlfaNumeric.Match(Right).Value;

                        if (beginAlfaNumeric.IsMatch(Right))
                            RightPartTwo = Right.Substring(beginAlfaNumeric.Match(Right).Value.Length);

                        if (!string.IsNullOrEmpty(RightPartTwo) && RightPartTwo.Length > 0 && !beginAlfaNumeric.IsMatch(RightPartTwo))
                            RightPartTwo = RightPartTwo.Substring(1);
                    }
                }
            }
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
