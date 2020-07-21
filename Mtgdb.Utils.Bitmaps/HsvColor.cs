using System;

namespace Mtgdb
{
	public readonly struct HsvColor : IEquatable<HsvColor>
	{
		/// <summary>
		/// Creates a new HslColor value.
		/// </summary>
		/// <param name="hue">Hue, as a value between 0 and 1.</param>
		/// <param name="saturation">Saturation, as a value between 0 and 1.</param>
		/// <param name="luminance">Luminance, as a value between 0 and 1.</param>
		public HsvColor(float hue, float saturation, float luminance)
		{
			H = hue.Modulo(1);
			S = preprocess(saturation);
			V = preprocess(luminance);

			float preprocess(float value)
			{
				if (double.IsNaN(value))
					throw new ArgumentOutOfRangeException(nameof(value), value, "Cannot have a NaN channel value.");

				return value.WithinRange(0, 1);
			}
		}

		/// <summary>
		/// Hue as a value between 0 and 1.
		/// </summary>
		public float H { get; }

		/// <summary>
		/// Saturation as a value between 0 and 1.
		/// </summary>
		public float S { get; }

		/// <summary>
		/// Luminosity (brightness) as a value between 0 and 1.
		/// </summary>
		public float V { get; }

		public HsvColor Transform(Func<float, float> h = null, Func<float, float> s = null, Func<float, float> v = null) =>
			new HsvColor(h?.Invoke(H) ?? H, s?.Invoke(S) ?? S, v?.Invoke(V) ?? V);

		public HsvColor Transform(Func<HsvColor, float> h = null, Func<HsvColor, float> s = null, Func<HsvColor, float> v = null) =>
			new HsvColor(h?.Invoke(this) ?? H, s?.Invoke(this) ?? S, v?.Invoke(this) ?? V);

		public bool Equals(HsvColor other) =>
			H.Equals(other.H) && S.Equals(other.S) && V.Equals(other.V);

		public override bool Equals(object obj) =>
			obj is HsvColor other && Equals(other);

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = H.GetHashCode();
				hashCode = (hashCode * 397) ^ S.GetHashCode();
				hashCode = (hashCode * 397) ^ V.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(HsvColor left, HsvColor right) =>
			left.Equals(right);

		public static bool operator !=(HsvColor left, HsvColor right) =>
			!left.Equals(right);

		public override string ToString() =>
			$"H{H:F0} S{S:F2} V{V:F2}";
	}
}