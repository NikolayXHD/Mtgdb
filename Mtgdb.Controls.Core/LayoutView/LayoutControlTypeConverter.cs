using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;

namespace Mtgdb.Controls
{
	public class LayoutControlTypeConverter : TypeConverter
	{
		[UsedImplicitly]
		public LayoutControlTypeConverter()
		{
		}

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
			if (value is string s)
				return Type.GetType(s);

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is Type type)
				return type.AssemblyQualifiedName;

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}