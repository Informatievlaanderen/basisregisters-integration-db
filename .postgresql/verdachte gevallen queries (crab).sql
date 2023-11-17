SELECT  ap.adrespositieid AS id, s.straatnaam + ' ' + h.huisnummer AS label FROM    odb.tblhuisnummer h INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid AND ap.aardadres = '2' INNER JOIN odb.tblhuisnummerstatus hs ON h.huisnummerid = hs.huisnummerid WHERE   h.einddatum IS NULL AND hs.einddatum IS NULL AND hs.huisnummerstatus = '3' AND ap.einddatum IS NULL AND ap.herkomstadrespositie NOT IN ('8', '9') AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND NOT EXISTS ( SELECT NULL FROM odb.tblterreinobject_huisnummer th INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid WHERE th.huisnummerid = h.huisnummerid AND th.einddatum IS NULL AND t.aardterreinobjectcode IN ('1','4','5') )
SELECT  ap.adrespositieid AS id, s.straatnaam + ' ' + h.huisnummer AS label FROM    odb.tblhuisnummer h INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid AND ap.aardadres = '2' INNER JOIN odb.tblhuisnummer_postkanton hp ON h.huisnummerid = hp.huisnummerid WHERE   h.einddatum IS NULL AND ap.einddatum IS NULL AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND hp.einddatum IS NULL AND NOT EXISTS ( SELECT NULL FROM   odb.tblpostkanton pk INNER JOIN odb.tblsubkanton sk ON pk.postkantonid = sk.postkantonid INNER JOIN odb.tblsubkanton_gemeente sg ON sk.subkantonid = sg.subkantonid WHERE  pk.postkantonid = hp.postkantonid AND sg.gemeenteid = s.gemeenteid )
SELECT  ap.adrespositieid AS id, s.straatnaam + ' ' + h.huisnummer AS label FROM    odb.tblhuisnummer h INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid AND ap.aardadres = '2' INNER JOIN odb.tblhuisnummerstatus hs ON h.huisnummerid = hs.huisnummerid WHERE   h.einddatum IS NULL AND ap.einddatum IS NULL AND hs.einddatum IS NULL AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND NOT EXISTS ( SELECT NULL FROM   odb.tblhuisnummer_postkanton hp WHERE  hp.huisnummerid = h.huisnummerid AND hp.einddatum IS NULL )
SELECT  ap.adrespositieid AS id, s.straatnaam + ' ' + h.huisnummer AS label FROM    odb.tbladrespositie ap INNER JOIN odb.tblhuisnummer h ON h.huisnummerid = ap.adresid INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid INNER JOIN odb.tblgemeentegeometry gg ON s.gemeenteid = gg.gemeenteid WHERE   h.einddatum IS NULL AND ap.einddatum IS NULL AND ap.aardadres = '2' AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND gg.geom.STContains(ap.adrespositie) = 0 UNION ALL SELECT ap.adrespositieid AS id, s.straatnaam + ' ' + h.huisnummer + ' ' + sa.subadres AS label FROM    odb.tbladrespositie ap INNER JOIN odb.tblsubadres sa ON ap.adresid = sa.subadresid INNER JOIN odb.tblhuisnummer h ON h.huisnummerid = sa.huisnummerid INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid INNER JOIN odb.tblgemeentegeometry gg ON s.gemeenteid = gg.gemeenteid WHERE   sa.einddatum IS NULL AND ap.einddatum IS NULL AND ap.aardadres = '1' AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND gg.geom.STContains(ap.adrespositie) = 0
SELECT s.straatnaamid AS id, straatnaam AS label FROM odb.tblstraatnaam s INNER JOIN odb.tblstraatnaamstatus ss ON s.straatnaamid = ss.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid WHERE s.einddatum IS NULL AND ss.straatnaamstatus = '3' AND ss.einddatum IS NULL AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND NOT EXISTS ( SELECT NULL FROM odb.tblstraatkant sk INNER JOIN odb.tblwegobject w ON sk.wegobjectid = w.wegobjectid WHERE sk.straatnaamid = s.straatnaamid AND sk.einddatum IS NULL AND w.aardwegobjectcode = '5' )
SELECT  ap.adrespositieid AS id, s.straatnaam + ' ' + h.huisnummer AS label FROM    odb.tblhuisnummer h INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid AND ap.aardadres = '2' WHERE   h.einddatum IS NULL AND ap.einddatum IS NULL AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND EXISTS ( SELECT NULL FROM   odb.tblterreinobject_huisnummer th INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid WHERE  th.huisnummerid = h.huisnummerid AND th.einddatum IS NULL AND t.aardterreinobjectcode = '1' and th.beginOrganisatie <> '1' GROUP BY th.huisnummerid HAVING COUNT(*) > 1 UNION ALL SELECT NULL FROM   odb.tblterreinobject_huisnummer th INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid WHERE  th.huisnummerid = h.huisnummerid AND th.einddatum IS NULL AND t.aardterreinobjectcode = '5' and th.beginOrganisatie <> '1' GROUP BY th.huisnummerid HAVING COUNT(*) > 1 )
SELECT  s.straatnaamid AS id, s.straatnaam + ' (' + s2.straatnaam + ')' AS label FROM    odb.tblstraatnaam s INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid INNER JOIN odb.tblstraatnaam s2 ON s.gemeenteid = s2.gemeenteid WHERE   s.einddatum IS NULL AND s2.einddatum IS NULL AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND s.straatnaamid <> s2.straatnaamid and s.beginOrganisatie <> '1' and s2.beginOrganisatie <> '1' AND dbo.fncgetsimilarity(s2.straatnaam, s.straatnaam) > 85
WITH    sortedHuisnr ( straatnaamid, huisnummerid, huisnummer, rownumber, pariteitcode ) AS ( SELECT   h.straatnaamid, h.huisnummerid, dbo.sortablehuisnr(h.huisnummer), ROW_NUMBER() OVER ( ORDER BY h.straatnaamid, dbo.sortablehuisnr(h.huisnummer) ), '1' FROM     odb.tblhuisnummer h INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid WHERE    h.einddatum IS NULL AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND dbo.fncnumeriekbegin(h.huisnummer) % 2 = 0 AND EXISTS ( SELECT NULL FROM   odb.tblstraatkant sk WHERE  sk.einddatum IS NULL AND sk.straatnaamid = s.straatnaamid AND sk.pariteitcode = '1' ) UNION ALL SELECT   h.straatnaamid, h.huisnummerid, dbo.sortablehuisnr(h.huisnummer), ROW_NUMBER() OVER ( ORDER BY h.straatnaamid, dbo.sortablehuisnr(h.huisnummer) ), '2' FROM     odb.tblhuisnummer h INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid WHERE    h.einddatum IS NULL AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND dbo.fncnumeriekbegin(h.huisnummer) % 2 = 1 AND EXISTS ( SELECT NULL FROM   odb.tblstraatkant sk WHERE  sk.einddatum IS NULL AND sk.straatnaamid = s.straatnaamid AND sk.pariteitcode = '2' ) UNION ALL SELECT   h.straatnaamid, h.huisnummerid, dbo.sortablehuisnr(h.huisnummer), ROW_NUMBER() OVER ( ORDER BY h.straatnaamid, dbo.sortablehuisnr(h.huisnummer) ), '3' FROM     odb.tblhuisnummer h INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid WHERE    h.einddatum IS NULL AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' AND EXISTS ( SELECT NULL FROM   odb.tblstraatkant sk WHERE  sk.einddatum IS NULL AND sk.straatnaamid = s.straatnaamid AND sk.pariteitcode = '3' ) ) SELECT  ap.adrespositieid AS id, s.straatnaam + ' ' + h.huisnummer AS label FROM    sortedHuisnr sh INNER JOIN odb.tblhuisnummer h ON sh.huisnummerid = h.huisnummerid INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid AND ap.aardadres = '2' INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid LEFT OUTER JOIN ( sortedHuisnr sh2 INNER JOIN odb.tbladrespositie ap2 ON sh2.huisnummerid = ap2.adresid AND ap2.aardadres = '2' AND ap2.einddatum IS NULL ) ON sh.straatnaamid = sh2.straatnaamid AND sh.pariteitcode = sh2.pariteitcode AND sh.rownumber = sh2.rownumber + 1 LEFT OUTER JOIN ( sortedHuisnr sh3 INNER JOIN odb.tbladrespositie ap3 ON sh3.huisnummerid = ap3.adresid AND ap3.aardadres = '2' AND ap3.einddatum IS NULL ) ON sh.straatnaamid = sh3.straatnaamid AND sh.pariteitcode = sh3.pariteitcode AND sh.rownumber = sh3.rownumber - 1 WHERE   ap.einddatum IS NULL and ap.beginorganisatie <> '1' AND ap2.adrespositie IS NOT NULL AND ap3.adrespositie IS NOT NULL AND NOT EXISTS ( SELECT NULL FROM   sortedHuisnr sh4 WHERE  sh.huisnummerid = sh4.huisnummerid AND sh.pariteitcode <> sh4.pariteitcode ) AND NOT ( ( ap.adrespositie.STX BETWEEN ap2.adrespositie.STX AND     ap3.adrespositie.STX OR ap.adrespositie.STX BETWEEN ap3.adrespositie.STX AND     ap2.adrespositie.STX ) OR ( ap.adrespositie.STY BETWEEN ap2.adrespositie.STY AND     ap3.adrespositie.STY OR ap.adrespositie.STY BETWEEN ap3.adrespositie.STY AND     ap2.adrespositie.STY ) )
SELECT  t.terreinobjectid AS id, t.identificatorterreinobject AS label FROM    odb.tblterreinobject_huisnummer th INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid INNER JOIN odb.tblhuisnummer h ON th.huisnummerid = h.huisnummerid INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid WHERE   th.einddatum IS NULL AND t.aardterreinobjectcode = '1' AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl' and th.beginOrganisatie <> '1' AND EXISTS ( SELECT NULL FROM   odb.tblterreinobject_huisnummer th2 WHERE  th2.einddatum IS NULL AND th2.terreinobjectid = th.terreinobjectid AND th2.huisnummerid <> th.huisnummerid and th2.beginOrganisatie <> '1' )
SELECT  sssn.substraat_straatnaam_id AS id, ssn.substraatnaam + ' ' + ss.straatcode AS label FROM    odb.tblsubstraat_straatnaam sssn INNER JOIN odb.tblsubstraat ss ON sssn.substraatid = ss.substraatid INNER JOIN odb.tblsubstraatnaam ssn ON ss.substraatid = ssn.substraatid INNER JOIN odb.tblsubkanton_gemeente skg ON skg.subkantonid = ss.subkantonid INNER JOIN odb.tblgemeentenaam gm ON skg.gemeenteid = gm.gemeenteid WHERE   sssn.einddatum IS NULL AND ss.einddatum IS NULL AND ss.straatcode NOT LIKE 'D%' AND gm.taalcodegemeentenaam = 'nl' AND gm.gemeentenaam = @gemeente AND NOT EXISTS ( SELECT NULL FROM   web2.vw_rrstraat WHERE  id = sssn.substraat_straatnaam_id )
SELECT  a.id, a.straatnaam + ' ' + a.huisnummer + ' bus ' + a.busnummer AS label FROM    web2.vw_adres a WHERE   busnummer IS NOT NULL AND gemeente = @gemeente AND EXISTS ( SELECT NULL FROM   web2.vw_adres a2 WHERE  a.id <> a2.id AND a.gemeente = a2.gemeente AND a.straatnaam = a2.straatnaam AND a.huisnummer = a2.huisnummer AND a2.busnummer IS NOT NULL AND a.busnummer <> a2.busnummer AND dbo.fncisinteger(a.busnummer) = dbo.fncisinteger(a2.busnummer) ) UNION SELECT  a.id, a.straatnaam + ' ' + a.huisnummer + ' app ' + a.appnummer AS label FROM    web2.vw_adres a WHERE   appnummer IS NOT NULL AND gemeente = @gemeente AND EXISTS ( SELECT NULL FROM   web2.vw_adres a2 WHERE  a.id <> a2.id AND a.gemeente = a2.gemeente AND a.straatnaam = a2.straatnaam AND a.huisnummer = a2.huisnummer AND a2.appnummer IS NOT NULL AND a.appnummer <> a2.appnummer AND dbo.fncisinteger(a.appnummer) = dbo.fncisinteger(a2.appnummer) ) 
SELECT  id, straatnaam AS label FROM    web2.vw_straat s WHERE   [status] <> 'inGebruik' AND gemeente = @gemeente AND NOT EXISTS ( SELECT NULL FROM   odb.tblstraatnaamstatus WHERE  straatnaamid = s.id AND straatnaamstatus = '3' AND begintijd > DATEADD(YEAR, -2, GETDATE()) ) AND NOT EXISTS ( SELECT NULL FROM   cdb.tblstraatnaamstatus_hist WHERE  straatnaamid = s.id AND straatnaamstatus = '3' AND eindtijd > DATEADD(YEAR, -2, GETDATE()) ) AND EXISTS ( SELECT NULL FROM   odb.tblstraatnaamstatus WHERE  straatnaamid = s.id AND straatnaamstatus <> '3' AND begintijd < DATEADD(YEAR, -2, GETDATE()) )
SELECT  id, straatnaam + ' ' + huisnummer AS label FROM    web2.vw_adres a INNER JOIN odb.tbladrespositie ap ON a.id = ap.adrespositieid WHERE   [status] <> 'inGebruik' AND gemeente = @gemeente AND busnummer IS NULL AND appnummer IS NULL AND NOT EXISTS ( SELECT NULL FROM   odb.tblhuisnummerstatus WHERE  huisnummerid = ap.adresid AND huisnummerstatus = '3' AND begintijd > DATEADD(YEAR, -2, GETDATE()) ) AND NOT EXISTS ( SELECT NULL FROM   cdb.tblhuisnummerstatus_hist WHERE  huisnummerid = ap.adresid AND huisnummerstatus = '3' AND eindtijd > DATEADD(YEAR, -2, GETDATE()) ) AND EXISTS ( SELECT NULL FROM   odb.tblhuisnummerstatus WHERE  huisnummerid = ap.adresid AND huisnummerstatus <> '3' AND begintijd < DATEADD(YEAR, -2, GETDATE()) )
SELECT  w.id, identificator AS label FROM    web2.vw_wegverbinding w INNER JOIN odb.tblwegverbindinggeometrie wvb ON w.id = wvb.wegverbindinggeometrieid CROSS APPLY odb.tblgemeentegeometry gemgeom WHERE   [status] <> 'inGebruik' AND gemgeom.NAME = @gemeente AND wvb.wegverbindinggeometrie.STIntersects(gemgeom.geom) = 1 AND NOT EXISTS ( SELECT NULL FROM   odb.tblwegverbindingstatus WHERE  wegobjectid = wvb.wegobjectid AND wegverbindingstatus = '3' AND begintijd > DATEADD(YEAR, -2, GETDATE()) ) AND NOT EXISTS ( SELECT NULL FROM   cdb.tblwegverbindingstatus_hist WHERE  wegobjectid = wvb.wegobjectid AND wegverbindingstatus = '3' AND eindtijd > DATEADD(YEAR, -2, GETDATE()) ) AND EXISTS ( SELECT NULL FROM   odb.tblwegverbindingstatus WHERE  wegobjectid = wvb.wegobjectid AND wegverbindingstatus <> '3' AND begintijd < DATEADD(YEAR, -2, GETDATE()) )
SELECT DISTINCT g.id, identificator AS label FROM    web2.vw_gebouw g CROSS APPLY odb.tblgemeentegeometry gemgeom WHERE   [status] <> 'inGebruik' AND gemgeom.NAME = @gemeente AND g.geom.STIntersects(gemgeom.geom) = 1 AND NOT EXISTS ( SELECT NULL FROM   odb.tblgebouwstatus WHERE  terreinobjectid = g.id AND gebouwstatus = '3' AND begintijd > DATEADD(YEAR, -2, GETDATE()) ) AND NOT EXISTS ( SELECT NULL FROM   cdb.tblgebouwstatus_hist WHERE  terreinobjectid = g.id AND gebouwstatus = '3' AND eindtijd > DATEADD(YEAR, -2, GETDATE()) ) AND EXISTS ( SELECT NULL FROM   odb.tblgebouwstatus WHERE  terreinobjectid = g.id AND gebouwstatus <> '3' AND begintijd < DATEADD(YEAR, -2, GETDATE()) )
SELECT  w.id, identificator AS label FROM    web2.vw_wegverbinding w INNER JOIN odb.tblwegverbindinggeometrie wvb ON w.id = wvb.wegverbindinggeometrieid CROSS APPLY odb.tblgemeentegeometry gemgeom WHERE   ( ( straatidlinks IS NULL AND straatidrechts IS NOT NULL ) OR ( straatidlinks IS NOT NULL AND straatidrechts IS NULL ) ) AND gemgeom.NAME = @gemeente AND wvb.wegverbindinggeometrie.STIntersects(gemgeom.geom) = 1
select g.id, identificator from web2.vw_gebouw g where g.aardgebouw = 'bijgebouw' and exists(select null from web2.vw_adres_gebouw ag inner join web2.vw_adres a on ag.adresid = a.id where ag.gebouwid = g.id and a.gemeente = @gemeente)
select a.id, a.straatnaam + ' ' + a.huisnummer AS label from web2.vw_adres a where a.gemeente = @gemeente and a.herkomst = 'manueleAanduidingVanGebouw' and exists(select null from web2.vw_adres_gebouw ag inner join web2.vw_gebouw g on ag.gebouwid = g.id where a.id = ag.adresid and a.geom.STIntersects(g.geom) = 0) and not exists(select null from web2.vw_adres_gebouw ag inner join web2.vw_gebouw g on ag.gebouwid = g.id where a.id = ag.adresid and a.geom.STIntersects(g.geom) = 1)
SELECT  ap.adrespositieid AS id, s.straatnaam + ' ' + h.huisnummer AS label  FROM    odb.tblhuisnummer h   INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid     INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid AND ap.aardadres = '2'    INNER JOIN odb.tblhuisnummerstatus hs ON h.huisnummerid = hs.huisnummerid    WHERE   h.einddatum IS NULL AND hs.einddatum IS NULL AND hs.huisnummerstatus = '3'    AND ap.einddatum IS NULL AND ap.herkomstadrespositie NOT IN ('1','8', '9')    AND gm.gemeentenaam = @gemeente AND gm.taalcodegemeentenaam = 'nl'    AND NOT EXISTS     ( SELECT NULL FROM odb.tblterreinobject_huisnummer th   INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid  WHERE th.huisnummerid = h.huisnummerid  AND th.einddatum IS NULL  AND t.aardterreinobjectcode IN ('1','4','5') )


SELECT
    ap.adrespositieid AS id,
    s.straatnaam + ' ' + h.huisnummer AS label
FROM
    odb.tblhuisnummer h
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
    INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid
    AND ap.aardadres = '2'
    INNER JOIN odb.tblhuisnummerstatus hs ON h.huisnummerid = hs.huisnummerid
WHERE
    h.einddatum IS NULL
    AND hs.einddatum IS NULL
    AND hs.huisnummerstatus = '3'
    AND ap.einddatum IS NULL
    AND ap.herkomstadrespositie NOT IN ('8', '9')
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblterreinobject_huisnummer th
            INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid
        WHERE
            th.huisnummerid = h.huisnummerid
            AND th.einddatum IS NULL
            AND t.aardterreinobjectcode IN ('1', '4', '5')
  )



