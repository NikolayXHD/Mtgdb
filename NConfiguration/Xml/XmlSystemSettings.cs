using System;
using System.Xml.Linq;

namespace NConfiguration.Xml
{
	public sealed class XmlSystemSettings : XmlFileSettings
	{
		private readonly string _sectionName;
		private readonly string _configPath;

		public XmlSystemSettings(string sectionName, string configPath = null)
			: base(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)
		{
			_sectionName = sectionName;
			_configPath = configPath;
		}

		/// <summary>
		/// XML root element that contains all the configuration section
		/// </summary>
		protected override XElement Root
		{
			get
			{
				var root = base.Root.Element(XName.Get(_sectionName));
				if (root == null)
					throw new FormatException(string.Format("section '{0}' not found ", _sectionName));

				return root;
			}
		}

		public override string Path
		{
			get
			{
				return _configPath ?? base.Path;
			}
		}
	}
}
