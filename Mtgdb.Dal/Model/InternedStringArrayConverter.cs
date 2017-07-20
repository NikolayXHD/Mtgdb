using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mtgdb.Dal
{
	public class InternedStringArrayConverter : JsonConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var result = new List<string>();

			if (reader.TokenType != JsonToken.StartArray)
				throw new JsonReaderException($"Unexpected token {reader.TokenType} {reader.Value} at {reader.Path} while reading IEnumerable<string>");

			while (true)
			{
				reader.Read();
				if (reader.TokenType == JsonToken.String)
				{
					if (reader.Value == null)
						result.Add(null);
					else
						result.Add(string.Intern((string) reader.Value));
				}
				else if (reader.TokenType == JsonToken.EndArray)
					break;
				else if (reader.TokenType != JsonToken.Comment)
					throw new JsonReaderException($"Unexpected token {reader.TokenType} {reader.Value} at {reader.Path} while reading IEnumerable<string>");
			}

			return result;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof (IEnumerable<string>).IsAssignableFrom(objectType);
		}


		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}
	}
}