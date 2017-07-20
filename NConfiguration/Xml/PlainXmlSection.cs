using System.Xml;
using System.Xml.Linq;
using System.Configuration;

namespace NConfiguration
{
	/// <summary>
	/// Represents a plain XML in section within a configuration file.
	/// </summary>
	public sealed class PlainXmlSection : ConfigurationSection
	{
		/// <summary>
		/// Plain XML in section within a configuration file.
		/// </summary>
		public XDocument PlainXml { get; set; }

		/// <summary>
		/// Reads XML from the configuration file.
		/// </summary>
		/// <param name="reader">The System.Xml.XmlReader object, which reads from the configuration file.</param>
		protected override void DeserializeSection(XmlReader reader)
		{
			PlainXml = XDocument.Load(reader);
		}
	}
}
