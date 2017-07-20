using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace NConfiguration.Serialization.Enums
{
	internal sealed class ByteEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;
		private readonly Dictionary<Byte, T> _numMap;

		public ByteEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);
			_numMap = new Dictionary<Byte, T>(count);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				Byte numkey = (Byte)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			Byte num;
			if(Byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Byte.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}

	internal class ByteFlagEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;

		public ByteFlagEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);
			}
		}
		
		private T ParseOne(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			Byte num;
			if(Byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Byte.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return (T)(ValueType)num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		private Byte ParseToNumber(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return (Byte)(ValueType)exist;

			Byte num;
			if(Byte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Byte.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		public T Parse(string text)
		{
			string[] items = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (items.Length == 0)
				return default(T);

			if (items.Length == 1)
				return ParseOne(items[0]);

			Byte result = 0;

			foreach (var item in items)
				result |= ParseToNumber(item);

			return (T)(ValueType)result;
		}
	}

	internal sealed class SByteEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;
		private readonly Dictionary<SByte, T> _numMap;

		public SByteEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);
			_numMap = new Dictionary<SByte, T>(count);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				SByte numkey = (SByte)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			SByte num;
			if(SByte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				SByte.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}

	internal class SByteFlagEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;

		public SByteFlagEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);
			}
		}
		
		private T ParseOne(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			SByte num;
			if(SByte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				SByte.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return (T)(ValueType)num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		private SByte ParseToNumber(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return (SByte)(ValueType)exist;

			SByte num;
			if(SByte.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				SByte.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		public T Parse(string text)
		{
			string[] items = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (items.Length == 0)
				return default(T);

			if (items.Length == 1)
				return ParseOne(items[0]);

			SByte result = 0;

			foreach (var item in items)
				result |= ParseToNumber(item);

			return (T)(ValueType)result;
		}
	}

	internal sealed class Int16EnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;
		private readonly Dictionary<Int16, T> _numMap;

		public Int16EnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);
			_numMap = new Dictionary<Int16, T>(count);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				Int16 numkey = (Int16)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			Int16 num;
			if(Int16.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int16.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}

	internal class Int16FlagEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;

		public Int16FlagEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);
			}
		}
		
		private T ParseOne(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			Int16 num;
			if(Int16.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int16.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return (T)(ValueType)num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		private Int16 ParseToNumber(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return (Int16)(ValueType)exist;

			Int16 num;
			if(Int16.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int16.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		public T Parse(string text)
		{
			string[] items = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (items.Length == 0)
				return default(T);

			if (items.Length == 1)
				return ParseOne(items[0]);

			Int16 result = 0;

			foreach (var item in items)
				result |= ParseToNumber(item);

			return (T)(ValueType)result;
		}
	}

	internal sealed class Int32EnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;
		private readonly Dictionary<Int32, T> _numMap;

		public Int32EnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);
			_numMap = new Dictionary<Int32, T>(count);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				Int32 numkey = (Int32)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			Int32 num;
			if(Int32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int32.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}

	internal class Int32FlagEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;

		public Int32FlagEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);
			}
		}
		
		private T ParseOne(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			Int32 num;
			if(Int32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int32.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return (T)(ValueType)num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		private Int32 ParseToNumber(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return (Int32)(ValueType)exist;

			Int32 num;
			if(Int32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int32.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		public T Parse(string text)
		{
			string[] items = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (items.Length == 0)
				return default(T);

			if (items.Length == 1)
				return ParseOne(items[0]);

			Int32 result = 0;

			foreach (var item in items)
				result |= ParseToNumber(item);

			return (T)(ValueType)result;
		}
	}

	internal sealed class Int64EnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;
		private readonly Dictionary<Int64, T> _numMap;

		public Int64EnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);
			_numMap = new Dictionary<Int64, T>(count);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				Int64 numkey = (Int64)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			Int64 num;
			if(Int64.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int64.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}

	internal class Int64FlagEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;

		public Int64FlagEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);
			}
		}
		
		private T ParseOne(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			Int64 num;
			if(Int64.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int64.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return (T)(ValueType)num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		private Int64 ParseToNumber(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return (Int64)(ValueType)exist;

			Int64 num;
			if(Int64.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				Int64.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		public T Parse(string text)
		{
			string[] items = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (items.Length == 0)
				return default(T);

			if (items.Length == 1)
				return ParseOne(items[0]);

			Int64 result = 0;

			foreach (var item in items)
				result |= ParseToNumber(item);

			return (T)(ValueType)result;
		}
	}

	internal sealed class UInt16EnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;
		private readonly Dictionary<UInt16, T> _numMap;

		public UInt16EnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);
			_numMap = new Dictionary<UInt16, T>(count);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				UInt16 numkey = (UInt16)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			UInt16 num;
			if(UInt16.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt16.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}

	internal class UInt16FlagEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;

		public UInt16FlagEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);
			}
		}
		
		private T ParseOne(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			UInt16 num;
			if(UInt16.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt16.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return (T)(ValueType)num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		private UInt16 ParseToNumber(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return (UInt16)(ValueType)exist;

			UInt16 num;
			if(UInt16.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt16.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		public T Parse(string text)
		{
			string[] items = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (items.Length == 0)
				return default(T);

			if (items.Length == 1)
				return ParseOne(items[0]);

			UInt16 result = 0;

			foreach (var item in items)
				result |= ParseToNumber(item);

			return (T)(ValueType)result;
		}
	}

	internal sealed class UInt32EnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;
		private readonly Dictionary<UInt32, T> _numMap;

		public UInt32EnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);
			_numMap = new Dictionary<UInt32, T>(count);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				UInt32 numkey = (UInt32)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			UInt32 num;
			if(UInt32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt32.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}

	internal class UInt32FlagEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;

		public UInt32FlagEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);
			}
		}
		
		private T ParseOne(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			UInt32 num;
			if(UInt32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt32.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return (T)(ValueType)num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		private UInt32 ParseToNumber(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return (UInt32)(ValueType)exist;

			UInt32 num;
			if(UInt32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt32.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		public T Parse(string text)
		{
			string[] items = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (items.Length == 0)
				return default(T);

			if (items.Length == 1)
				return ParseOne(items[0]);

			UInt32 result = 0;

			foreach (var item in items)
				result |= ParseToNumber(item);

			return (T)(ValueType)result;
		}
	}

	internal sealed class UInt64EnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;
		private readonly Dictionary<UInt64, T> _numMap;

		public UInt64EnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);
			_numMap = new Dictionary<UInt64, T>(count);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);

				UInt64 numkey = (UInt64)(ValueType)item;
				if (!_numMap.TryGetValue(numkey, out exist))
					_numMap.Add(numkey, item);
			}
		}

		public T Parse(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			UInt64 num;
			if(UInt64.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt64.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				if (_numMap.TryGetValue(num, out exist))
					return exist;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}
	}

	internal class UInt64FlagEnumParser<T>: IEnumParser<T> where T: struct
	{
		private readonly Dictionary<string, T> _nameMap;

		public UInt64FlagEnumParser()
		{
			int count = Enum.GetValues(typeof(T)).Length;

			_nameMap = new Dictionary<string, T>(count, NameComparer.Instance);

			foreach (T item in Enum.GetValues(typeof(T)).Cast<T>())
			{
				T exist;
				string strkey = item.ToString();
				if (!_nameMap.TryGetValue(strkey, out exist))
					_nameMap.Add(strkey, item);
			}
		}
		
		private T ParseOne(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return exist;

			UInt64 num;
			if(UInt64.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt64.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return (T)(ValueType)num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		private UInt64 ParseToNumber(string text)
		{
			T exist;
			if (_nameMap.TryGetValue(text, out exist))
				return (UInt64)(ValueType)exist;

			UInt64 num;
			if(UInt64.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture.NumberFormat, out num) ||
				UInt64.TryParse(text.Replace("0x", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out num))
			{
				return num;
			}

			throw new FormatException(string.Format("enum {0} not contain value '{1}'", typeof(T).FullName, text));
		}

		public T Parse(string text)
		{
			string[] items = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			if (items.Length == 0)
				return default(T);

			if (items.Length == 1)
				return ParseOne(items[0]);

			UInt64 result = 0;

			foreach (var item in items)
				result |= ParseToNumber(item);

			return (T)(ValueType)result;
		}
	}

}
