using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Mtgdb.Controls
{
	public class LayoutControlTypeConverter : TypeConverter
	{
		//private readonly StandardValuesCollection _knownLayoutControlTypes;

		public LayoutControlTypeConverter()
		{
			//_knownLayoutControlTypes = new StandardValuesCollection(
			//	AppDomain.CurrentDomain
			//		.GetAssemblies()
			//		.SelectMany(a => a.GetExportedTypes())
			//		.Where(t => t.IsSubclassOf(typeof (LayoutControl)))
			//		.ToArray());
		}

		//public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		//{
		//	return true;
		//}

		//public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		//{
		//	return _knownLayoutControlTypes;
		//}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof (Type) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
				return Type.GetType((string) value);

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is Type)
				return ((Type) value).AssemblyQualifiedName;

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}