using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using NConfiguration.Serialization;

namespace Mtgdb
{
	[TypeConverter(typeof(FsPathTypeConverter))]
	[Deserializer(typeof(FsPathDeserializer<>))]
	public readonly struct FsPath : IEquatable<FsPath>
	{
		public FsPath(string value)
		{
			Value = value;
		}

		public FsPath(string value1, string value2)
			: this(Path.Combine(value1, value2))
		{
		}

		public FsPath(string value1, string value2, string value3)
			: this(Path.Combine(value1, value2, value3))
		{
		}

		public FsPath(string value1, string value2, string value3, string value4)
			: this(Path.Combine(value1, value2, value3, value4))
		{
		}

		public FsPath(params string[] values)
			: this(Path.Combine(values))
		{
		}

		public FsPath Parent() =>
			new FsPath(Path.GetDirectoryName(Value));

		public bool IsPathRooted() =>
			Path.IsPathRooted(Value);

		public FsPath RelativeTo(FsPath other, StringComparison? comparison = null)
		{
			if (string.IsNullOrEmpty(other.Value))
				return this;


			int len = Value.Length;
			if (len == other.Length)
			{
				return Value.StartsWith(other.Value, comparison ?? Comparison)
					? Empty
					: None;
			}

			int minLen = other.Value.Length + 1;
			if (len < minLen || Value[other.Value.Length] != Separator || !Value.StartsWith(other.Value, comparison ?? Comparison))
				return None;

			if (len == minLen)
				return Empty;

			return new FsPath(Value.Substring(minLen));
		}

		public string Basename(bool extension = true)
		{
			if (string.IsNullOrEmpty(Value))
				return Value;

			if (extension)
				return Path.GetFileName(Value);

			return Path.GetFileNameWithoutExtension(Value);
		}

		public FsPath Base(bool extension = true) =>
			new FsPath(Basename(extension));


		public FsPath Join(FsPath other) =>
			new FsPath(Path.Combine(Value, other.Value));

		public FsPath Join(FsPath other1, FsPath other2) =>
			new FsPath(Path.Combine(Value, other1.Value, other2.Value));

		public FsPath Join(FsPath other1, FsPath other2, FsPath other3) =>
			new FsPath(Path.Combine(Value, other1.Value, other2.Value, other3.Value));

		public FsPath Join(string other) =>
			new FsPath(Path.Combine(Value, other));

		public FsPath Join(string other1, string other2) =>
			new FsPath(Path.Combine(Value, other1, other2));

		public FsPath Join(string other1, string other2, string other3) =>
			new FsPath(Path.Combine(Value, other1, other2, other3));

		public FsPath Join(params string[] others)
		{
			var values = new string[others.Length + 1];
			values[0] = Value;
			others.CopyTo(values, 1);
			return new FsPath(Path.Combine(values));
		}


		public FsPath Concat(FsPath other) =>
			Concat(other.Value);

		public FsPath Concat(string other)
		{
			if (Value == null)
				return this;

			if (other == null)
				return None;

			return new FsPath(Value + other);
		}

		public FsPath Intern(bool doIntern = true)
		{
			if (Value != null && doIntern)
				return new FsPath(string.Intern(Value));

			return this;
		}

		public bool IsValid() =>
			Value != null && Value.All(c => !InvalidChars.Contains(c));

		public override string ToString() =>
			Value;

		public int Length => Value.Length;


		public bool Equals(FsPath other) =>
			Comparer.Equals(Value, other.Value);

		public override bool Equals(object obj) =>
			obj is FsPath other && Comparer.Equals(Value, other.Value);

		public override int GetHashCode() =>
			Value == null ? 0 : Value.GetHashCode();

		public static bool operator ==(FsPath left, FsPath right) =>
			Comparer.Equals(left.Value, right.Value);

		public static bool operator !=(FsPath left, FsPath right) =>
			!Comparer.Equals(left.Value, right.Value);

		public readonly string Value;

		public static readonly FsPath Empty = new FsPath(string.Empty);
		public static readonly FsPath None = new FsPath((string) null);

		internal const StringComparison Comparison = StringComparison.Ordinal;
		internal static readonly StringComparer Comparer = StringComparer.Ordinal;
		public static readonly char Separator = Path.DirectorySeparatorChar;
		private static readonly HashSet<char> InvalidChars = new HashSet<char>(Path.GetInvalidPathChars());
	}
}
