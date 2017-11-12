/* 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Collections;

namespace SpellChecker.Net.Search.Spell
{
	using System;
	using IndexReader = Lucene.Net.Index.IndexReader;
	using TermEnum = Lucene.Net.Index.TermEnum;
	using Term = Lucene.Net.Index.Term;

	/// <summary> 
	/// Lucene Dictionary
	/// </summary>
	public class LuceneDictionary : IDictionary, System.Collections.Generic.IEnumerable<string>
	{
		private readonly IndexReader _reader;
		private readonly string _field;

		public LuceneDictionary(IndexReader reader, string field)
		{
			_reader = reader;
			_field = field;
		}

		public virtual System.Collections.Generic.IEnumerator<string> GetWordsIterator()
		{
			return new LuceneIterator(this);
		}

		public System.Collections.Generic.IEnumerator<string> GetEnumerator()
		{
			return GetWordsIterator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private sealed class LuceneIterator : System.Collections.Generic.IEnumerator<string>
		{
			private readonly TermEnum _termEnum;
			private Term _actualTerm;
			private bool _hasNextCalled;

			private readonly LuceneDictionary _enclosingInstance;

			public LuceneIterator(LuceneDictionary enclosingInstance)
			{
				_enclosingInstance = enclosingInstance;
				try
				{
					var term = new Term(enclosingInstance._field, "");
					_termEnum = enclosingInstance._reader.Terms(term);
				}
				catch (System.IO.IOException ex)
				{
					Console.Error.WriteLine(ex.StackTrace);
				}
			}

			public string Current
			{
				get
				{
					if (!_hasNextCalled)
						MoveNext();

					_hasNextCalled = false;
					return _actualTerm?.Text;
				}
			}

			object IEnumerator.Current => Current;

			public bool MoveNext()
			{
				_hasNextCalled = true;

				_actualTerm = _termEnum.Term;

				// if there are no words return false
				if (_actualTerm == null)
					return false;

				string fieldt = _actualTerm.Field;
				_termEnum.Next();

				// if the next word doesn't have the same field return false
				if (fieldt != _enclosingInstance._field)
				{
					_actualTerm = null;
					return false;
				}
				return true;
			}

			public void Reset()
			{
			}

			public void Dispose()
			{
				// Nothing
			}
		}
	}
}