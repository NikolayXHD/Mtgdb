using System.Collections.Generic;
using FluentAssertions;
using Mtgdb.Data;
using Mtgdb.Dev;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class MetadataFromFileNameTests
	{
		[TestCase("name.jpg", ExpectedResult = "name.jpg")]
		[TestCase("name.[set ori].jpg", ExpectedResult = "name.[set ori].jpg")]
		[TestCase("name[set ori].jpg", ExpectedResult = "name.[set ori].jpg")]
		[TestCase("name.[set ogw][set ori].jpg", ExpectedResult = "name.[set ogw,ori].jpg")]
		[TestCase("name.[artist aaron].jpg", ExpectedResult = "name.[artist aaron].jpg")]
		[TestCase("name.[artist aaron][artist james miller].jpg", ExpectedResult = "name.[artist aaron,james miller].jpg")]
		[TestCase("name.[set ogw][set ori].jpg", ExpectedResult = "name.[set ogw,ori].jpg")]
		[TestCase("name.[set ogw][set ori][unknown_prop].jpg", ExpectedResult = "name.[unknown_prop][set ogw,ori].jpg")]
		[TestCase("name.[artist aaron a. miller][artist james].jpg", ExpectedResult = "name.[artist aaron a. miller,james].jpg")]
		public string ArtworkRenamer_merges_artists_and_sets(string input) =>
			ArtworkRenamer.Rename(input);

		[TestCase("name.jpg", ExpectedResult = null)]
		[TestCase("name.[artist aaron].jpg", ExpectedResult = "aaron")]
		[TestCase("name.[artist aaron a. miller].jpg", ExpectedResult = "aaron a. miller")]
		public string ImageRepository_parses_artist(string filename)
		{
			IList<string> set = null;
			IList<string> artist = null;
			ImageRepository.GetMetadataFromName(filename, ref artist, ref set);

			if (artist == null)
				return null;

			artist.Should().HaveCount(1);
			return artist[0];
		}

		[TestCase("name.jpg", ExpectedResult = null)]
		[TestCase("name.[set ori].jpg", ExpectedResult = "ori")]
		public string ImageRepository_parses_set(string filename)
		{
			IList<string> set = null;
			IList<string> artist = null;
			ImageRepository.GetMetadataFromName(filename, ref artist, ref set);

			if (set == null)
				return null;

			set.Should().HaveCount(1);
			return set[0];
		}
	}
}
