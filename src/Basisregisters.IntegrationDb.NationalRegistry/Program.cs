// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Basisregisters.IntegrationDb.NationalRegistry.Model;
using FlatFiles;
using FlatFiles.TypeMapping;

Console.WriteLine("Hello, let's process national registry data!");

using (TextReader textReader = new StringReader("11001263010100002    ADRIAAN SANDERSLEI              0000\n11001263010300036B001ANTWERPSESTEENWEG               0000"))
{
    var mapper = FlatFileRecord.Mapper;
    var records = mapper.Read(textReader).ToList();

    Console.WriteLine(records.Count);
}

Environment.Exit(0);
