using Mtgdb.Data;

namespace Mtgdb.Ui
{
	public interface ISearchSubsystem
	{
		ISearchResultBase SearchResult { get; }
	}
}