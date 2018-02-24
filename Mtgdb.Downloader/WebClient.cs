using System;
using System.Net;

namespace Mtgdb.Downloader
{
	internal class WebClient : System.Net.WebClient
	{
		static WebClient()
		{
			ServicePointManager.SecurityProtocol |= (SecurityProtocolType) 3072;
		}

		protected override WebRequest GetWebRequest(Uri uri)
		{
			var request = base.GetWebRequest(uri);
			request.Timeout = 20 * 60 * 1000;
			return request;
		}
	}
}