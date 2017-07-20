using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NConfiguration.Serialization;
using NConfiguration.Xml.Protected;
using System.Xml;
using System.Security.Cryptography;

namespace NConfiguration.Xml
{
	/// <summary>
	/// The mapping XML-document to nodes of configuration
	/// </summary>
	public sealed class XmlViewNode : CfgNode
	{
		private static readonly XNamespace _cryptDataNs = XNamespace.Get("http://www.w3.org/2001/04/xmlenc#");

		private IXmlEncryptable _xmlEncryptable;
		private bool _decrypted = false;
		private XElement _element;

		/// <summary>
		/// The mapping XML-document to nodes of configuration
		/// </summary>
		/// <param name="converter">string converter into a simple values</param>
		/// <param name="element">XML element</param>
		public XmlViewNode(IXmlEncryptable xmlEncryptable,  XElement element)
		{
			_xmlEncryptable = xmlEncryptable;
			_element = element;
		}

		private XElement decrypt(XElement el)
		{
			if (el == null)
				return null;

			var attr = el.Attribute("configProtectionProvider");
			if (attr == null)
				return el;

			if (_xmlEncryptable == null)
				throw new InvalidOperationException("protection providers not configured");

			var provider = _xmlEncryptable.Providers.Get(attr.Value);
			if (provider == null)
				throw new InvalidOperationException(string.Format("protection provider `{0}' not found", attr.Value));

			var encData = el.Element(_cryptDataNs + "EncryptedData");
			if (encData == null)
				throw new FormatException(string.Format("element `EncryptedData' not found in element `{0}'", el.Name));

			var xmlEncData = encData.ToXmlElement();
			XmlElement xmlData;

			try
			{
				xmlData = (XmlElement)provider.Decrypt(xmlEncData);
			}
			catch (SystemException sex)
			{
				throw new CryptographicException(string.Format("can't decrypt the configuration section `{0}'", encData.Name), sex);
			}

			return xmlData.ToXElement();
		}

		private void tryDecrypt()
		{
			if (_decrypted)
				return;

			_element = decrypt(_element);
			_decrypted = true;
		}

		public override string GetNodeText()
		{
			tryDecrypt();

			return _element.Value;
		}

		public override IEnumerable<KeyValuePair<string, ICfgNode>> GetNestedNodes()
		{
			tryDecrypt();

			foreach (var attr in _element.Attributes())
				yield return new KeyValuePair<string, ICfgNode>(attr.Name.LocalName, new ViewPlainField(attr.Value));

			foreach (var el in _element.Elements())
				yield return new KeyValuePair<string, ICfgNode>(el.Name.LocalName, new XmlViewNode(_xmlEncryptable, el));
		}
	}
}

