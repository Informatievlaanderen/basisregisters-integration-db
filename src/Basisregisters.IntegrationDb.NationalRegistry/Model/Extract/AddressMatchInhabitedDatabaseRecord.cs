namespace Basisregisters.IntegrationDb.NationalRegistry.Model.Extract
{
    using Be.Vlaanderen.Basisregisters.Shaperon;

    public class AddressMatchInhabitedDatabaseRecord : DbaseRecord
    {
        public static readonly AddressMatchInhabitedDbaseSchema Schema = new();

        public DbaseCharacter ID { get; }
        public DbaseCharacter StraatnaamID { get; }
        public DbaseCharacter StraatNM { get; }
        public DbaseCharacter HuisNR { get; }
        public DbaseCharacter BusNR { get; }
        public DbaseCharacter NisGemCode { get; }
        public DbaseCharacter GemNM { get; }
        public DbaseCharacter PKanCode { get; }
        public DbaseCharacter Herkomst { get; }
        public DbaseCharacter Methode { get; }
        public DbaseCharacter Bewoond { get; }
        public DbaseCharacter HuisnrStat { get; }

        public AddressMatchInhabitedDatabaseRecord()
        {
            ID = new DbaseCharacter(Schema.ID);
            StraatnaamID = new DbaseCharacter(Schema.StraatNMID);
            StraatNM = new DbaseCharacter(Schema.StraatNM);
            HuisNR = new DbaseCharacter(Schema.HuisNR);
            BusNR = new DbaseCharacter(Schema.BusNR);
            NisGemCode = new DbaseCharacter(Schema.NisGemCode);
            GemNM = new DbaseCharacter(Schema.GemNM);
            PKanCode = new DbaseCharacter(Schema.PKanCode);
            Herkomst = new DbaseCharacter(Schema.Herkomst);
            Methode = new DbaseCharacter(Schema.Methode);
            Bewoond = new DbaseCharacter(Schema.Bewoond);
            HuisnrStat = new DbaseCharacter(Schema.HuisnrStat);

            Values = new DbaseFieldValue[]
            {
                ID,
                StraatnaamID,
                StraatNM,
                HuisNR,
                BusNR,
                NisGemCode,
                GemNM,
                PKanCode,
                Herkomst,
                Methode,
                Bewoond,
                HuisnrStat
            };
        }
    }

    public class AddressMatchInhabitedDbaseSchema : DbaseSchema
    {
        public DbaseField ID => Fields[0];
        public DbaseField StraatNMID => Fields[1];
        public DbaseField StraatNM => Fields[2];
        public DbaseField HuisNR => Fields[3];
        public DbaseField BusNR => Fields[4];
        public DbaseField NisGemCode => Fields[5];
        public DbaseField GemNM => Fields[6];
        public DbaseField PKanCode => Fields[7];
        public DbaseField Herkomst => Fields[8];
        public DbaseField Methode => Fields[9];
        public DbaseField Bewoond => Fields[10];
        public DbaseField HuisnrStat => Fields[11];
        public AddressMatchInhabitedDbaseSchema() => Fields =
        [
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(ID)), new DbaseFieldLength(50)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(StraatNMID)), new DbaseFieldLength(50)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(StraatNM)), new DbaseFieldLength(80)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(HuisNR)), new DbaseFieldLength(11)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(BusNR)), new DbaseFieldLength(35)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(NisGemCode)), new DbaseFieldLength(5)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(GemNM)), new DbaseFieldLength(40)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(PKanCode)), new DbaseFieldLength(4)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(Herkomst)), new DbaseFieldLength(20)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(Methode)), new DbaseFieldLength(30)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(Bewoond)), new DbaseFieldLength(10)),
            DbaseField.CreateCharacterField(new DbaseFieldName(nameof(HuisnrStat)), new DbaseFieldLength(20))
        ];
    }
}
