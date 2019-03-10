using System;

namespace Mtgdb.Data
{
	public class LanguageController
	{
		public LanguageController(string defaultLanguage)
		{
			_language = defaultLanguage;
		}

		public string Language
		{
			get => _language;
			set
			{
				if (Str.Equals(value, _language))
					return;

				_language = value;
				LanguageChanged?.Invoke();
			}
		}

		public event Action LanguageChanged;

		private string _language;
	}
}