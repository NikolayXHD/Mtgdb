using System.Threading;
using System.Threading.Tasks;

namespace Mtgdb.Data
{
	public interface IDataDownloader
	{
		Task DownloadMtgjson(CancellationToken token);
		Task DownloadPrices(CancellationToken token);
	}
}