using System;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class InternedStringConverter : JsonConverter
	{
		public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer) =>
			reader.TokenType == JsonToken.String
				? string.Intern((string) reader.Value)
				: null;

		public override bool CanConvert(Type type) =>
			type == typeof(string);

		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
			throw new NotSupportedException();
	}
}
