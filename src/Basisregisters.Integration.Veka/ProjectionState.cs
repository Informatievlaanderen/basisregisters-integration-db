namespace Basisregisters.Integration.Veka
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IProjectionState
    {
        Task<int> GetLastPosition(CancellationToken ct);
        Task SetLastPosition(int lastPosition, CancellationToken ct);
    }

    public class FakeProjectionState : IProjectionState
    {
        public Task<int> GetLastPosition(CancellationToken ct)
        {
            const int meldingAfgerondEventPosition = 1618116; // melding b67520d6-a0c8-41ca-8cbd-f33d7e5f3321

            return Task.FromResult(meldingAfgerondEventPosition - 10);
        }

        public async Task SetLastPosition(int lastPosition, CancellationToken ct)
        {

        }
    }
}
