namespace Basisregisters.IntegrationDb.Consumer.Address
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Schema;
    using Schema.Models;

    public class BackgroundTask : BackgroundService
    {
        private readonly IKafkaConsumer _myConsumer;
        private readonly IntegrationContext _integrationContext;

        public BackgroundTask(IKafkaConsumer myConsumer, IntegrationContext integrationContext)
        {
            _myConsumer = myConsumer;
            _integrationContext = integrationContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _myConsumer.Consume(async (msg, objectId) =>
            {
                try
                {
                    if (msg is null)
                    {
                        var m = _integrationContext.Addresses.Find(new object[] { objectId });
                        m.IsRemoved = true;

                        Console.WriteLine($"Message is null, {objectId} IsRemoved");
                    }
                    else
                    {
                        var a = new Address();
                        a.PersistentLocalId = (int) msg!["identificator"]["objectId"];
                        a.NisCode = msg["gemeente"]["objectId"];
                        a.PostalCode = msg["postinfo"]["objectId"];
                        a.StreetNamePersistentLocalId = (int) msg!["straatnaam"]["objectId"];
                        a.Status = msg["adresStatus"];
                        a.HouseNumber = msg["huisnummer"];
                        a.BoxNumber = msg["busnummer"];
                        a.FullName = msg["volledigAdres"]["geografischeNaam"]["spelling"];
                        a.GeometryGml = msg["adresPositie"]["geometrie"]["gml"];
                        a.PositionMethod = msg["adresPositie"]["positieGeometrieMethode"];
                        a.PositionSpecification = msg["adresPositie"]["positieSpecificatie"];
                        a.IsOfficiallyAssigned = (bool) msg!["officieelToegekend"];
                        a.IsRemoved = false;
                        a.PuriId = msg["identificator"]["id"];
                        a.Namespace = msg["identificator"]["naamruimte"];
                        a.VersionString = ((DateTimeOffset) msg["identificator"]["versieId"]).ToString("o");
                        a.VersionTimestamp = ((DateTime) msg["identificator"]["versieId"]).ToUniversalTime();
                        _integrationContext.Addresses.Add(a);
                    }

                    _integrationContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException.Message);
                    throw;
                }
            }, stoppingToken);
        }
    }
}
