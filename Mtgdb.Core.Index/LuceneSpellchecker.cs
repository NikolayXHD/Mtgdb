using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Contrib;
using Lucene.Net.Store;
using ReadOnlyCollectionsExtensions;

namespace Mtgdb.Index
{
	public abstract class LuceneSpellchecker<TId, TDoc> : IDisposable
	{
		protected LuceneSpellchecker(IDocumentAdapter<TId, TDoc> adapter)
		{
			Adapter = adapter;
			MaxCount = 20;

			_userFields =
				Adapter.FieldByAlias.Keys.OrderBy(Str.Comparer)
					.Concat(Adapter.GetUserFields().OrderBy(Str.Comparer))
					.Select(f => f + ":")
				.ToReadOnlyList();

			_allTokensAreField = _userFields
				.Select(_ => TokenType.Field)
				.ToReadOnlyList();
		}

		public void LoadIndex(LuceneSearcherState<TId, TDoc> searcherState)
		{
			if (!searcherState.IsLoaded)
				throw new InvalidOperationException();

			CreateIndex(searcherState);
		}

		public IntellisenseSuggest Suggest(TextInputState input, string language)
		{
			string query = input.Text;
			int caret = input.Caret;

			var token = new MtgTolerantTokenizer(query).GetEditedToken(caret);

			if (token == null || token.Type.IsAny(TokenType.ModifierValue))
				return LuceneSpellcheckerConstants.EmptySuggest;

			string userField = Adapter.GetActualField(token.ParentField);
			bool isFieldInvalid = !Adapter.IsAnyField(userField) && !Adapter.IsUserField(userField);

			if (!Adapter.IsSuggestAnalyzedIn(userField, language))
				token = token.PhraseStart ?? token;

			string valuePart =
				StringEscaper.Unescape(query.Substring(token.Position, caret - token.Position));

			if (token.Type.IsAny(TokenType.FieldValue | TokenType.Wildcard))
			{
				IReadOnlyList<string> valueSuggest;

				if (isFieldInvalid || string.IsNullOrEmpty(userField) && string.IsNullOrEmpty(valuePart))
					valueSuggest = ReadOnlyList.Empty<string>();
				else
				{
					if (Adapter.IsAnyField(userField))
						valueSuggest = State.SuggestAllFieldValues(valuePart, language);
					else
						valueSuggest = State.SuggestValues(userField, language, valuePart);
				}

				if (!string.IsNullOrEmpty(userField))
					return new IntellisenseSuggest(token, valueSuggest, _allTokensAreValues);

				var fieldSuggest = suggestFields(fieldPart: valuePart);

				var values = fieldSuggest.Concat(valueSuggest).ToReadOnlyList();

				var types = fieldSuggest.Select(_ => TokenType.Field)
					.Concat(valueSuggest.Select(_ => TokenType.FieldValue))
					.ToReadOnlyList();

				return new IntellisenseSuggest(token, values, types);
			}

			if (token.Type.IsAny(TokenType.Field))
				return new IntellisenseSuggest(token, suggestFields(fieldPart: valuePart), _allTokensAreField);

			if (token.Type.IsAny(TokenType.Boolean))
				return new IntellisenseSuggest(token, LuceneSpellcheckerConstants.BooleanOperators, LuceneSpellcheckerConstants.AllTokensAreBoolean);

			return LuceneSpellcheckerConstants.EmptySuggest;
		}

		public IntellisenseSuggest CycleValue(TextInputState input, bool backward, string language)
		{
			string query = input.Text;
			int caret = input.Caret;

			var token = new MtgTolerantTokenizer(query).GetEditedToken(caret);

			if (token == null || token.Type.IsAny(TokenType.ModifierValue))
				return null;

			string userField = Adapter.GetActualField(token.ParentField);
			bool isFieldInvalid = !Adapter.IsAnyField(userField) && !Adapter.IsUserField(userField);

			if (isFieldInvalid)
				return null;

			string currentValue;

			if (!Adapter.IsSuggestAnalyzedIn(userField, language))
			{
				token = token.PhraseStart ?? token;
				currentValue = StringEscaper.Unescape(token.GetPhraseText(query));
			}
			else
			{
				currentValue = StringEscaper.Unescape(token.Value);
			}

			var snapshot = State;
			var allValues = snapshot.GetValuesCache(userField, language);

			if (allValues.Count == 0)
				return null;

			var currentIndex =
				allValues.BinarySearchLastIndexOf(str => Str.Comparer.Compare(str, currentValue) <= 0);

			int increment = backward ? -1 : 1;
			var nextIndex = currentIndex + increment;

			if (nextIndex == allValues.Count)
				nextIndex = 0;
			else if (nextIndex == -1)
				nextIndex = allValues.Count - 1;

			var nextValue = allValues[nextIndex];
			return new IntellisenseSuggest(token, ReadOnlyList.From(nextValue), _allTokensAreValues);
		}



		private IReadOnlyList<string> suggestFields(string fieldPart)
		{
			var fieldSuggest = _userFields
				.Where(_ => _.IndexOf(fieldPart, Str.Comparison) >= 0)
				.OrderByDescending(_ => _.StartsWith(fieldPart, Str.Comparison))
				.ToReadOnlyList();

			return fieldSuggest;
		}

		public void Dispose() =>
			State?.Dispose();

		protected virtual Directory CreateIndex(LuceneSearcherState<TId, TDoc> searcherState)
		{
			var spellchecker = CreateSpellchecker();
			var state = CreateState(searcherState, spellchecker, loaded: false);

			bool stateExisted = State != null;

			if (!stateExisted)
				State = state;

			void progressHandler() =>
				IndexingProgress?.Invoke();

			state.IndexingProgress += progressHandler;
			var result = state.CreateIndex();
			state.IndexingProgress -= progressHandler;

			if (stateExisted)
			{
				State.Dispose();
				State = state;
			}

			return result;
		}

		protected Spellchecker CreateSpellchecker() =>
			new Spellchecker(Adapter.IsAnyField);

		protected void Update(LuceneSpellcheckerState<TId, TDoc> state)
		{
			var oldState = State;
			var newState = state;

			State = newState;
			oldState?.Dispose();
		}

		protected abstract LuceneSpellcheckerState<TId, TDoc> CreateState(LuceneSearcherState<TId, TDoc> searcherState, Spellchecker spellchecker, bool loaded);

		public event Action IndexingProgress;

		public int MaxCount
		{
			get => _allTokensAreValues.Count;
			set => _allTokensAreValues =
				Enumerable.Range(0, value).Select(_ => TokenType.FieldValue).ToReadOnlyList();
		}

		public bool IsLoaded => State?.IsLoaded ?? false;
		public bool IsLoading => State?.IsLoading ?? false;
		public int TotalFields => State?.TotalFields ?? 0;
		public int IndexedFields => State?.IndexedFields ?? 0;

		private LuceneSpellcheckerState<TId, TDoc> State { get; set; }
		private IReadOnlyList<TokenType> _allTokensAreValues;

		private readonly IReadOnlyList<string> _userFields;
		private readonly IReadOnlyList<TokenType> _allTokensAreField;
		protected readonly IDocumentAdapter<TId, TDoc> Adapter;
	}
}