namespace Basisregisters.IntegrationDb.Consumer.Municipality
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Newtonsoft.Json.Linq;
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
                bool ArrayContains(string arrayName, string value)
                    => ((JArray) msg["officieleTalen"]).Any(x => (string) x == value);

                try
                {
                    if (msg is null)
                    {
                        var m = _integrationContext.Municipalities.Find(new object[] { objectId });
                        m.IsRemoved = true;

                        Console.WriteLine($"Message is null, {objectId} IsRemoved");
                    }
                    else
                    {
                        var nisCode = (int) msg["identificator"]["objectId"];

                        var existing = _integrationContext.Municipalities.Find(new object[] { nisCode });

                        var a = existing ?? new Municipality { NisCode = nisCode };
                        a.Status = msg["gemeenteStatus"];
                        a.OfficialLanguageDutch = ArrayContains("officieleTalen", "nl");
                        a.OfficialLanguageFrench = ArrayContains("officieleTalen", "fr");
                        a.OfficialLanguageGerman = ArrayContains("officieleTalen", "de");
                        a.OfficialLanguageEnglish = ArrayContains("officieleTalen", "en");
                        a.FacilityLanguageDutch = ArrayContains("faciliteitenTalen", "nl");
                        a.FacilityLanguageFrench = ArrayContains("faciliteitenTalen", "fr");
                        a.FacilityLanguageGerman = ArrayContains("faciliteitenTalen", "de");
                        a.FacilityLanguageEnglish = ArrayContains("faciliteitenTalen", "en");
                        a.IsRemoved = false;
                        a.PuriId = msg["identificator"]["id"];
                        a.Namespace = msg["identificator"]["naamruimte"];
                        a.VersionString = ((DateTimeOffset) msg["identificator"]["versieId"]).ToString("o");
                        a.VersionTimestamp = ((DateTime) msg["identificator"]["versieId"]).ToUniversalTime();

                        if (existing is null)
                        {
                            _integrationContext.Municipalities.Add(a);
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
