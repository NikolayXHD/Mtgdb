using System;

namespace Mtgdb.Dal
{
	public class LanguageController
	{
		public LanguageController(string defaultLanguage)
		{
			_language = defaultLanguage;
		}

		public string Language
		{
			get { return _language; }
			set
			{
				if (!Str.Equals(value, _language))
				{
					_language = value;
					LanguageChanged?.Invoke();
				}
			}
		}

		public event Action LanguageChanged;

		private string _language;
	}
}