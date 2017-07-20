using System;
using System.Xml.Linq;
using NConfiguration.Monitoring;

namespace NConfiguration.Xml
{
	/// <summary>
	/// settings loaded from a file
	/// </summary>
	public class XmlFileSettings : XmlSettings, IFilePathOwner, IIdentifiedSource, ILoadedFromFile
	{
		public static XmlFileSettings Create(string fileName)
		{
			return new XmlFileSettings(fileName);
		}

		private readonly ReadedFileInfo _fileInfo;
		private XElement _root;

		/// <summary>
		/// settings loaded from a file
		/// </summary>
		/// <param name="fileName">file name</param>
		public XmlFileSettings(string fileName)
		{
			try
			{
				_fileInfo = ReadedFileInfo.Create(fileName,
					stream => { _root = XDocument.Load(stream).Root; });
			}
			catch(SystemException ex)
			{
				throw new ApplicationException(string.Format("Unable to load file `{0}'", fileName), ex);
			}
		}

		/// <summary>
		/// XML root element that contains all the configuration section
		/// </summary>
		protected override XElement Root
		{
			get
			{
				return _root;
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