SELECT
    ap.adrespositieid AS id,
    s.straatnaam + ' ' + h.huisnummer AS label
FROM
    odb.tblhuisnummer h
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
    INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid
    AND ap.aardadres = '2'
    INNER JOIN odb.tblhuisnummer_postkanton hp ON h.huisnummerid = hp.huisnummerid
WHERE
    h.einddatum IS NULL
    AND ap.einddatum IS NULL
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND hp.einddatum IS NULL
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblpostkanton pk
            INNER JOIN odb.tblsubkanton sk ON pk.postkantonid = sk.postkantonid
            INNER JOIN odb.tblsubkanton_gemeente sg ON sk.subkantonid = sg.subkantonid
        WHERE
            pk.postkantonid = hp.postkantonid
            AND sg.gemeenteid = s.gemeenteid
  )



SELECT
    ap.adrespositieid AS id,
    s.straatnaam + ' ' + h.huisnummer AS label
FROM
    odb.tblhuisnummer h
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
    INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid
    AND ap.aardadres = '2'
    INNER JOIN odb.tblhuisnummerstatus hs ON h.huisnummerid = hs.huisnummerid
WHERE
    h.einddatum IS NULL
    AND ap.einddatum IS NULL
    AND hs.einddatum IS NULL
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblhuisnummer_postkanton hp
        WHERE
            hp.huisnummerid = h.huisnummerid
            AND hp.einddatum IS NULL
  )



