namespace Basisregisters.IntegrationDb.SuspiciousCases.Functions
{
    using Infrastructure;

    public sealed class GetFullAddress
    {
        public const string Create =
            $@"CREATE OR REPLACE FUNCTION {Schema.FullAddress}(
                 StreetName VARCHAR,
                HouseNumber VARCHAR,
                BoxNumber VARCHAR,
                PostalCode VARCHAR,
                Municipality VARCHAR
            )
            RETURNS VARCHAR AS $$
            BEGIN
                IF StreetName IS NOT NULL AND BoxNumber IS NULL THEN
                    RETURN StreetName || ' ' || HouseNumber || ', ' || PostalCode || ' ' || Municipality;
                END IF;

                IF StreetName IS NOT NULL AND BoxNumber IS NOT NULL THEN
                    RETURN StreetName || ' ' || HouseNumber || ' bus ' || BoxNumber || ', ' || PostalCode || ' ' || Municipality;
                END IF;

                RETURN NULL;
            END;
            $$ LANGUAGE plpgsql;
            ";
    }
}
