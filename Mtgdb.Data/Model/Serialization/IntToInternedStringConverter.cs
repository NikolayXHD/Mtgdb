using System;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class IntToInternedStringConverter : JsonConverter
	{
		public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
				return string.Intern((string) reader.Value);

			if (reader.TokenType == JsonToken.Integer || reader.TokenType == JsonToken.Float)
				return string.Intern(reader.Value.ToString());

			return null;
		}

		public override bool CanConvert(Type type)
		{
			return type == typeof(int?) ||
			       type == typeof(int) ||
			       type == typeof(string);
		}


		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}
	}
}
