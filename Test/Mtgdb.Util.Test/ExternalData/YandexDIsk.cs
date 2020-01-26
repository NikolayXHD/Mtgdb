using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mtgdb.Downloader;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Mtgdb.Util
{
	[TestFixture]
	public class YandexDIsk
	{
		private const string RootDirPublicKey = "oZys52trJjEUJ9U7XSb4cKpduHKboapl4JvINZqbpIEC85CkM4GdTyqrS2JkBEt5q/J6bpmRyOJonT3VoXnDag==";

		private const string ApiUrl = "https://cloud-api.yandex.net/v1/disk/public/resources";
		const string DirUrl = "https://yadi.sk/d/f1HuKUg7xW2FUQ";
		const int Limit = 1000;
		private const string RootDirResponseFile = @"D:\temp\root_dir.json";

		[Test]
		public async Task Get_directory()
		{
			var http = new HttpClient();
			var client = new WebClientBase();
			var metaUrl = getRootDirUrl(DirUrl);
			var response = await client.GetResponse(http, new HttpRequestMessage(HttpMethod.Get, metaUrl), CancellationToken.None);
			response.EnsureSuccessStatusCode();
			var contentStr = await response.Content.ReadAsStringAsync();
			File.WriteAllText(RootDirResponseFile, contentStr);
		}

		[TestCase("/lq-list/filelist.7z")]
		public async Task Get_link(string path)
		{
			var http = new HttpClient();
			var client = new WebClientBase();

			var rootStr = File.ReadAllText(RootDirResponseFile);
			DirectoryWrapperJson rootMetaInfo = JsonConvert.DeserializeObject<DirectoryWrapperJson>(rootStr);

			var linkUrl = getLinkUrl(new DirectoryEntryJson()
			{
				PublicKey = rootMetaInfo.PublicKey,
				Path = path
			});

			var response = await client.GetResponse(http, new HttpRequestMessage(HttpMethod.Get, linkUrl), CancellationToken.None);
			response.EnsureSuccessStatusCode();
			var linkStr = await response.Content.ReadAsStringAsync();
			var link = JsonConvert.DeserializeObject<LinkJson>(linkStr);
		}

		private string getRootDirUrl(string dirUrl) =>
			$"{ApiUrl}?public_key={dirUrl}&limit={Limit}";

		private string getDirUrl(DirectoryEntryJson entry) =>
			$"{ApiUrl}?public_key={entry.PublicKey}&path={entry.Path}&limit={Limit}";

		private string getLinkUrl(DirectoryEntryJson entry)
		{
			return $"{ApiUrl}/download?public_key={entry.PublicKey}&path={entry.Path}";
		}
	}

	[JsonObject]
	public class DirectoryWrapperJson
	{
		[JsonProperty("public_key")]
		public string PublicKey { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("_embedded")]
		public DirectoryJson Directory { get; set; }
	}

	[JsonObject]
	public class DirectoryJson
	{
		[JsonProperty("items")]
		public DirectoryEntryJson[] Items { get; set; }
	}

	[JsonObject]
	public class DirectoryEntryJson
	{
		[JsonProperty("public_key")]
		public string PublicKey { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }
	}

	[JsonObject]
	public class LinkJson
	{
		[JsonProperty("href")]
		public string Href { get; set; }

		[JsonProperty("method")]
		public string Method { get; set; }

		[JsonProperty("templated")]
		public bool Templated { get; set; }
	}
}
