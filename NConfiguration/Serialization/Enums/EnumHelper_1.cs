using System;

namespace NConfiguration.Serialization.Enums
{
	public sealed class EnumHelper<T> where T: struct
	{
		private static readonly IEnumParser<T> _parser;

		static EnumHelper()
		{
			var type = typeof(T);
			if(!type.IsEnum)
				throw new NotImplementedException("supported only Enum types");
			var underType = type.GetEnumUnderlyingType();
			
			Type parserType;

			if (type.GetCustomAttributes(typeof(FlagsAttribute), true).Length != 0)
			{
				if (underType == typeof(Byte))
					parserType = typeof(ByteFlagEnumParser<T>);
				else if (underType == typeof(SByte))
					parserType = typeof(SByteFlagEnumParser<T>);
				else if (underType == typeof(Int16))
					parserType = typeof(Int16FlagEnumParser<T>);
				else if (underType == typeof(Int32))
					parserType = typeof(Int32FlagEnumParser<T>);
				else if (underType == typeof(Int64))
					parserType = typeof(Int64FlagEnumParser<T>);
				else if (underType == typeof(UInt16))
					parserType = typeof(UInt16FlagEnumParser<T>);
				else if (underType == typeof(UInt32))
					parserType = typeof(UInt32FlagEnumParser<T>);
				else if (underType == typeof(UInt64))
					parserType = typeof(UInt64FlagEnumParser<T>);
				else
					throw new NotImplementedException("unexpected underlying type: " + underType.FullName);
			}
			else
			{
				if (underType == typeof(Byte))
					parserType = typeof(ByteEnumParser<T>);
				else if (underType == typeof(SByte))
					parserType = typeof(SByteEnumParser<T>);
				else if (underType == typeof(Int16))
					parserType = typeof(Int16EnumParser<T>);
				else if (underType == typeof(Int32))
					parserType = typeof(Int32EnumParser<T>);
				else if (underType == typeof(Int64))
					parserType = typeof(Int64EnumParser<T>);
				else if (underType == typeof(UInt16))
					parserType = typeof(UInt16EnumParser<T>);
				else if (underType == typeof(UInt32))
					parserType = typeof(UInt32EnumParser<T>);
				else if (underType == typeof(UInt64))
					parserType = typeof(UInt64EnumParser<T>);
				else
					throw new NotImplementedException("unexpected underlying type: " + underType.FullName);
			}

			_parser = (IEnumParser<T>)Activator.CreateInstance(parserType);
		}

		public static T Parse(string text)
		{
			return _parser.Parse(text);
		}
	}
}
