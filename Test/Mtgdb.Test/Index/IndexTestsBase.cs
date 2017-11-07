using System;
using System.Diagnostics;
using Mtgdb.Dal.Index;
using Ninject;
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

				Console.WriteLine($"Index created in {sw.ElapsedMilliseconds} ms");
			}
			else
			{
				var sw = new Stopwatch();
				sw.Start();

				Searcher.LoadIndex(null);

				sw.Stop();
				Console.WriteLine($"Index created in {sw.ElapsedMilliseconds} ms");
			}

			Spellchecker = Searcher.Spellchecker;
		}

		[OneTimeTearDown]
		public void Teardown()
		{
			Spellchecker.Dispose();
			Searcher.Dispose();
		}
	}
}