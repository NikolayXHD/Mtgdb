using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class InternedStringToIntDictionaryConverter : JsonConverter
	{
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
			throw new NotSupportedException();



		public override bool CanConvert(Type objectType) =>
			objectType.IsAssignableFrom(typeof(Dictionary<string, int>));

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
				return null;

			if (reader.TokenType != JsonToken.StartObject)
				raise();

			var result = new Dictionary<string, int>();
			while (true)
			{
				reader.Read();

				if (reader.TokenType == JsonToken.EndObject)
					break;

				if (reader.TokenType != JsonToken.PropertyName)
					raise();

				string name = string.Intern((string) reader.Value);

				reader.Read();

				if (reader.TokenType != JsonToken.Integer)
					raise();

				result[name] = (int) (long) reader.Value;
			}

			void raise()
			{
				throw new JsonReaderException(
					$"Unexpected token {reader.TokenType} {reader.Value} at {reader.Path} while reading IDictionary<string, int>");
			}

			return result;
		}
	}
}
