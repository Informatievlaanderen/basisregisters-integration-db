namespace Basisregisters.IntegrationDb.Consumer.BuildingUnit
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Basisregisters.IntegrationDb.Schema;
    using Basisregisters.IntegrationDb.Schema.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json.Linq;
    using Npgsql;

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
                        var m = _integrationContext.BuildingUnits.Find(new object[] { objectId });
                        m.IsRemoved = true;

                        Console.WriteLine($"Message is null, {objectId} IsRemoved");
                    }
                    else
                    {
                        var persistentLocalId = (int) msg["identificator"]["objectId"];

                        var existing = _integrationContext.BuildingUnits.Find(new object[] { persistentLocalId });

                        var a = existing ?? new BuildingUnit { PersistentLocalId = persistentLocalId };
                        a.BuildingPersistentLocalId = msg["gebouw"]["objectId"];
                        a.Status = msg["gebouweenheidStatus"];
                        a.Function = msg["functie"];
                        a.GeometryGml = msg["gebouweenheidPositie"]["geometrie"]["gml"];
                        a.GeometryMethod = msg["gebouweenheidPositie"]["positieGeometrieMethode"];
                        a.Addresses = string.Join(", ", ((JArray) msg["adressen"]).Select(x => x["objectId"]));
                        a.IsRemoved = false;
                        a.HasDeviation = (bool) msg["afwijkingVastgesteld"];
                        a.PuriId = msg["identificator"]["id"];
                        a.Namespace = msg["identificator"]["naamruimte"];
                        a.VersionString = ((DateTimeOffset) msg["identificator"]["versieId"]).ToString("o");
                        a.VersionTimestamp = ((DateTime) msg["identificator"]["versieId"]).ToUniversalTime();

                        if (existing is null)
                        {
                            _integrationContext.BuildingUnits.Add(a);
                        }
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
