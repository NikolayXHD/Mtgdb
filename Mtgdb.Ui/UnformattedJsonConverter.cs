using System;
using Newtonsoft.Json;

namespace Mtgdb.Ui
{
	public class UnformattedJsonConverter : JsonConverter
	{
		public UnformattedJsonConverter(Func<Type, bool> canConvert)
		{
			_canConvert = canConvert;
		}

		public override bool CanConvert(Type objectType) =>
			_canConvert(objectType);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			throw new NotSupportedException();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteRawValue(JsonConvert.SerializeObject(value, Formatting.None));
		}


		public override bool CanRead => false;

		private readonly Func<Type, bool> _canConvert;
	}
}