SELECT
    ap.adrespositieid AS id,
    s.straatnaam + ' ' + h.huisnummer AS label
FROM
    odb.tbladrespositie ap
    INNER JOIN odb.tblhuisnummer h ON h.huisnummerid = ap.adresid
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
    INNER JOIN odb.tblgemeentegeometry gg ON s.gemeenteid = gg.gemeenteid
WHERE
    h.einddatum IS NULL
    AND ap.einddatum IS NULL
    AND ap.aardadres = '2'
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND gg.geom.STContains(ap.adrespositie) = 0
UNION
ALL
SELECT
    ap.adrespositieid AS id,
    s.straatnaam + ' ' + h.huisnummer + ' ' + sa.subadres AS label
FROM
    odb.tbladrespositie ap
    INNER JOIN odb.tblsubadres sa ON ap.adresid = sa.subadresid
    INNER JOIN odb.tblhuisnummer h ON h.huisnummerid = sa.huisnummerid
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
    INNER JOIN odb.tblgemeentegeometry gg ON s.gemeenteid = gg.gemeenteid
WHERE
    sa.einddatum IS NULL
    AND ap.einddatum IS NULL
    AND ap.aardadres = '1'
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND gg.geom.STContains(ap.adrespositie) = 0
SELECT
    s.straatnaamid AS id,
    straatnaam AS label
