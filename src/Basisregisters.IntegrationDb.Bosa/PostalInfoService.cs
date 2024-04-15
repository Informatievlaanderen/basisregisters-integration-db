namespace Basisregisters.IntegrationDb.Bosa
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Threading.Tasks;
    using Extensions;
    using Model.Xml;
    using Repositories;

    public class PostalInfoService(IPostalInfoRepository repo)
    {
        public async Task<ZipArchive> Export(Stream outputStream)
        {
            var items = repo.GetAll();

            var serializable = new XmlPostalInfoRoot
            {
                Source = "flanders",
                Timestamp = DateTimeOffset.Now,
                PostalInfos = items
                    .GroupBy(x => x.PostalCode)
                    .Select(x =>
                    {
                        var postalInfo = x.First();
                        var names = string.Join('/', x.Select(y => y.DutchName));

                        return new XmlPostalInfo
                        {
                            Code = new XmlCode
                            {
                                Namespace = $"{postalInfo.Namespace}/", // Remove slash after comparison
                                ObjectIdentifier = postalInfo.PostalCode,
                                VersionIdentifier = postalInfo.CrabVersionTimestamp
                                                    ?? postalInfo.VersionTimestamp.ToBelgianString()
                            },
                            Name = new XmlName
                            {
                                Language = "nl",
                                Spelling = names
                            }
                        };
                    })
                    .ToArray()
            };

            return await RegistryZipArchiveFactory.Create(
                outputStream,
                serializable,
                $"FlandersPostalInfo{DateTimeOffset.Now:yyyyMMdd}L72.xml");
        }
    }
}
