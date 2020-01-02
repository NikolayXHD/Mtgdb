using System;
using System.Collections.Generic;

namespace Lucene.Net.Contrib
{
	[Flags]
	public enum TokenType
	{
		None = 0,

		AnyOpen = Open | OpenQuote | OpenRegex,
		AnyClose = Close | CloseQuote | CloseRegex,

		Open = OpenOpenRange | OpenClosedRange | OpenGroup,
		OpenRange = OpenOpenRange | OpenClosedRange,
		OpenOpenRange = 1 << 0,
		OpenClosedRange = 1 << 1,
		OpenGroup = 1 << 2,

		Close = CloseOpenRange | CloseClosedRange | CloseGroup,
		CloseOpenRange = 1 << 5,
		CloseClosedRange = 1 << 6,
		CloseGroup = 1 << 7,

		To = 1 << 10,

		Quote = OpenQuote | CloseQuote,
		OpenQuote = 1 << 11,
		CloseQuote = 1 << 12,

		RegexDelimiter = OpenRegex | CloseRegex,
		OpenRegex = 1 << 13,
		CloseRegex = 1 << 14,

		Boolean = And | Or | Not,
		And = 1 << 16,
		Or = 1 << 17,
		Not = 1 << 18,

		Wildcard = AnyChar | AnyString,
		AnyChar = 1 << 19,
		AnyString = 1 << 20,

		RegexBody = 1 << 21,

		Modifier = BoostModifier | SlopeModifier,
		BoostModifier = 1 << 23,
		SlopeModifier = 1 << 24,

		Field = 1 << 27,
		Colon = 1 << 28,

		FieldValue = 1 << 29,
		ModifierValue = 1 << 30,
		Custom = 1 << 31
	}

	public static class TokenTypeExtension
	{
		public static bool IsAny(this TokenType value, TokenType kind) =>
			value != TokenType.None && (value & kind) == value;

		public static bool IsLegalCloserOf(this TokenType closer, TokenType? opener)
		{
			return opener.HasValue && closer.IsAny(TokenType.Close) && _legalOpenersByCloser[closer].Contains(opener.Value);
		}

		private static readonly Dictionary<TokenType, List<TokenType>> _legalOpenersByCloser = new Dictionary<TokenType, List<TokenType>>
		{
			[TokenType.CloseOpenRange] = new List<TokenType> { TokenType.OpenOpenRange, TokenType.OpenClosedRange },
			[TokenType.CloseClosedRange] = new List<TokenType> { TokenType.OpenClosedRange, TokenType.OpenOpenRange },
			[TokenType.CloseGroup] = new List<TokenType> { TokenType.OpenGroup }
		};
	}
}