FROM
    odb.tblstraatnaam s
    INNER JOIN odb.tblstraatnaamstatus ss ON s.straatnaamid = ss.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
WHERE
    s.einddatum IS NULL
    AND ss.straatnaamstatus = '3'
    AND ss.einddatum IS NULL
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblstraatkant sk
            INNER JOIN odb.tblwegobject w ON sk.wegobjectid = w.wegobjectid
        WHERE
            sk.straatnaamid = s.straatnaamid
            AND sk.einddatum IS NULL
            AND w.aardwegobjectcode = '5'
  )



SELECT
    ap.adrespositieid AS id,
    s.straatnaam + ' ' + h.huisnummer AS label
FROM
    odb.tblhuisnummer h
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
    INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid
    AND ap.aardadres = '2'
WHERE
    h.einddatum IS NULL
    AND ap.einddatum IS NULL
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND EXISTS (
        SELECT
            NULL
        FROM
            odb.tblterreinobject_huisnummer th
            INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid
        WHERE
            th.huisnummerid = h.huisnummerid
            AND th.einddatum IS NULL
            AND t.aardterreinobjectcode = '1'
            and th.beginOrganisatie <> '1'
        GROUP BY
            th.huisnummerid
        HAVING
            COUNT(*) > 1
        UNION
        ALL
        SELECT
            NULL
        FROM
            odb.tblterreinobject_huisnummer th
            INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid
        WHERE
            th.huisnummerid = h.huisnummerid
            AND th.einddatum IS NULL
            AND t.aardterreinobjectcode = '5'
            and th.beginOrganisatie <> '1'
        GROUP BY
            th.huisnummerid
        HAVING
            COUNT(*) > 1
  )



