using System;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class InternedStringConverter : JsonConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
				return string.Intern((string) reader.Value);

			return null;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(string);
		}



		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}
	}
}