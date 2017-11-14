using System.Diagnostics;
using Mtgdb.Dal.Index;
using Ninject;
using NLog;
using NUnit.Framework;

namespace Mtgdb.Test
{
	public class IndexTestsBase : TestsBase
	{
		// ReSharper disable CompareOfFloatsByEqualityOperator
		protected LuceneSearcher Searcher;
		protected LuceneSpellchecker Spellchecker;

		[OneTimeSetUp]
		public void Setup()
		{
			LoadModules();
			LoadCards();
			LoadTranslations();

			Searcher = Kernel.Get<LuceneSearcher>();

			if (!Searcher.IsUpToDate)
			{
				var sw = new Stopwatch();
				sw.Restart();
				Searcher.LoadIndex(Repo);
				sw.Stop();

				Log.Info($"Index created in {sw.ElapsedMilliseconds} ms");
			}
			else
			{
				var sw = new Stopwatch();
				sw.Start();

				Searcher.LoadIndex(null);

				sw.Stop();
				Log.Info($"Index created in {sw.ElapsedMilliseconds} ms");
			}

			Spellchecker = Searcher.Spellchecker;

			LogManager.Flush();
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			Spellchecker.Dispose();
			Searcher.Dispose();
		}

		[TearDown]
		public void Teardown()
		{
			LogManager.Flush();
		}
	}
}