SELECT
    s.straatnaamid AS id,
    s.straatnaam + ' (' + s2.straatnaam + ')' AS label
FROM
    odb.tblstraatnaam s
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
    INNER JOIN odb.tblstraatnaam s2 ON s.gemeenteid = s2.gemeenteid
WHERE
    s.einddatum IS NULL
    AND s2.einddatum IS NULL
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND s.straatnaamid <> s2.straatnaamid
    and s.beginOrganisatie <> '1'
    and s2.beginOrganisatie <> '1'
    AND dbo.fncgetsimilarity(s2.straatnaam, s.straatnaam) > 85 WITH sortedHuisnr (
        straatnaamid,
        huisnummerid,
        huisnummer,
        rownumber,
        pariteitcode
    ) AS (
        SELECT
            h.straatnaamid,
            h.huisnummerid,
            dbo.sortablehuisnr(h.huisnummer),
            ROW_NUMBER() OVER (
                ORDER BY
                    h.straatnaamid,
                    dbo.sortablehuisnr(h.huisnummer)
            ),
            '1'
        FROM
            odb.tblhuisnummer h
            INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
            INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
        WHERE
            h.einddatum IS NULL
            AND gm.gemeentenaam = @gemeente
            AND gm.taalcodegemeentenaam = 'nl'
            AND dbo.fncnumeriekbegin(h.huisnummer) % 2 = 0
            AND EXISTS (
                SELECT
                    NULL
                FROM
                    odb.tblstraatkant sk
                WHERE
                    sk.einddatum IS NULL
                    AND sk.straatnaamid = s.straatnaamid
                    AND sk.pariteitcode = '1'
            )
        UNION
        ALL
        SELECT
            h.straatnaamid,
            h.huisnummerid,
            dbo.sortablehuisnr(h.huisnummer),
            ROW_NUMBER() OVER (
                ORDER BY
                    h.straatnaamid,
                    dbo.sortablehuisnr(h.huisnummer)
            ),
            '2'
        FROM
            odb.tblhuisnummer h
            INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
            INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
        WHERE
            h.einddatum IS NULL
            AND gm.gemeentenaam = @gemeente
            AND gm.taalcodegemeentenaam = 'nl'
            AND dbo.fncnumeriekbegin(h.huisnummer) % 2 = 1
            AND EXISTS (
                SELECT
                    NULL
                FROM
                    odb.tblstraatkant sk
                WHERE
                    sk.einddatum IS NULL
                    AND sk.straatnaamid = s.straatnaamid
                    AND sk.pariteitcode = '2'
            )
        UNION
        ALL
        SELECT
            h.straatnaamid,
            h.huisnummerid,
            dbo.sortablehuisnr(h.huisnummer),
            ROW_NUMBER() OVER (
                ORDER BY
                    h.straatnaamid,
                    dbo.sortablehuisnr(h.huisnummer)
            ),
            '3'
        FROM
            odb.tblhuisnummer h
            INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
            INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
        WHERE
            h.einddatum IS NULL
            AND gm.gemeentenaam = @gemeente
            AND gm.taalcodegemeentenaam = 'nl'
            AND EXISTS (
                SELECT
                    NULL
                FROM
                    odb.tblstraatkant sk
                WHERE
                    sk.einddatum IS NULL
                    AND sk.straatnaamid = s.straatnaamid
                    AND sk.pariteitcode = '3'
            )
  )



