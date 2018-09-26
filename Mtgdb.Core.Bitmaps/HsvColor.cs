using System;

namespace Mtgdb
{
	public struct HsvColor : IEquatable<HsvColor>
	{
		public HsvColor(float h, float s, float v)
		{
			H = modulo(h, 360);
			S = s;
			V = v;
		}

		public HsvColor Transform(Func<float, float> h = null, Func<float, float> s = null, Func<float, float> v = null) =>
			new HsvColor(h?.Invoke(H) ?? H, s?.Invoke(S) ?? S, v?.Invoke(V) ?? V);

		public float H { get; }
		public float S { get; }
		public float V { get; }

		private static float modulo(float v, int d)
		{
			v = v % d;
			if (v < 0)
				v += d;

			return v;
		}

		public float DistanceTo(HsvColor other, float wH = 1f, float wS = 1f, float wV = 1f) =>
			modulo(H - other.H, 360) / 360f * wH + Math.Abs(S - other.S) * wS + Math.Abs(V - other.V) * wV;

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
	}
}