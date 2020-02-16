using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Mtgdb.Data
{
	public class InternedStringArrayConverter : JsonConverter
	{
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
			throw new NotSupportedException();



		public override bool CanConvert(Type type)
		{
			return
				type.IsAssignableFrom(typeof(HashSet<string>)) ||
				type.IsAssignableFrom(typeof(List<string>)) ||
				type.IsAssignableFrom(typeof(string[]));
		}

		public override object ReadJson(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
		{
			if (type == typeof(HashSet<string>))
				return new HashSet<string>(readValues(reader), Str.Comparer);

			if (type == typeof(string[]))
				return readValues(reader).ToArray();

			return new List<string>(readValues(reader));
		}

		private static IEnumerable<string> readValues(JsonReader reader)
		{
			if (reader.TokenType != JsonToken.StartArray)
				throw new JsonReaderException($"Unexpected token {reader.TokenType} {reader.Value} at {reader.Path} while reading IEnumerable<string>");

			while (true)
			{
				reader.Read();

				if (reader.TokenType == JsonToken.EndArray)
					break;

				if (reader.Value == null)
					yield return null;
				else if (reader.TokenType == JsonToken.String)
					yield return string.Intern((string) reader.Value);
				else if (reader.TokenType != JsonToken.Comment)
					throw new JsonReaderException($"Unexpected token {reader.TokenType} {reader.Value} at {reader.Path} while reading IEnumerable<string>");
			}
		}
	}
}
