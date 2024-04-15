using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basisregisters.IntegrationDb.Bosa.Model
{
    /*
     * postinfoid = { Value = message.Message.PostalCode },
       versieid = { Value = message.Message.Provenance.Timestamp.ToBelgianDateTimeOffset().FromDateTimeOffset() }
     */
    /*DECLARE @xml AS XML;
       WITH XMLNAMESPACES( 'uri1' AS tns, 'uri2' AS com)
       SELECT @xml= ( SELECT
       		            ( SELECT    'https://data.vlaanderen.be/id/postinfo/' AS 'com:namespace' ,
       		                        pk.postinfoid AS 'com:objectIdentifier' ,
       		                        pk.versieid AS 'com:versionIdentifier'
       		            FOR
       		                XML PATH('com:code') ,
       		                    TYPE
       		            ) ,
       		            ( SELECT
       		                       'nl' AS 'com:language',
       								pk.postnaam AS 'com:spelling'
       						
       		            FOR
       		                XML PATH('com:name') ,
       		                    TYPE
       		            ) 	
       		    FROM   dbo.vbr_postinfo pk
       		FOR
       		    XML PATH('tns:postalInfo') ,
       		        TYPE, ROOT ('tns:MyRoot')
       		);
       */
    public record PostalInfo(string Namespace, string PostalCode, DateTimeOffset VersionTimestamp, string DutchName);
}
