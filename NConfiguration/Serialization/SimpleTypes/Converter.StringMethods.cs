using System;
using System.Collections.Generic;
using System.Globalization;

namespace NConfiguration.Serialization.SimpleTypes
{
	public static partial class Converter
	{
		private static readonly HashSet<Type> _primitiveTypes = new HashSet<Type>
		{
			typeof(Boolean),
			typeof(Byte),
			typeof(SByte),
			typeof(Char),
			typeof(Int16),
			typeof(Int32),
			typeof(Int64),
			typeof(UInt16),
			typeof(UInt32),
			typeof(UInt64),
			typeof(Single),
			typeof(Double),
			typeof(Decimal),
			typeof(TimeSpan),
			typeof(DateTime),
			typeof(Guid),
			typeof(String),
			typeof(byte[])
		};

		private static object CreateFunctionFromString(Type type)
		{
			if(type == typeof(string))
				return (Deserialize<string>)ToString;
			else if(type == typeof(byte[]))
				return (Deserialize<byte[]>)ToByteArray;
			else if(type == typeof(Boolean))
				return (Deserialize<Boolean>)ToBoolean;
			else if(type == typeof(Boolean?))
				return (Deserialize<Boolean?>)ToNBoolean;
			else if(type == typeof(Byte))
				return (Deserialize<Byte>)ToByte;
			else if(type == typeof(Byte?))
				return (Deserialize<Byte?>)ToNByte;
			else if(type == typeof(SByte))
				return (Deserialize<SByte>)ToSByte;
			else if(type == typeof(SByte?))
				return (Deserialize<SByte?>)ToNSByte;
			else if(type == typeof(Char))
				return (Deserialize<Char>)ToChar;
			else if(type == typeof(Char?))
				return (Deserialize<Char?>)ToNChar;
			else if(type == typeof(Int16))
				return (Deserialize<Int16>)ToInt16;
			else if(type == typeof(Int16?))
				return (Deserialize<Int16?>)ToNInt16;
			else if(type == typeof(Int32))
				return (Deserialize<Int32>)ToInt32;
			else if(type == typeof(Int32?))
				return (Deserialize<Int32?>)ToNInt32;
			else if(type == typeof(Int64))
				return (Deserialize<Int64>)ToInt64;
			else if(type == typeof(Int64?))
				return (Deserialize<Int64?>)ToNInt64;
			else if(type == typeof(UInt16))
				return (Deserialize<UInt16>)ToUInt16;
			else if(type == typeof(UInt16?))
				return (Deserialize<UInt16?>)ToNUInt16;
			else if(type == typeof(UInt32))
				return (Deserialize<UInt32>)ToUInt32;
			else if(type == typeof(UInt32?))
				return (Deserialize<UInt32?>)ToNUInt32;
			else if(type == typeof(UInt64))
				return (Deserialize<UInt64>)ToUInt64;
			else if(type == typeof(UInt64?))
				return (Deserialize<UInt64?>)ToNUInt64;
			else if(type == typeof(Single))
				return (Deserialize<Single>)ToSingle;
			else if(type == typeof(Single?))
				return (Deserialize<Single?>)ToNSingle;
			else if(type == typeof(Double))
				return (Deserialize<Double>)ToDouble;
			else if(type == typeof(Double?))
				return (Deserialize<Double?>)ToNDouble;
			else if(type == typeof(Decimal))
				return (Deserialize<Decimal>)ToDecimal;
			else if(type == typeof(Decimal?))
				return (Deserialize<Decimal?>)ToNDecimal;
			else if(type == typeof(TimeSpan))
				return (Deserialize<TimeSpan>)ToTimeSpan;
			else if(type == typeof(TimeSpan?))
				return (Deserialize<TimeSpan?>)ToNTimeSpan;
			else if(type == typeof(DateTime))
				return (Deserialize<DateTime>)ToDateTime;
			else if(type == typeof(DateTime?))
				return (Deserialize<DateTime?>)ToNDateTime;
			else if(type == typeof(Guid))
				return (Deserialize<Guid>)ToGuid;
			else if(type == typeof(Guid?))
				return (Deserialize<Guid?>)ToNGuid;
			
			throw new NotSupportedException("type " + type.FullName + " not support");
		}
		/// <summary>
		/// Convert text to Nullable[Boolean]
		/// </summary>
		public static Boolean? ToNBoolean(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToBoolean(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Byte]
		/// </summary>
		public static Byte? ToNByte(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToByte(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[SByte]
		/// </summary>
		public static SByte? ToNSByte(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToSByte(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Char]
		/// </summary>
		public static Char? ToNChar(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToChar(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Int16]
		/// </summary>
		public static Int16? ToNInt16(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToInt16(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Int32]
		/// </summary>
		public static Int32? ToNInt32(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToInt32(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Int64]
		/// </summary>
		public static Int64? ToNInt64(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToInt64(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[UInt16]
		/// </summary>
		public static UInt16? ToNUInt16(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToUInt16(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[UInt32]
		/// </summary>
		public static UInt32? ToNUInt32(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToUInt32(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[UInt64]
		/// </summary>
		public static UInt64? ToNUInt64(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToUInt64(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Single]
		/// </summary>
		public static Single? ToNSingle(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToSingle(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Double]
		/// </summary>
		public static Double? ToNDouble(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToDouble(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Decimal]
		/// </summary>
		public static Decimal? ToNDecimal(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToDecimal(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[TimeSpan]
		/// </summary>
		public static TimeSpan? ToNTimeSpan(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToTimeSpan(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[DateTime]
		/// </summary>
		public static DateTime? ToNDateTime(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToDateTime(context, node);
		}
		/// <summary>
		/// Convert text to Nullable[Guid]
		/// </summary>
		public static Guid? ToNGuid(IDeserializer context, ICfgNode node)
		{
			if (string.IsNullOrWhiteSpace(node.Text))
				return null;

			return ToGuid(context, node);
		}
	}
}

