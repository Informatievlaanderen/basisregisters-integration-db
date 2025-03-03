namespace Basisregisters.IntegrationDb.NationalRegistry.Model.Extract
{
    using Be.Vlaanderen.Basisregisters.Shaperon;

    public class UnmatchedAddressDbaseRecord : DbaseRecord
    {
        public static readonly UnmatchedAddressDbaseSchema Schema = new UnmatchedAddressDbaseSchema();

        public DbaseCharacter RRNiscode { get; }
        public DbaseCharacter RRPostcode { get; }
        public DbaseCharacter RRStraatcode { get; }
        public DbaseCharacter RRHuisnummer { get; }
        public DbaseCharacter RRIndex { get; }
        public DbaseCharacter RRStraatnaam { get; }
        public DbaseCharacter GRARAdresID { get; }
        public DbaseCharacter GRARStraatnaamID { get; }
        public DbaseCharacter GRARNiscode { get; }
        public DbaseCharacter GRARPostcode { get; }
        public DbaseCharacter GRARStraatnaam { get; }
        public DbaseCharacter GRARHuisnummer { get; }
        public DbaseCharacter Precisie { get; }
        public DbaseInt32 Inwoners { get; }

        public UnmatchedAddressDbaseRecord()
        {
            RRNiscode = new DbaseCharacter(Schema.RRNiscode);
            RRPostcode = new DbaseCharacter(Schema.RRPostcode);
            RRStraatcode = new DbaseCharacter(Schema.RRStraatcode);
            RRHuisnummer = new DbaseCharacter(Schema.RRHuisnummer);
            RRIndex = new DbaseCharacter(Schema.RRIndex);
            RRStraatnaam = new DbaseCharacter(Schema.RRStraatnaam);
            GRARAdresID = new DbaseCharacter(Schema.GRARAdresID);
            GRARStraatnaamID = new DbaseCharacter(Schema.GRARStraatnaamID);
            GRARNiscode = new DbaseCharacter(Schema.GRARNiscode);
            GRARPostcode = new DbaseCharacter(Schema.GRARPostcode);
            GRARStraatnaam = new DbaseCharacter(Schema.GRARStraatnaam);
            GRARHuisnummer = new DbaseCharacter(Schema.GRARHuisnummer);
            Precisie = new DbaseCharacter(Schema.Precisie);
            Inwoners = new DbaseInt32(Schema.Inwoners);

            Values =
            [
                RRNiscode,
                RRPostcode,
                RRStraatcode,
                RRHuisnummer,
                RRIndex,
                RRStraatnaam,
                GRARAdresID,
                GRARStraatnaamID,
                GRARNiscode,
                GRARPostcode,
                GRARStraatnaam,
                GRARHuisnummer,
                Precisie,
                Inwoners
            ];
        }
    }

    public class UnmatchedAddressDbaseSchema : DbaseSchema
    {
        public DbaseField RRNiscode => Fields[0];
        public DbaseField RRPostcode => Fields[1];
        public DbaseField RRStraatcode => Fields[2];
        public DbaseField RRHuisnummer => Fields[3];
        public DbaseField RRIndex => Fields[4];
        public DbaseField RRStraatnaam => Fields[5];
        public DbaseField GRARAdresID => Fields[6];
        public DbaseField GRARStraatnaamID => Fields[7];
        public DbaseField GRARNiscode => Fields[8];
        public DbaseField GRARPostcode => Fields[9];
        public DbaseField GRARStraatnaam => Fields[10];
        public DbaseField GRARHuisnummer => Fields[11];
        public DbaseField Precisie => Fields[12];
        public DbaseField Inwoners => Fields[13];

        public UnmatchedAddressDbaseSchema() => Fields =
        [
            DbaseField.CreateCharacterField(new DbaseFieldName("RRNiscode"), new DbaseFieldLength(5)),
            DbaseField.CreateCharacterField(new DbaseFieldName("RRPostcode"), new DbaseFieldLength(4)),
            DbaseField.CreateCharacterField(new DbaseFieldName("RRStrcode"), new DbaseFieldLength(4)),
            DbaseField.CreateCharacterField(new DbaseFieldName("RRHuisNR"), new DbaseFieldLength(4)),
            DbaseField.CreateCharacterField(new DbaseFieldName("RRIndex"), new DbaseFieldLength(4)),
            DbaseField.CreateCharacterField(new DbaseFieldName("RRStraatNM"), new DbaseFieldLength(32)),
            DbaseField.CreateCharacterField(new DbaseFieldName("GRAdresID"), new DbaseFieldLength(50)),
            DbaseField.CreateCharacterField(new DbaseFieldName("GRStraatID"), new DbaseFieldLength(50)),
            DbaseField.CreateCharacterField(new DbaseFieldName("GRNiscode"), new DbaseFieldLength(5)),
            DbaseField.CreateCharacterField(new DbaseFieldName("GRPostcode"), new DbaseFieldLength(4)),
            DbaseField.CreateCharacterField(new DbaseFieldName("GRStraatNM"), new DbaseFieldLength(80)),
            DbaseField.CreateCharacterField(new DbaseFieldName("GRHuisNR"), new DbaseFieldLength(11)),
            DbaseField.CreateCharacterField(new DbaseFieldName("Precisie"), new DbaseFieldLength(10)),
            DbaseField.CreateNumberField(new DbaseFieldName("Inwoners"), new DbaseFieldLength(4), new DbaseDecimalCount(0))
        ];
    }
}
