using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using NConfiguration.Serialization.Enums;

namespace NConfiguration.Serialization.SimpleTypes
{
	/// <summary>
	/// Allows you to create delegates to convert a string to an instance of a specified type.
	/// Support types: Boolean, Byte, SByte, Char, Int16, Int32, Int64, UInt16, UInt32, UInt64, Single, Double, Decimal, TimeSpan, DateTime, Guid
	///  and their nullable variants.
	/// </summary>
	public static partial class Converter
	{
		public static bool IsPrimitive(Type type)
		{
			var ntype = Nullable.GetUnderlyingType(type);
			if(ntype != null) // is Nullable<>
				type = ntype;

			if (type.IsEnum)
				return true;

			return _primitiveTypes.Contains(type);
		}

		private static readonly CultureInfo _ci = CultureInfo.InvariantCulture;

		public static object TryCreateFunction(Type targetType)
		{
			return IsPrimitive(targetType) ? CreateFunction(targetType) : null;
		}

		/// <summary>
		/// Creates a delegate to convert a string to an instance of a specified type.
		/// </summary>
		/// <param name="type">Type of the desired object.</param>
		/// <returns>Instance of Func[string, type].</returns>
		public static object CreateFunction(Type type)
		{
			if (type == typeof(string))
				return CreateNativeConverter();

			if (type.IsEnum)
				return CreateStringToEnum(type);

			var undtype = Nullable.GetUnderlyingType(type);
			if (undtype != null && undtype.IsEnum) // is nullable enum
				return CreateStringToNullableEnum(type, undtype);

			return CreateFunctionFromString(type);
		}

		private static object CreateNativeConverter()
		{
			var mi = typeof(Converter).GetMethod("NativeConverter", BindingFlags.NonPublic | BindingFlags.Static);
			var funcType = typeof(Deserialize<string>);
			return Delegate.CreateDelegate(funcType, mi);
		}

		private static string NativeConverter(IDeserializer context, ICfgNode node)
		{
			return node.Text;
		}

		private static object CreateStringToEnum(Type type)
		{
			var mi = typeof(Converter).GetMethod("ToEnum", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(type);
			var funcType = typeof(Deserialize<>).MakeGenericType(type);
			return Delegate.CreateDelegate(funcType, mi);
		}

		public static T ToEnum<T>(IDeserializer context, ICfgNode node) where T : struct
		{
			return EnumHelper<T>.Parse(node.Text);
		}

		private static object CreateStringToNullableEnum(Type type, Type underlyingType)
		{
			var mi = typeof(Converter).GetMethod("ToNullableEnum", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(underlyingType);
			var funcType = typeof(Deserialize<>).MakeGenericType(type);
			return Delegate.CreateDelegate(funcType, mi);
		}

		public static T? ToNullableEnum<T>(IDeserializer context, ICfgNode node) where T : struct
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return EnumHelper<T>.Parse(node.Text);
		}

		/// <summary>
		/// Converts the specified string, which encodes binary data as base-64 digits, to an equivalent 8-bit unsigned integer array.
		/// </summary>
		/// <param name="text">The string to convert.</param>
		public static Byte[] ToByteArray(IDeserializer context, ICfgNode node)
		{
			return System.Convert.FromBase64String(node.Text);
		}

		/// <summary>
		/// Converts the string representation of a char to its Char equivalent.
		/// </summary>
		/// <param name="text">A string that represents the char to convert.</param>
		public static Char ToChar(IDeserializer context, ICfgNode node)
		{
			if (node.Text.Length != 1)
				throw new ArgumentOutOfRangeException("text", "must contain only one char");
			return node.Text[0];
		}

		/// <summary>
		/// Converts the string representation of a number to its 16-bit unsigned integer equivalent.
		/// </summary>
		/// <param name="text">A string that represents the number to convert.</param>
		public static UInt16 ToUInt16(IDeserializer context, ICfgNode node)
		{
			return UInt16.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 16-bit signed integer equivalent.
		/// </summary>
		public static Int16 ToInt16(IDeserializer context, ICfgNode node)
		{
			return Int16.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 32-bit unsigned integer equivalent.
		/// </summary>
		public static UInt32 ToUInt32(IDeserializer context, ICfgNode node)
		{
			return UInt32.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 32-bit signed integer equivalent.
		/// </summary>
		public static Int32 ToInt32(IDeserializer context, ICfgNode node)
		{
			return Int32.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 64-bit unsigned integer equivalent.
		/// </summary>
		public static UInt64 ToUInt64(IDeserializer context, ICfgNode node)
		{
			return UInt64.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 64-bit signed integer equivalent.
		/// </summary>
		public static Int64 ToInt64(IDeserializer context, ICfgNode node)
		{
			return Int64.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its single-precision floating-point number equivalent.
		/// </summary>
		public static Single ToSingle(IDeserializer context, ICfgNode node)
		{
			return Single.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its double-precision floating-point number equivalent.
		/// </summary>
		public static Double ToDouble(IDeserializer context, ICfgNode node)
		{
			return Double.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its System.Decimal number equivalent.
		/// </summary>
		public static Decimal ToDecimal(IDeserializer context, ICfgNode node)
		{
			return Decimal.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its 8-bit signed integer equivalent.
		/// </summary>
		public static SByte ToSByte(IDeserializer context, ICfgNode node)
		{
			return SByte.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a number to its System.Byte equivalent.
		/// </summary>
		public static Byte ToByte(IDeserializer context, ICfgNode node)
		{
			return Byte.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a time interval to its System.TimeSpan equivalent.
		/// </summary>
		public static TimeSpan ToTimeSpan(IDeserializer context, ICfgNode node)
		{
			return TimeSpan.Parse(node.Text, _ci);
		}

		/// <summary>
		/// Converts the string representation of a GUID to the equivalent System.Guid structure.
		/// </summary>
		public static Guid ToGuid(IDeserializer context, ICfgNode node)
		{
			return Guid.Parse(node.Text);
		}

		/// <summary>
		/// Converts the specified string representation of a date and time to its System.DateTime equivalent.
		/// </summary>
		public static DateTime ToDateTime(IDeserializer context, ICfgNode node)
		{
			return DateTime.Parse(node.Text, _ci,
				DateTimeStyles.AdjustToUniversal |
				DateTimeStyles.AllowWhiteSpaces |
				DateTimeStyles.AssumeUniversal |
				DateTimeStyles.NoCurrentDateDefault);
		}

		/// <summary>
		/// returns the origin string
		/// </summary>
		public static string ToString(IDeserializer context, ICfgNode node)
		{
			return node.Text;
		}

		private static Dictionary<string, bool> _booleanMap = new Dictionary<string, bool>(NameComparer.Instance)
		{
			{"true", true},
			{"yes", true},
			{"1", true},
			{"+", true},
			{"t", true},
			{"y", true},
			{"false", false},
			{"no", false},
			{"0", false},
			{"-", false},
			{"f", false},
			{"n", false}
		};

		/// <summary>
		/// Converts the specified string representation of a logical value to its System.Boolean equivalent.
		/// Support formats: true/false, t/f, yes/no, y/n, 1/0, +/- in case insensitivity.
		/// </summary>
		public static bool ToBoolean(IDeserializer context, ICfgNode node)
		{
			bool result;
			if (_booleanMap.TryGetValue(node.Text, out result))
				return result;

			throw new FormatException(string.Format("can not convert '{0}' to a boolean type", node.Text));
		}
	}
}

