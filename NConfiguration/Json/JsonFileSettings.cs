using System;
using System.IO;
using NConfiguration.Json.Parsing;
using NConfiguration.Monitoring;
using System.Text;

namespace NConfiguration.Json
{
	public class JsonFileSettings : JsonSettings, IFilePathOwner, IIdentifiedSource, ILoadedFromFile
	{
		public static JsonFileSettings Create(string fileName)
		{
			return new JsonFileSettings(fileName);
		}

		private readonly JObject _obj;
		private readonly ReadedFileInfo _fileInfo;

		public JsonFileSettings(string fileName)
		{
			try
			{
				JValue val = null;
				_fileInfo = ReadedFileInfo.Create(fileName, stream =>
				{
					using (var sr = new StreamReader(stream, Encoding.UTF8))
						val = JValue.Parse(sr.ReadToEnd());
				});

				if (val.Type != TokenType.Object)
					throw new FormatException("required json object in content");

				_obj = (JObject)val;
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		protected override JObject Root
		{
			get { return _obj; }
		}

		/// <summary>
		/// source identifier the application settings
		/// </summary>
		public virtual string Identity
		{
			get
			{
				return this.GetIdentitySource(_fileInfo.FullName);
			}
		}

		/// <summary>
		/// Directory containing the configuration file
		/// </summary>
		public virtual string Path
		{
			get { return System.IO.Path.GetDirectoryName(_fileInfo.FullName); }
		}

		public virtual ReadedFileInfo FileInfo
		{
			get { return _fileInfo; }
		}
	}
}