SELECT
    ap.adrespositieid AS id,
    s.straatnaam + ' ' + h.huisnummer AS label
FROM
    sortedHuisnr sh
    INNER JOIN odb.tblhuisnummer h ON sh.huisnummerid = h.huisnummerid
    INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid
    AND ap.aardadres = '2'
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    LEFT OUTER JOIN (
        sortedHuisnr sh2
        INNER JOIN odb.tbladrespositie ap2 ON sh2.huisnummerid = ap2.adresid
        AND ap2.aardadres = '2'
        AND ap2.einddatum IS NULL
    ) ON sh.straatnaamid = sh2.straatnaamid
    AND sh.pariteitcode = sh2.pariteitcode
    AND sh.rownumber = sh2.rownumber + 1
    LEFT OUTER JOIN (
        sortedHuisnr sh3
        INNER JOIN odb.tbladrespositie ap3 ON sh3.huisnummerid = ap3.adresid
        AND ap3.aardadres = '2'
        AND ap3.einddatum IS NULL
    ) ON sh.straatnaamid = sh3.straatnaamid
    AND sh.pariteitcode = sh3.pariteitcode
    AND sh.rownumber = sh3.rownumber - 1
WHERE
    ap.einddatum IS NULL
    and ap.beginorganisatie <> '1'
    AND ap2.adrespositie IS NOT NULL
    AND ap3.adrespositie IS NOT NULL
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            sortedHuisnr sh4
        WHERE
            sh.huisnummerid = sh4.huisnummerid
            AND sh.pariteitcode <> sh4.pariteitcode
    )
    AND NOT (
        (
            ap.adrespositie.STX BETWEEN ap2.adrespositie.STX
            AND ap3.adrespositie.STX
            OR ap.adrespositie.STX BETWEEN ap3.adrespositie.STX
            AND ap2.adrespositie.STX
        )
        OR (
            ap.adrespositie.STY BETWEEN ap2.adrespositie.STY
            AND ap3.adrespositie.STY
            OR ap.adrespositie.STY BETWEEN ap3.adrespositie.STY
            AND ap2.adrespositie.STY
        )
  )



SELECT
    t.terreinobjectid AS id,
    t.identificatorterreinobject AS label
