using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lucene.Net.Contrib;
using Lucene.Net.Queries.Mlt;
using Lucene.Net.Search;
using Token = Lucene.Net.QueryParsers.Classic.Token;

namespace Mtgdb.Data.Index
{
	public class CardQueryParser : MtgQueryParser
	{
		public CardQueryParser(
			MtgAnalyzer analyzer,
			CardRepository repository,
			CardDocumentAdapter adapter,
			string language)
			: base(analyzer, adapter, language)
		{
			_repository = repository;
		}

		protected override Query HandleBareTokenQuery(string field, Token termToken, Token slopToken, bool prefix, bool wildcard, bool fuzzy, bool regexp)
		{
			if (Str.Equals(Like, field))
			{
				if (prefix || wildcard || regexp || fuzzy)
					return MatchNothingQuery;

				return getMoreLikeQuery(termToken.Image, slopToken);
			}

			return base.HandleBareTokenQuery(field, termToken, slopToken, prefix, wildcard, fuzzy, regexp);
		}

		protected override Query HandleQuotedTerm(string field, Token termToken, Token slopToken)
		{
			string termImage = termToken.Image.Substring(1, termToken.Image.Length - 2);

			if (Str.Equals(Like, field))
				return getMoreLikeQuery(termImage, slopToken);

			return base.HandleQuotedTerm(field, termToken, slopToken);
		}

		protected override IEnumerable<string> ExpandAnyField() =>
			base.ExpandAnyField().Except(Sequence.From(Like), Str.Comparer);

		private Query getMoreLikeQuery(string termImage, Token slopToken)
		{
			string unescaped = StringEscaper.Unescape(termImage);
			float slop = parseSlop(slopToken);

			if (string.IsNullOrEmpty(unescaped))
				return MatchNothingQuery;

			if (!_repository.IsLoadingComplete.Signaled)
				return MatchNothingQuery;

			string cardName = unescaped.RemoveDiacritics();
			if (!_repository.CardsByName.TryGetValue(cardName, out var cards))
				return MatchNothingQuery;

			var card = cards[0];

			var result = new DisjunctionMaxQuery(0.1f);

			if (!string.IsNullOrEmpty(card.TextEn))
				result.Add(createMoreLikeThisQuery(slop, card.TextEn, nameof(card.TextEn)));

			if (!string.IsNullOrEmpty(card.GeneratedMana))
				result.Add(createMoreLikeThisQuery(slop, card.GeneratedMana, nameof(card.GeneratedMana)));

			if (result.Disjuncts.Count == 0)
				return MatchNothingQuery;

			return result;
		}

		private static float parseSlop(Token fuzzySlop)
		{
			if (fuzzySlop == null)
				return 0f;

			if (fuzzySlop.Image.Length <= 1)
				return 0f;

			float.TryParse(fuzzySlop.Image.Substring(1), NumberStyles.Float, Str.Culture, out var slop);

			return slop;
		}

		private MoreLikeThisQuery createMoreLikeThisQuery(float slop, string value, string field)
		{
			if (slop <= 0f)
				slop = 0.6f;
			else if (slop > 1f)
				slop = 1f;

			field = field.ToLower(Str.Culture);
			return new MoreLikeThisQuery(
				value,
				Array.From(field),
				Analyzer,
				field)
			{
				PercentTermsToMatch = slop,
				MaxQueryTerms = 20,
				MinDocFreq = 3,
				StopWords = _moreLikeStopWords
			};
		}



		private readonly CardRepository _repository;

		private static readonly ISet<string> _moreLikeStopWords = new HashSet<string>(
			Sequence.From("a", "an", "the")
				.Concat(MtgAlphabet.SingletoneWordChars.Select(c => new string(c, 1))),
			Str.Comparer);

		public const string Like = nameof(Like);
	}
}
