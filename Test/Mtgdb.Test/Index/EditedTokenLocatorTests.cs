using Lucene.Net.Contrib;
using Mtgdb.Dal.Index;
using Mtgdb.Index;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class EditedTokenLocatorTests : TestsBase
	{
		[OneTimeSetUp]
		public static void Setup()
		{
			LoadIndexes();
		}

		[TestCase(
			"*:",
			"--^")]
		public void Any_field_is_recognized_as_field(string query, string caretIndicator)
		{
			int caret = caretIndicator.IndexOf("^", Str.Comparison);
			var token = new MtgTolerantTokenizer(query).GetEditedToken(caret);

			Assert.That(token, Is.Not.Null);
			Assert.That(token.ParentField, Is.EqualTo("*"));
			Assert.That(token.Type.IsAny(TokenType.FieldValue), Is.True);
		}
	}
}