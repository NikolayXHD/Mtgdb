using System;
using System.Net;

namespace Mtgdb.Downloader
{
	internal class WebClient : System.Net.WebClient
	{
		protected override WebRequest GetWebRequest(Uri uri)
		{
			var request = base.GetWebRequest(uri);
			request.Timeout = 20 * 60 * 1000;
			return request;
		}
	}
}