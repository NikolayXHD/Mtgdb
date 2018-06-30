using Mtgdb.Index;

namespace Mtgdb.Gui
{
	public interface ISearchSubsystem<TId>
	{
		SearchResult<int> SearchResult { get; }
	}
}