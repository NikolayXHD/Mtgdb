namespace Mtgdb.Index
{
	public interface ISearchSubsystem<TId>
	{
		SearchResult<TId> SearchResult { get; }
	}
}