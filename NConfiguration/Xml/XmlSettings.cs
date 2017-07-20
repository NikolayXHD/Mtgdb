using System.Collections.Generic;
using System.Xml.Linq;
using NConfiguration.Xml.Protected;
using NConfiguration.Serialization;

namespace NConfiguration.Xml
{
	/// <summary>
	/// This settings loaded from a XML document
	/// </summary>
	public abstract class XmlSettings : CachedConfigNodeProvider, IXmlEncryptable
	{
		/// <summary>
		/// XML root element that contains all the configuration section
		/// </summary>
		protected abstract XElement Root { get; }

		public IProviderCollection Providers { get; set; }

		protected override IEnumerable<KeyValuePair<string, ICfgNode>> GetAllNodes()
		{
			if (Root == null)
				yield break;

			foreach (var at in Root.Attributes())
				yield return new KeyValuePair<string, ICfgNode>(at.Name.LocalName, new ViewPlainField(at.Value));

			foreach (var el in Root.Elements())
				yield return new KeyValuePair<string, ICfgNode>(el.Name.LocalName, new XmlViewNode(this, el));
		}
	}
}

