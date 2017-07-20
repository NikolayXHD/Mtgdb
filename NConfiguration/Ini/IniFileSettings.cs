using System;
using System.IO;
using System.Collections.Generic;
using NConfiguration.Ini.Parsing;
using NConfiguration.Monitoring;
using System.Text;

namespace NConfiguration.Ini
{
	public class IniFileSettings : IniSettings, IFilePathOwner, IIdentifiedSource, ILoadedFromFile
	{
		public static IniFileSettings Create(string fileName)
		{
			return new IniFileSettings(fileName);
		}

		private readonly List<Section> _sections;
		private readonly ReadedFileInfo _fileInfo;

		public IniFileSettings(string fileName)
		{
			try
			{
				var context = new ParseContext();
				_fileInfo = ReadedFileInfo.Create(fileName, stream =>
				{
					using (var sr = new StreamReader(stream, Encoding.UTF8))
						context.ParseSource(sr.ReadToEnd());
				});
				_sections = new List<Section>(context.Sections);
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		protected override IEnumerable<Section> Sections
		{
			get
			{
				return _sections;
			}
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

