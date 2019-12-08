using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Mtgdb.Util
{
	public static class Json
	{
		public static string Serialize(object cardsBySet)
		{
			var builder = new StringBuilder();
			using (var writer = new StringWriter(builder))
			{
				using var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 };
				new JsonSerializer().Serialize(jsonWriter, cardsBySet);
			}

			var serialized = builder.ToString();
			return serialized;
		}
	}
}