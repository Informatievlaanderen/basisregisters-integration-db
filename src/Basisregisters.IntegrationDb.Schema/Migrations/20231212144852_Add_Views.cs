using Microsoft.EntityFrameworkCore.Migrations;
using Basisregisters.IntegrationDb.Schema.Models.Views;
#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public partial class Add_Views : Migration
    {
       protected override void Up(MigrationBuilder migrationBuilder)
       {
           migrationBuilder.Sql(Views.BuildingUnitAddressRelations.Create);
           migrationBuilder.Sql(Views.ParcelAddressRelations.Create);

           // 1. Adressen ‘ingebruik’ zonder koppeling aan perceel of gebouweenheid
           migrationBuilder.Sql(Views.CurrentAddressWithoutLinkedParcelOrBuildingUnit.Create);
           // 2. Adressen ‘voorgesteld’ zonder koppeling aan perceel of gebouweenheid
           migrationBuilder.Sql(Views.ProposedAddressWithoutLinkedParcelOrBuildingUnit.Create);
           // 3. Huisnummers zonder postcode
           //migrationBuilder.Sql(Views.AddressesWithoutPostalCode.Create);
           // 4. Adressen die buiten de grenzen van de gemeente vallen
           migrationBuilder.Sql(Views.CurrentAddressesOutsideMunicipalityBounds.Create);
           // 5. Straatnamen "in gebruik" zonder koppeling met wegverbinding TODO
           migrationBuilder.Sql(Views.CurrentStreetNameWithoutLinkedRoadSegments.Create);

           // 6. Straatnamen langer dan 2 jaar “voorgesteld”
           // 7. Adressen bestaat langer dan 2 jaar en heeft nog de status “voorgesteld”
           // 8. Wegverbindingen bestaat langer dan 2 jaar en heeft nog de status "voorgesteld"
           // 9. Gebouw bestaat langer dan 2 jaar en heeft nog steeds de status “gepland”
           // 10. Gebouweenheden bestaat langer dan 2 jaar en heeft nog de status “gepland"
           // 11. Straatnaam waarbij enkel een wegverbindingen gekoppeld aan slechts 1 kant
           // 12. Adressen met herkomst "manueleAanduiding" buiten het gekoppelde gebouw
           // 13. Adressen met specificatie afgeleidvangebouweenheid “in gebruik” zonder koppeling met gebouweenheid
           // 14. Actueel gebouweenheden zonder adres
           // 15. Actueel Gebouweenheid gekoppeld aan meerdere actieve adressen

           // 16. Actieve adres gekoppeld aan meerdere actuele gebouweenheden
           migrationBuilder.Sql(Views.AddressesLinkedToMultipleBuildingUnits.Create);

           // TODO: review
           // migrationBuilder.Sql(Views.AddressesLinkedToMultipleParcels.Create);
           // migrationBuilder.Sql(Views.AddressesWithMultipleLinks.Create);
           // migrationBuilder.Sql(Views.ParcelsLinkedToMultipleAddresses.Create);
           // migrationBuilder.Sql(Views.CurrentAddressWithoutLinkedParcels.Create);
       }

       protected override void Down(MigrationBuilder migrationBuilder)
       {
       }
    }
}
