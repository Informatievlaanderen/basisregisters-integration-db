using Microsoft.EntityFrameworkCore.Migrations;

using Basisregisters.IntegrationDb.Schema.Models.Views;
using Basisregisters.IntegrationDb.Schema.Models.Views.SuspiciousCases;
#nullable disable

namespace Basisregisters.IntegrationDb.Schema.Migrations
{
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    public partial class Add_Views : Migration
    {
       protected override void Up(MigrationBuilder migrationBuilder)
       {
           migrationBuilder.Sql(BuildingUnitAddressRelationConfiguration.Create);
           migrationBuilder.Sql(ParcelAddressRelationConfiguration.Create);

           // 1. Adressen ‘ingebruik’ zonder koppeling aan perceel of gebouweenheid
           migrationBuilder.Sql(CurrentAddressWithoutLinkedParcelOrBuildingUnitsConfiguration.Create);
           // 2. Adressen ‘voorgesteld’ zonder koppeling aan perceel of gebouweenheid
           migrationBuilder.Sql(ProposedAddressWithoutLinkedParcelOrBuildingUnitsConfiguration.Create);
           // 4. Adressen die buiten de grenzen van de gemeente vallen
           migrationBuilder.Sql(CurrentAddressesOutsideMunicipalityBoundsConfiguration.Create);
           // 5. Straatnamen "in gebruik" zonder koppeling met wegverbinding TODO
           migrationBuilder.Sql(CurrentStreetNameWithoutLinkedRoadSegmentsConfiguration.Create);

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
           migrationBuilder.Sql(AddressesLinkedToMultipleBuildingUnitsConfiguration.Create);

           // Suspicious cases List items
           migrationBuilder.Sql(SuspiciousCaseListItemConfiguration.Create);
       }

       protected override void Down(MigrationBuilder migrationBuilder)
       {
       }
    }
}
