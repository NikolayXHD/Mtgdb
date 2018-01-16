using Lucene.Net.Contrib;
using NUnit.Framework;

namespace Mtgdb.Test
{
	[TestFixture]
	public class EditedTokenLocatorTests : IndexTestsBase
	{
		[TestCase(
			"*:",
			"--^")]
		public void Any_field_is_recognized_as_field(string query, string caretIndicator)
		{
			int caret = caretIndicator.IndexOf("^", Str.Comparison);
			var token = EditedTokenLocator.GetEditedToken(query, caret);

			Assert.That(token, Is.Not.Null);
			Assert.That(token.ParentField, Is.EqualTo("*"));
			Assert.That(token.Type.IsAny(TokenType.FieldValue), Is.True);
		}
	}
}