FROM
    odb.tblterreinobject_huisnummer th
    INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid
    INNER JOIN odb.tblhuisnummer h ON th.huisnummerid = h.huisnummerid
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
WHERE
    th.einddatum IS NULL
    AND t.aardterreinobjectcode = '1'
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    and th.beginOrganisatie <> '1'
    AND EXISTS (
        SELECT
            NULL
        FROM
            odb.tblterreinobject_huisnummer th2
        WHERE
            th2.einddatum IS NULL
            AND th2.terreinobjectid = th.terreinobjectid
            AND th2.huisnummerid <> th.huisnummerid
            and th2.beginOrganisatie <> '1'
  )



SELECT
    sssn.substraat_straatnaam_id AS id,
    ssn.substraatnaam + ' ' + ss.straatcode AS label
FROM
    odb.tblsubstraat_straatnaam sssn
    INNER JOIN odb.tblsubstraat ss ON sssn.substraatid = ss.substraatid
    INNER JOIN odb.tblsubstraatnaam ssn ON ss.substraatid = ssn.substraatid
    INNER JOIN odb.tblsubkanton_gemeente skg ON skg.subkantonid = ss.subkantonid
    INNER JOIN odb.tblgemeentenaam gm ON skg.gemeenteid = gm.gemeenteid
WHERE
    sssn.einddatum IS NULL
    AND ss.einddatum IS NULL
    AND ss.straatcode NOT LIKE 'D%'
    AND gm.taalcodegemeentenaam = 'nl'
    AND gm.gemeentenaam = @gemeente
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            web2.vw_rrstraat
        WHERE
            id = sssn.substraat_straatnaam_id
  )



SELECT
    a.id,
    a.straatnaam + ' ' + a.huisnummer + ' bus ' + a.busnummer AS label
FROM
    web2.vw_adres a
WHERE
    busnummer IS NOT NULL
    AND gemeente = @gemeente
    AND EXISTS (
        SELECT
            NULL
        FROM
            web2.vw_adres a2
        WHERE
            a.id <> a2.id
            AND a.gemeente = a2.gemeente
            AND a.straatnaam = a2.straatnaam
            AND a.huisnummer = a2.huisnummer
            AND a2.busnummer IS NOT NULL
            AND a.busnummer <> a2.busnummer
            AND dbo.fncisinteger(a.busnummer) = dbo.fncisinteger(a2.busnummer)
    )
UNION
SELECT
    a.id,
    a.straatnaam + ' ' + a.huisnummer + ' app ' + a.appnummer AS label
FROM
    web2.vw_adres a
WHERE
    appnummer IS NOT NULL
    AND gemeente = @gemeente
    AND EXISTS (
        SELECT
            NULL
        FROM
            web2.vw_adres a2
        WHERE
            a.id <> a2.id
            AND a.gemeente = a2.gemeente
            AND a.straatnaam = a2.straatnaam
            AND a.huisnummer = a2.huisnummer
            AND a2.appnummer IS NOT NULL
            AND a.appnummer <> a2.appnummer
            AND dbo.fncisinteger(a.appnummer) = dbo.fncisinteger(a2.appnummer)
  )



SELECT
    id,
    straatnaam AS label
FROM
    web2.vw_straat s
WHERE
    [status] <> 'inGebruik'
    AND gemeente = @gemeente
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblstraatnaamstatus
        WHERE
            straatnaamid = s.id
            AND straatnaamstatus = '3'
            AND begintijd > DATEADD(YEAR, -2, GETDATE())
    )
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            cdb.tblstraatnaamstatus_hist
        WHERE
            straatnaamid = s.id
            AND straatnaamstatus = '3'
            AND eindtijd > DATEADD(YEAR, -2, GETDATE())
    )
    AND EXISTS (
        SELECT
            NULL
        FROM
            odb.tblstraatnaamstatus
        WHERE
            straatnaamid = s.id
            AND straatnaamstatus <> '3'
            AND begintijd < DATEADD(YEAR, -2, GETDATE())
  )



SELECT
    id,
    straatnaam + ' ' + huisnummer AS label
FROM
    web2.vw_adres a
    INNER JOIN odb.tbladrespositie ap ON a.id = ap.adrespositieid
WHERE
    [status] <> 'inGebruik'
    AND gemeente = @gemeente
    AND busnummer IS NULL
    AND appnummer IS NULL
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblhuisnummerstatus
        WHERE
            huisnummerid = ap.adresid
            AND huisnummerstatus = '3'
            AND begintijd > DATEADD(YEAR, -2, GETDATE())
    )
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            cdb.tblhuisnummerstatus_hist
        WHERE
            huisnummerid = ap.adresid
            AND huisnummerstatus = '3'
            AND eindtijd > DATEADD(YEAR, -2, GETDATE())
    )
    AND EXISTS (
        SELECT
            NULL
        FROM
            odb.tblhuisnummerstatus
        WHERE
            huisnummerid = ap.adresid
            AND huisnummerstatus <> '3'
            AND begintijd < DATEADD(YEAR, -2, GETDATE())
  )



SELECT
    w.id,
    identificator AS label
FROM
    web2.vw_wegverbinding w
    INNER JOIN odb.tblwegverbindinggeometrie wvb ON w.id = wvb.wegverbindinggeometrieid
    CROSS APPLY odb.tblgemeentegeometry gemgeom
