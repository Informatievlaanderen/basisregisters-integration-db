namespace Basisregisters.IntegrationDb.DataIntegrity.Feeds;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IFeedIntegrityRepository
{
    string RegisterName { get; }
    Task RefreshViewAsync(CancellationToken cancellationToken);
    Task<IEnumerable<string>> GetIntegrityErrorsAsync(CancellationToken cancellationToken);
}
