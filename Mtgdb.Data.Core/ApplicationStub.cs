using System.Threading;

namespace Mtgdb.Data
{
    public class ApplicationStub : IApplication
    {
        public CancellationToken CancellationToken { get; } = CancellationToken.None;
    }
}