WHERE
    [status] <> 'inGebruik'
    AND gemgeom.NAME = @gemeente
    AND wvb.wegverbindinggeometrie.STIntersects(gemgeom.geom) = 1
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblwegverbindingstatus
        WHERE
            wegobjectid = wvb.wegobjectid
            AND wegverbindingstatus = '3'
            AND begintijd > DATEADD(YEAR, -2, GETDATE())
    )
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            cdb.tblwegverbindingstatus_hist
        WHERE
            wegobjectid = wvb.wegobjectid
            AND wegverbindingstatus = '3'
            AND eindtijd > DATEADD(YEAR, -2, GETDATE())
    )
    AND EXISTS (
        SELECT
            NULL
        FROM
            odb.tblwegverbindingstatus
        WHERE
            wegobjectid = wvb.wegobjectid
            AND wegverbindingstatus <> '3'
            AND begintijd < DATEADD(YEAR, -2, GETDATE())
  )



SELECT
    DISTINCT g.id,
    identificator AS label
FROM
    web2.vw_gebouw g
    CROSS APPLY odb.tblgemeentegeometry gemgeom
WHERE
    [status] <> 'inGebruik'
    AND gemgeom.NAME = @gemeente
    AND g.geom.STIntersects(gemgeom.geom) = 1
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblgebouwstatus
        WHERE
            terreinobjectid = g.id
            AND gebouwstatus = '3'
            AND begintijd > DATEADD(YEAR, -2, GETDATE())
    )
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            cdb.tblgebouwstatus_hist
        WHERE
            terreinobjectid = g.id
            AND gebouwstatus = '3'
            AND eindtijd > DATEADD(YEAR, -2, GETDATE())
    )
    AND EXISTS (
        SELECT
            NULL
        FROM
            odb.tblgebouwstatus
        WHERE
            terreinobjectid = g.id
            AND gebouwstatus <> '3'
            AND begintijd < DATEADD(YEAR, -2, GETDATE())
  )



SELECT
    w.id,
    identificator AS label
FROM
    web2.vw_wegverbinding w
    INNER JOIN odb.tblwegverbindinggeometrie wvb ON w.id = wvb.wegverbindinggeometrieid
    CROSS APPLY odb.tblgemeentegeometry gemgeom
WHERE
    (
        (
            straatidlinks IS NULL
            AND straatidrechts IS NOT NULL
        )
        OR (
            straatidlinks IS NOT NULL
            AND straatidrechts IS NULL
        )
    )
    AND gemgeom.NAME = @gemeente
    AND wvb.wegverbindinggeometrie.STIntersects(gemgeom.geom) = 1
select
    g.id,
    identificator
from
    web2.vw_gebouw g
where
    g.aardgebouw = 'bijgebouw'
    and exists(
        select
            null
        from
            web2.vw_adres_gebouw ag
            inner join web2.vw_adres a on ag.adresid = a.id
        where
            ag.gebouwid = g.id
            and a.gemeente = @gemeente
  )



SELECT
    a.id,
    a.straatnaam + ' ' + a.huisnummer AS label
from
    web2.vw_adres a
where
    a.gemeente = @gemeente
    and a.herkomst = 'manueleAanduidingVanGebouw'
    and exists(
        select
            null
        from
            web2.vw_adres_gebouw ag
            inner join web2.vw_gebouw g on ag.gebouwid = g.id
        where
            a.id = ag.adresid
            and a.geom.STIntersects(g.geom) = 0
    )
    and not exists(
        select
            null
        from
            web2.vw_adres_gebouw ag
            inner join web2.vw_gebouw g on ag.gebouwid = g.id
        where
            a.id = ag.adresid
            and a.geom.STIntersects(g.geom) = 1
  )



SELECT
    ap.adrespositieid AS id,
    s.straatnaam + ' ' + h.huisnummer AS label
FROM
    odb.tblhuisnummer h
    INNER JOIN odb.tblstraatnaam s ON h.straatnaamid = s.straatnaamid
    INNER JOIN odb.tblgemeentenaam gm ON s.gemeenteid = gm.gemeenteid
    INNER JOIN odb.tbladrespositie ap ON h.huisnummerid = ap.adresid
    AND ap.aardadres = '2'
    INNER JOIN odb.tblhuisnummerstatus hs ON h.huisnummerid = hs.huisnummerid
WHERE
    h.einddatum IS NULL
    AND hs.einddatum IS NULL
    AND hs.huisnummerstatus = '3'
    AND ap.einddatum IS NULL
    AND ap.herkomstadrespositie NOT IN ('1', '8', '9')
    AND gm.gemeentenaam = @gemeente
    AND gm.taalcodegemeentenaam = 'nl'
    AND NOT EXISTS (
        SELECT
            NULL
        FROM
            odb.tblterreinobject_huisnummer th
            INNER JOIN odb.tblterreinobject t ON th.terreinobjectid = t.terreinobjectid
        WHERE
            th.huisnummerid = h.huisnummerid
            AND th.einddatum IS NULL
            AND t.aardterreinobjectcode IN ('1', '4', '5')
    )