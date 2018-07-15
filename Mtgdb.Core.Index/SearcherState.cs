using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Mtgdb.Index
{
	public class SearcherState<TId, TDoc> : IDisposable
	{
		public SearcherState(
			IDocumentAdapter<TId, TDoc> adapter,
			Func<IEnumerable<IEnumerable<Document>>> getDocumentGroupsToIndex)
		{
			_adapter = adapter;
			_getDocumentGroupsToIndex = getDocumentGroupsToIndex;
		}

		public Directory CreateIndex()
		{
			if (IsLoading || IsLoaded)
				throw new InvalidOperationException();

			IsLoading = true;

			var index = new RAMDirectory();

			var indexWriterAnalyzer = _adapter.CreateAnalyzer();
			var config = IndexUtils.CreateWriterConfig(indexWriterAnalyzer);

			using (var writer = new IndexWriter(index, config))
			{
				void indexDocumentGroup(IEnumerable<Document> documents)
				{
					if (_aborted)
						return;

					foreach (var doc in documents)
					{
						if (_aborted)
							return;

						writer.AddDocument(doc);
					}

					Interlocked.Increment(ref GroupsAddedToIndex);
					IndexingProgress?.Invoke();
				}

				IndexUtils.ForEach(_getDocumentGroupsToIndex(), indexDocumentGroup);

				if (_aborted)
					return null;

				writer.Flush(triggerMerge: true, applyAllDeletes: false);
				writer.Commit();
			}

			IsLoading = false;

			return index;
		}

		public void Load(Directory index)
		{
			_index = index;
			Reader = DirectoryReader.Open(index);
			_searcher = new IndexSearcher(Reader);

			IsLoaded = true;
		}

		public Dictionary<TId, float> Search(Query query)
		{
			var searchResult = SearchIndex(query);

			var relevanceById = searchResult.ScoreDocs
				.GroupBy(d=> _adapter.GetId(Doc(d.Doc)))
				.ToDictionary(gr => gr.Key, gr => gr.First().Score);

			return relevanceById;
		}

		public TopDocs SearchIndex(Query query) =>
			_searcher.SearchWrapper(query, Reader.MaxDoc);

		public Document Doc(int docId) =>
			_searcher.Doc(docId);

		public void Dispose()
		{
			_aborted = true;
			IsLoaded = false;

			Reader?.Dispose();
			_index?.Dispose();
		}

		private bool _aborted;

		private Directory _index;
		private IndexSearcher _searcher;

		public DirectoryReader Reader { get; private set; }

		private readonly IDocumentAdapter<TId, TDoc> _adapter;
		private readonly Func<IEnumerable<IEnumerable<Document>>> _getDocumentGroupsToIndex;

		public int GroupsAddedToIndex;

		public event Action IndexingProgress;

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }
	}
}