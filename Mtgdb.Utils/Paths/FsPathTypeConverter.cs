using System;
using System.ComponentModel;

namespace Mtgdb
{
	internal class FsPathTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof (string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string path)
				return FsPathPersistence.Deserialize(path);

			return base.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value is string path)
				return FsPathPersistence.Deserialize(path).IsValid();

			return base.IsValid(context, value);
		}
	}
}
