using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;

namespace Mtgdb.Data
{
	public abstract class LuceneSearcherState<TId, TDoc> : IDisposable
	{
		protected LuceneSearcherState(IDocumentAdapter<TId, TDoc> adapter)
		{
			Adapter = adapter;
		}

		public Directory CreateIndex()
		{
			if (IsLoading || IsLoaded)
				throw new InvalidOperationException();

			IsLoading = true;

			var index = new RAMDirectory();

			var indexWriterAnalyzer = Adapter.CreateAnalyzer();
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

				IndexUtils.ForEach(GetDocumentGroupsToIndex(), indexDocumentGroup);

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
				.GroupBy(d=> Adapter.GetId(Doc(d.Doc)))
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

		protected readonly IDocumentAdapter<TId, TDoc> Adapter;
		protected abstract IEnumerable<IEnumerable<Document>> GetDocumentGroupsToIndex();

		public int GroupsAddedToIndex;

		public event Action IndexingProgress;

		public bool IsLoading { get; private set; }
		public bool IsLoaded { get; private set; }
	}
}