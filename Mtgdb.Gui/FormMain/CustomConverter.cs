using System;
using System.Collections.Generic;
using Mtgdb.Controls;
using Newtonsoft.Json;

namespace Mtgdb.Gui
{
	internal class CustomConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return
				typeof (IEnumerable<FilterValueState>).IsAssignableFrom(objectType) ||
				typeof (IDictionary<string, int>).IsAssignableFrom(objectType) ||
				typeof (IEnumerable<string>).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.None));
		}

		public override bool CanRead => false;
	}
}