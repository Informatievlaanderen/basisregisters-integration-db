namespace Basisregisters.IntegrationDb.Bosa
{
    using System.IO;

    public interface IRegistryService
    {
        string GetXmlFileName();
        string GetZipFileName();
        void CreateXml(Stream outputStream);
    }
}
