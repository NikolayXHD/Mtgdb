using System;

namespace Mtgdb
{
	public struct HsvColor : IEquatable<HsvColor>
	{
		public float H { get; }
		public float S { get; }
		public float V { get; }

		public HsvColor(float h, float s, float v)
		{
			H = h.Modulo(360);
			S = s.WithinRange(0f, 1f);
			V = v.WithinRange(0f, 1f);
		}

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