using System;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class InternedStringConverter : JsonConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
			reader.TokenType == JsonToken.String
				? string.Intern((string) reader.Value)
				: null;

		public override bool CanConvert(Type objectType) =>
			objectType == typeof(string);

		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
			throw new NotSupportedException();
	}
}