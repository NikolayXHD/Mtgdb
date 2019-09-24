using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class EntryListConverter<TVal> : JsonConverter
	{
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
			throw new NotSupportedException();

		public override bool CanConvert(Type objectType) =>
			objectType == typeof(List<KeyValuePair<string, TVal>>);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.StartObject)
				throw new JsonReaderException(
					$"Unexpected token {reader.TokenType} {reader.Value} at {reader.Path} " +
					$"while reading Dictionary<string, {typeof(TVal).Name}> at {reader.Path}");

			var result = new List<KeyValuePair<string, TVal>>();

			while (true)
			{
				reader.Read();

				if (reader.TokenType == JsonToken.EndObject)
					break;

				if (reader.TokenType != JsonToken.PropertyName)
					throw new JsonReaderException($"Property name was expected at {reader.Path}");

				string key = string.Intern((string) reader.Value);
				reader.Read();
				TVal value = serializer.Deserialize<TVal>(reader);
				result.Add(new KeyValuePair<string, TVal>(key, value));
			}

			return result;
		}
	}
}